using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    class DocItems
    {
        public int id { get; set; }
        public string acKey { get; set; }
        public int anNo { get; set; }
        public string acIdent { get; set; }
        public string acName { get; set; }
        public float anQty { get; set; }
        public float anPrice { get; set; }
        public float anRebate { get; set; }
        public float anVat { get; set; }
        public string acVatCode { get; set; }
        public string acUM { get; set; }
    }
}
