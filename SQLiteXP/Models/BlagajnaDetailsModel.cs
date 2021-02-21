using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Models
{
    public class BlagajnaDetailsModel
    {
        public float cek { get; set; }
        public float gotovina { get; set; }
        public float virman { get; set; }
        public float kartica { get; set; }
        public float uplaceno { get; set; }

        public float cekNefiskalizovan { get; set; }
        public float gotovinaNefiskalizovan { get; set; }
        public float virmanNefiskalizovan { get; set; }
        public float karticaNefiskalizovan { get; set; }
        public float uplacenoNefiskalizovan { get; set; }
    }
}
