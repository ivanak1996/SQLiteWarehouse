using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class Buyers
    {
        public int id { get; set; }
        public int ident { get; set; }
        public string sifra { get; set; }
        public string naziv { get; set; }
        public string adresa { get; set; }
        public string posta { get; set; }
        public string grad { get; set; }
        public string pib { get; set; }
        public string maticniBroj { get; set; }
    }
}
