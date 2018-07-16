using NIPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NIPS.Controllers
{
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        public HttpResponseMessage Get()
        {
            try
            {
                using (NipsDBEntities entities = new NipsDBEntities())
                {
                    List<User> users = entities.Users.ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, users);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("GetUserPoints")]
        public HttpResponseMessage Get(string userName)
        {
            try
            {
                using (NipsDBEntities entities = new NipsDBEntities())
                {
                    var id = entities.AspNetUsers.FirstOrDefault(u => u.UserName == userName).Id;
                    User curUser = entities.Users.FirstOrDefault(u => u.ID == id);
                    List<User> users = entities.Users.ToList<User>();
                    Dictionary<string, int> retDict = new Dictionary<string, int>();
                    retDict.Add("totalPoints", curUser.TotalPoints);
                    retDict.Add("semesterPoints" , curUser.SemesterPoints);
                    retDict.Add("weekPoints", curUser.WeekPoints);
                    //retDict.Add("weekRank", 1);
                    //retDict.Add("semesterRank", 1);
                    retDict.Add("weekRank" , getWeekRank(curUser, users));
                    retDict.Add("semesterRank" , getSemesterRank(curUser, users));
                    return Request.CreateResponse(HttpStatusCode.OK, retDict);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("GetAccessLevel")]
        public HttpResponseMessage GetAccessLevel(string userName)
        {
            try
            {
                using (NipsDBEntities entities = new NipsDBEntities())
                {
                    var id = entities.AspNetUsers.FirstOrDefault(u => u.UserName == userName).Id;
                    User curUser = entities.Users.FirstOrDefault(u => u.ID == id);
                    return Request.CreateResponse(HttpStatusCode.OK, curUser.AccessLevel);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("GetPointsTable")]
        public HttpResponseMessage GetPointsTable(string userName)
        {
            var accessLevelGetter = GetAccessLevel(userName).Content;
            int accessLevel = (int) ((ObjectContent)accessLevelGetter).Value;
            if (accessLevel == 2)
            {
                try
                {
                    using (NipsDBEntities entities = new NipsDBEntities())
                    {
                        List<RankFormattedUser> retList = new List<RankFormattedUser>();
                        List<User> users = entities.Users.ToList();
                        foreach (User u in users)
                        {
                            RankFormattedUser retUser = new RankFormattedUser
                            {
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                WeekPoints = u.WeekPoints,
                                SemesterPoints = u.SemesterPoints,
                                TotalPoints = u.TotalPoints,
                                WeekRank = getWeekRank(u, users),
                                SemesterRank = getSemesterRank(u, users),
                            };
                            retList.Add(retUser);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, retList);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        [Route("GetLedger")]
        public HttpResponseMessage GetLedger(string userName)
        {
            var accessLevelGetter = GetAccessLevel(userName).Content;
            int accessLevel = (int)((ObjectContent)accessLevelGetter).Value;
            if (accessLevel == 2)
            {
                try
                {
                    using (NipsDBEntities entities = new NipsDBEntities())
                    {
                        List<Ledger> ledger = entities.Ledgers.ToList();
                        foreach (Ledger l in ledger)
                        {
                            l.GiverID = IDtoName(l.GiverID);
                            l.GetterID = IDtoName(l.GetterID);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, ledger);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        [Route("GivePoints")]
        public HttpResponseMessage Post(Ledger ledgerEntry)
        {
            try
            {
                using (NipsDBEntities entities = new NipsDBEntities())
                {
                    string giver = ledgerEntry.GiverID;
                    string getter = ledgerEntry.GetterID;
                    int amount = ledgerEntry.Amount;
                    string reason = ledgerEntry.Reason;
                    var giverId = entities.AspNetUsers.FirstOrDefault(u => u.UserName == giver).Id;
                    var ledgerKey = 0;
                    try
                    {
                        ledgerKey = entities.Ledgers.Select(l => l.LedgerID).Max() + 1;
                    }
                    catch (Exception ex)
                    {
                        ledgerKey = 0;
                    }
                    var giverAccessLevel = entities.Users.FirstOrDefault(u => u.ID == giverId).AccessLevel;
                    if (giverAccessLevel > 0)
                    {
                        var lastName = getter.Split(',')[0];
                        var firstName = getter.Split(',')[1];
                        var getterObj = entities.Users.FirstOrDefault(u => u.LastName == lastName && u.FirstName == firstName);
                        getterObj.TotalPoints += amount;
                        getterObj.SemesterPoints += amount;
                        getterObj.WeekPoints += amount;
                        ledgerEntry.LedgerID = ledgerKey;
                        ledgerEntry.GiverID = giverId;
                        ledgerEntry.GetterID = getterObj.ID;
                        entities.Ledgers.Add(ledgerEntry);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Created);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        [Route("ChangeAccess")]
        public HttpResponseMessage ChangeAccessLevel(AccessLevelData accessData)
        {
            var accessLevelGetter = GetAccessLevel(accessData.AdminUserName).Content;
            int accessLevel = (int)((ObjectContent)accessLevelGetter).Value;
            if (accessLevel == 2)
            {
                try
                {
                    using (NipsDBEntities entities = new NipsDBEntities())
                    {
                        var lastName = accessData.BrotherName.Split(',')[0];
                        var firstName = accessData.BrotherName.Split(',')[1];
                        var brother = entities.Users.FirstOrDefault(u => u.LastName == lastName && u.FirstName == firstName);
                        brother.AccessLevel = accessData.NewAccessLevel;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        public HttpResponseMessage Delete(DeleteLedgerData deleteInfo)
        {

            var accessLevelGetter = GetAccessLevel(deleteInfo.AdminUserName).Content;
            int accessLevel = (int)((ObjectContent)accessLevelGetter).Value;
            if (accessLevel == 2)
            {
                try
                {
                    using (NipsDBEntities entities = new NipsDBEntities())
                    {
                        Ledger ledgerEntry = entities.Ledgers.FirstOrDefault(l => l.LedgerID == deleteInfo.LedgerID);
                        User getterObj = entities.Users.FirstOrDefault(u => u.ID == ledgerEntry.GetterID);
                        getterObj.TotalPoints -= ledgerEntry.Amount;
                        getterObj.SemesterPoints -= ledgerEntry.Amount;
                        getterObj.WeekPoints -= ledgerEntry.Amount;
                        entities.Ledgers.Remove(entities.Ledgers.FirstOrDefault(l => l.LedgerID == deleteInfo.LedgerID));
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Accepted);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        [Route("DeleteUser")]
        public HttpResponseMessage DeleteUser(DeleteUserData deleteInfo)
        {
            var accessLevelGetter = GetAccessLevel(deleteInfo.AdminUserName).Content;
            int accessLevel = (int)((ObjectContent)accessLevelGetter).Value;
            if (accessLevel == 2)
            {
                try
                {
                    using (NipsDBEntities entities = new NipsDBEntities())
                    {
                        var lastName = deleteInfo.DeleteName.Split(',')[0];
                        var firstName = deleteInfo.DeleteName.Split(',')[1];
                        var id = entities.Users.FirstOrDefault(u => u.LastName == lastName && u.FirstName == firstName).ID;
                        entities.AspNetUsers.Remove(entities.AspNetUsers.FirstOrDefault(u => u.Id == id));
                        entities.Ledgers.RemoveRange(entities.Ledgers.Where(l => l.GetterID == id));
                        entities.Users.Remove(entities.Users.FirstOrDefault(u => u.ID == id));
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Accepted);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        private int getWeekRank(User curUser, List<User> users)
        {
            users = users.OrderByDescending(u => u.WeekPoints).ToList();
            int index = users.IndexOf(curUser);
            if (index == 0)
            {
                return 1;
            }
            else if (users.ElementAt(index - 1).WeekPoints == curUser.WeekPoints)
            {
                while (users.ElementAt(index).WeekPoints == curUser.WeekPoints && index != 0)
                {
                    index -= 1;
                }
            }
            return index + 1;
        }

        private int getSemesterRank(User curUser, List<User> users)
        {
            users = users.OrderByDescending(u => u.SemesterPoints).ToList();
            int index = users.IndexOf(curUser);
            if (index == 0) {
                return 1;
            }
            else if (users.ElementAt(index - 1).SemesterPoints == curUser.SemesterPoints)
            {
                while (users.ElementAt(index).SemesterPoints == curUser.SemesterPoints && index != 0)
                {
                    index -= 1;
                }
            }
            return index + 1;
        }
        
        private string IDtoName(string id)
        {
            using (NipsDBEntities entities = new NipsDBEntities())
            {
                string lastName = entities.Users.FirstOrDefault(u => u.ID == id).LastName;
                string firstName = entities.Users.FirstOrDefault(u => u.ID == id).FirstName;
                return lastName + ',' + firstName;
            }
             
        }

    }
}