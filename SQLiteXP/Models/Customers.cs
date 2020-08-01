using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Customers
    {
        public int id { get; set; }
        public string acSubject { get; set; }
        public string acName2 { get; set; }
        public string acAddress { get; set; }
        public string acPost { get; set; }
        public string acCity { get; set; }
        public string acCode { get; set; }
        public string acRegNo { get; set; }
        public float anRebate { get; set; }
        public int anDaysForPayment { get; set; }
    }
}
