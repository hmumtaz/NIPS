using NIPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;

namespace NIPS.Controllers
{
    [RoutePrefix("api/system")]
    public class SystemController : ApiController
    {
        public void Get()
        {
            try
            {
                using (NipsDBEntities entities = new NipsDBEntities())
                {
                    List<User> users = entities.Users.ToList();
                    users = users.OrderBy(u => u.WeekPoints).ToList();
                    List<User> retList = new List<User>();
                    if (users.Count >= 3)
                    {
                        retList.Add(users[0]);
                        retList.Add(users[1]);
                        retList.Add(users[2]);
                        User edge = users[2];
                        int i = 3;
                        while (users.ElementAt(i).WeekPoints == edge.WeekPoints)
                        {
                            retList.Add(users[i]);
                        }
                    }
                    else
                    {
                        retList = users;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("This week's top brothers are:");
                    foreach (User u in retList)
                    {
                        sb.AppendLine(u.LastName + ',' + u.FirstName);
                    }
                    sb.AppendLine("Please congratulate them!");
                    string body = sb.ToString();
                    MailMessage message = new MailMessage();
                    message.To.Add("hmumtaz996@gmail.com");
                    message.Subject = "This week's top brothers";
                    message.From = new MailAddress("hmumtaz996@gmail.com");
                    message.Body = body;
                    var client = new SmtpClient();
                    client.EnableSsl = true;
                    client.Send(message);
                    foreach (User u in users)
                    {
                        u.WeekPoints = 0;
                    }
                    entities.SaveChanges();
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
