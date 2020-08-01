using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Settings
    {
        public int id { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string acDocType { get; set; }
        public string acWarehouse { get; set; }
    }
}
