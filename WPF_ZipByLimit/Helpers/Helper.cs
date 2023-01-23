using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ZipByLimit.Helpers
{
    public class Helper
    {
        public static string GetDisplaySizeWithUnit(long size)
        {
            double KB = 1024;
            double MB = KB * 1024;
            double GB = MB * 1024;
            if (size > GB)
            {
                double result = size / GB;
                return result.ToString("0.0") + " GB";
            }
            else if (size > MB)
            {
                double result = size / MB;
                return result.ToString("0.0") + " MB";
            }
            else if (size > KB)
            {
                double result = size / KB;
                return result.ToString("0.0") + " KB";
            }
            else
            {
                return size.ToString() + " BT";
            }
        }
    }
}
