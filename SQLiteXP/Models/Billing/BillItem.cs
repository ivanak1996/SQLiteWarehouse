using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models.Billing
{
    public class BillItem
    {       
        public int billId { get; set; }
        public int id { get; set; }
        public float Quantity { get; set; }

        // product related fields
        public int productIdent { get; set; }
        public float productCena { get; set; }
        public float productPopust { get; set; }
        public string productSifra { get; set; }
        public string productNaziv { get; set; }
        public string productJM { get; set; }
        public float productPdv { get; set; }
        public string productBarkod { get; set; }
    }
}
