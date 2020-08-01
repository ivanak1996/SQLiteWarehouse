using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLiteXP.Delegates
{
    class SaveProductsUpdateProgress : IUpdateProgress
    {
        public ProgressBar ProgressBar { get; set; }
        public void UpdateProgress(int percent)
        {
            ProgressBar.Value = percent;
        }
    }
}
