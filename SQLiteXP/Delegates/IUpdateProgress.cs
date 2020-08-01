using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteXP.Delegates
{
    public interface IUpdateProgress
    {
        void UpdateProgress(int percent);
    }
}
