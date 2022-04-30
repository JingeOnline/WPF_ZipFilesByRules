using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ZipByLimit.Models
{
    public class ConstantsAndEnums
    {

    }

    public enum ZipRule
    {
        [Description("By Size")]
        BySize,
        [Description("By Amount")]
        ByAmount
    }

    public enum SizeUnit
    {
        MB,
        GB
    }
}
