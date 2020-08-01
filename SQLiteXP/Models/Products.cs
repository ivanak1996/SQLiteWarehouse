using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Products
    {
        public int id { get; set; }
        public string acIdent { get; set; }
        public string acName { get; set; }
        public string acUM { get; set; }
        public float anVat { get; set; }
        public float anSalePrice { get; set; }
        public float anRtPrice { get; set; }
        public string anPluCode { get; set; }
        public string acBarCode { get; set; }
    }
}
