using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    class DocHeader
    {
        public int id { get; set; }
        public string acKey { get; set; }
        public string acDocType { get; set; }
        public string adDate { get; set; }
        public string acReceiver { get; set; }
        public string acPrsn3 { get; set; }
        public string acStatus { get; set; }
        public string acPosted { get; set; }
        public string acName2 { get; set; }
        public string acAddress { get; set; }
        public string acPost { get; set; }
        public string acCity { get; set; }
        public string acCountry { get; set; }
        public int anClerk { get; set; }
        public int anUserIns { get; set; }
        public int anUserChg { get; set; }
        public DateTime adTimeIns { get; set; }
        public DateTime adTimeChg { get; set; }

    }
}
