using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Stock
    {
        public int id { get; set; }
        public string acWarehouse { get; set; }
        public string acIdent { get; set; }
        public float anStock { get; set; }
    }
}
