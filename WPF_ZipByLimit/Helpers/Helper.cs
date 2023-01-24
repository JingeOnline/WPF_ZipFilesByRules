using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_ZipByLimit.Constants;

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
                return size.ToString() + " Byte";
            }
        }

        public static long GetSizeByUnit(int sizeLimitNum, SizeUnit unit)
        {
            switch (unit)
            {
                case SizeUnit.KB:
                    return (long)sizeLimitNum * 1024;
                case SizeUnit.MB:
                    return (long)sizeLimitNum * 1024 * 1024;
                case SizeUnit.GB:
                    return (long)sizeLimitNum * 1024 * 1024 * 1024;
                default:
                    return 0;
            }
        }
    }
}
