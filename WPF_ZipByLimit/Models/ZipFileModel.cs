using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ZipByLimit.Models
{
    public class ZipFileModel
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public List<FileInfo> Files { get; set; }
        public long SizeLimit { get; set; }
        public int AmountLimit { get; set; }
    }
}
