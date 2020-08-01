using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Users
    {
        public int id { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public int loggedIn { get; set; }
    }
}
