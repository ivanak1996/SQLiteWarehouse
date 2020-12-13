using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Products
    {
        public int id { get; set; }
        public int ident { get; set; }
        public string sifra { get; set; }
        public string naziv { get; set; }
        public string grupa { get; set; }
        public string jm { get; set; }
        public string barkod { get; set; }
        public float pdv { get; set; }
        public float cena { get; set; }
        public float popust { get; set; }
        public string opis { get; set; }
    }
}
