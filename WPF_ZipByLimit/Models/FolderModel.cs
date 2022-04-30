using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ZipByLimit.Models
{
    public class FolderModel: BindableBase
    {
        private string _FolderName;
        public string FolderName 
        {
            get { return _FolderName; }
            set { SetProperty(ref _FolderName, value); }
        }
        private string _FolderPath;
        public string FolderPath 
        {
            get { return _FolderPath; }
            set { SetProperty(ref _FolderPath, value); }
        }
        private long _FolderSize;
        public long FolderSize 
        {
            get { return _FolderSize; }
            set { SetProperty(ref _FolderSize,value); }
        }
        private int _FileCount;
        public int FileCount 
        {
            get { return _FileCount; }
            set { SetProperty(ref _FileCount, value); }
        }
        private int _OutputZipCount;
        public int OutputZipCount 
        {
            get { return _OutputZipCount; }
            set { SetProperty(ref _OutputZipCount, value); }
        }
        private string _CurrentZipFile;
        public string CurrentZipFile 
        {
            get { return _CurrentZipFile; }
            set { SetProperty(ref _CurrentZipFile, value); }
        }
    }
}
