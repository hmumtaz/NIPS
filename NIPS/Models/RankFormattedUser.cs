using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIPS.Models
{
    public partial class RankFormattedUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalPoints { get; set; }
        public int SemesterPoints { get; set; }
        public int WeekPoints { get; set; }
        public int WeekRank { get; set; }
        public int SemesterRank { get; set; }

    }
}