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
        private string _FolderSize;
        public string FolderSize 
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
        private bool _HasSubFolder;
        public bool HasSubFolder
        {
            get { return _HasSubFolder; }
            set { SetProperty(ref _HasSubFolder, value); }
        }

        public List<ZipFileModel> ZipFileList { get; set; } = new List<ZipFileModel>();
        //private bool _Removed;
        //public bool Removed
        //{
        //    get { return _Removed; }
        //    set { SetProperty(ref _Removed, value); }
        //}
    }
}
