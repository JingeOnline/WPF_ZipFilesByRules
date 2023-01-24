using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_ZipByLimit.Helpers;

namespace WPF_ZipByLimit.Models
{
    public class FolderModel : BindableBase
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
            set { SetProperty(ref _FolderSize, value); }
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
            set { SetProperty(ref _FolderSizeTotalToDisplay, value); }
        }


        private string _OutputZipFilesString;
        public string OutputZipFilesString
        {
            get { return _OutputZipFilesString; }
            set { SetProperty(ref _OutputZipFilesString, value); RaisePropertyChanged(nameof(DetailOutputZipMessage)); }
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
            set
            {
                SetProperty(ref _OverSizedFileCount, value);
                RaisePropertyChanged(nameof(DetailAttentionMessage));
                RaisePropertyChanged(nameof(IsDetailAttentionMessageVisiable));
            }
        }

        public List<FileInfo> OverSizedFileList { get; set; }
        public List<ZipFileModel> ZipFileList { get; set; }

        private ZipFileModel _ZipResultFile;
        /// <summary>
        /// 当以文件夹为单位压缩的时候，当前文件夹会被压缩成的Zip文件对象
        /// </summary>
        public ZipFileModel ZipResultFile
        {
            get { return _ZipResultFile; }
            set
            {
                SetProperty(ref _ZipResultFile, value);
            }
        }


        private bool _IsOverSized;
        /// <summary>
        /// 当以文件夹为单位压缩的时候，该文件夹是否超过压缩文件大小限制
        /// </summary>
        public bool IsOverSized
        {
            get { return _IsOverSized; }
            set
            {
                SetProperty(ref _IsOverSized, value);
            }
        }

        /// <summary>
        /// 作为Attention提示信息，显示到DataGrid Detail中
        /// </summary>
        public string DetailAttentionMessage
        {
            get
            {
                if (OverSizedFileList != null && OverSizedFileList.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (FileInfo fileInfo in OverSizedFileList)
                    {
                        string sizeWithUnit = Helper.GetDisplaySizeWithUnit(fileInfo.Length);
                        //stringBuilder.AppendLine($"● {fileInfo.FullName} [Size={sizeWithUnit}] beyond the size limit that you set.");
                        stringBuilder.AppendLine($"● [File Over Size Limit] [Size={sizeWithUnit}] {fileInfo.FullName}");
                    }
                    return stringBuilder.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsDetailAttentionMessageVisiable
        {
            get { return DetailAttentionMessage != null; }
        }

        public string DetailOutputZipMessage
        {
            get
            {
                if (ZipFileList != null && ZipFileList.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (ZipFileModel zipFile in ZipFileList)
                    {
                        string sizeWithUnit = Helper.GetDisplaySizeWithUnit(zipFile.Size);
                        stringBuilder.AppendLine($"● {zipFile.Path} [Size={sizeWithUnit}] [Contains Files={zipFile.ContainedFiles.Count}]");
                        //stringBuilder.AppendLine($"● [Over Size Limit]");
                    }
                    return stringBuilder.ToString();
                }
                else
                {
                    return null;
                }
            }
        }


        //private bool _Removed;
        //public bool Removed
        //{
        //    get { return _Removed; }
        //    set { SetProperty(ref _Removed, value); }
        //}
    }
}
