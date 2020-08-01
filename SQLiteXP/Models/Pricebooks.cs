using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Pricebooks
    {
        public int id { get; set; }
        public string acIdent { get; set; }
        public string acSubject { get; set; }
        public float anSalePrice { get; set; }
        public float anRtPrice { get; set; }
        public float anRebate { get; set; }
        public string adDateStart { get; set; }
        public string adDateEnd { get; set; }
    }
}
