using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ZipByLimit.Models
{
    public class FolderModel: BindableBase
    {
        private string _FolderName;
        /// <summary>
        /// 文件夹的名称
        /// </summary>
        public string FolderName 
        {
            get { return _FolderName; }
            set { SetProperty(ref _FolderName, value); }
        }
        private string _FolderPath;
        /// <summary>
        /// 文件夹的路径
        /// </summary>
        public string FolderPath 
        {
            get { return _FolderPath; }
            set { SetProperty(ref _FolderPath, value); }
        }
        private string _FolderSize;
        /// <summary>
        /// 文件夹的大小，不包含子级文件夹
        /// </summary>
        public string FolderSize 
        {
            get { return _FolderSize; }
            set { SetProperty(ref _FolderSize,value); }
        }
        private int _FileCount;
        /// <summary>
        /// 文件夹中顶层的文件数量，不包含子级文件夹，也不包含次级文件
        /// </summary>
        public int FileCount
        {
            get { return _FileCount; }
            set { SetProperty(ref _FileCount, value); }
        }

        private int _FileCountTotal;
        public int FileCountTotal
        {
            get { return _FileCountTotal; }
            set { SetProperty(ref _FileCountTotal, value); }
        }

        private long _FolderSizeTotal;
        /// <summary>
        /// 文件夹的大小，包含所有下级文件夹
        /// </summary>
        public long FolderSizeTotal
        {
            get { return _FolderSizeTotal; }
            set { SetProperty(ref _FolderSizeTotal, value); }
        }

        private string _FolderSizeTotalToDisplay;
        /// <summary>
        /// 文件夹的大小，显示文字，包含单位
        /// </summary>
        public string FolderSizeTotalToDisplay
        {
            get { return _FolderSizeTotalToDisplay; }
            set { SetProperty(ref _FolderSizeTotalToDisplay,value); }
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
        private int _SubFolderCount;
        /// <summary>
        /// 所有下级文件夹的数量
        /// </summary>
        public int SubFolderCount
        {
            get { return _SubFolderCount; }
            set { SetProperty(ref _SubFolderCount, value); }
        }
        private double _Progress;
        public double Progress
        {
            get { return _Progress; }
            set { SetProperty(ref _Progress, value); }
        }

        private int _OverSizedFileCount;
        public int OverSizedFileCount
        {
            get { return _OverSizedFileCount; }
            set { SetProperty(ref _OverSizedFileCount, value); }
        }

        public List<FileInfo> OverSizedFileList { get; set; }
        public List<ZipFileModel> ZipFileList { get; set; }

        private ZipFileModel _ZipResultFile;
        /// <summary>
        /// 把多个文件夹压缩成一个文件
        /// </summary>
        public ZipFileModel ZipResultFile
        {
            get { return _ZipResultFile; }
            set { SetProperty(ref _ZipResultFile, value); }
        }

        
        private bool _IsOverSized;
        /// <summary>
        /// 当以文件夹为单位压缩的时候，该文件夹是否超过压缩文件大小限制
        /// </summary>
        public bool IsOverSized
        {
            get { return _IsOverSized; }
            set { SetProperty(ref _IsOverSized, value); }
        }
        //private bool _Removed;
        //public bool Removed
        //{
        //    get { return _Removed; }
        //    set { SetProperty(ref _Removed, value); }
        //}
    }
}
