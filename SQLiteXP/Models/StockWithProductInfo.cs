using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class StockWithProductInfo
    {
        public int id { get; set; }
        public string anRow { get; set; }
        public string sifraSkladista { get; set; }
        public string sifraProizvoda { get; set; }
        public float zaliha { get; set; }
        public string naziv { get; set; }
        public string jm { get; set; }
    }
}
