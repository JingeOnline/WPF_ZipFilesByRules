using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;
using WPF_ZipByLimit.Constants;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Collections.ObjectModel;
using WPF_ZipByLimit.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;

namespace WPF_ZipByLimit.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        //private SelectFolderRule _FolderRule;
        //public SelectFolderRule FolderRule
        //{
        //    get { return _FolderRule; }
        //    set { SetProperty(ref _FolderRule, value); }
        //}

        private ZipRule _SelectedZipRule;
        public ZipRule SelectedZipRule
        {
            get { return _SelectedZipRule; }
            set
            {
                SetProperty(ref _SelectedZipRule, value);
                RaisePropertyChanged(nameof(ByAmountSettingVisibility));
                RaisePropertyChanged(nameof(BySizeSettingVisibility));
                preCalculate();
            }
        }
        private SizeUnit _SelectedSizeUnit;
        public SizeUnit SelectedSizeUnit
        {
            get { return _SelectedSizeUnit; }
            set
            {
                SetProperty(ref _SelectedSizeUnit, value);
                preCalculate();
            }
        }

        public Visibility BySizeSettingVisibility
        {
            get
            {
                return SelectedZipRule == ZipRule.BySize ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ByAmountSettingVisibility
        {
            get
            {
                return SelectedZipRule == ZipRule.ByAmount ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private string _SourceFolderPath;
        public string SourceFolderPath
        {
            get { return _SourceFolderPath; }
            set { SetProperty(ref _SourceFolderPath, value); }
        }
        private string _TargetFolderPath;
        public string TargetFolderPath
        {
            get { return _TargetFolderPath; }
            set { SetProperty(ref _TargetFolderPath, value); }
        }

        private ObservableCollection<FolderModel> _FolderModelCollection;
        public ObservableCollection<FolderModel> FolderModelCollection
        {
            get { return _FolderModelCollection; }
            set { SetProperty(ref _FolderModelCollection, value); }
        }

        private int _MaxZipSize;
        public int MaxZipSize
        {
            get { return _MaxZipSize; }
            set
            {
                SetProperty(ref _MaxZipSize, value);
                preCalculate();
            }
        }

        private int _MaxZipFilesContains;
        public int MaxZipFilesContains
        {
            get { return _MaxZipFilesContains; }
            set
            {
                SetProperty(ref _MaxZipFilesContains, value);
                preCalculate();
            }
        }

        private bool _DeleteFilesAfterZip;
        public bool DeleteFilesAfterZip
        {
            get { return _DeleteFilesAfterZip; }
            set { SetProperty(ref _DeleteFilesAfterZip, value); }
        }

        private bool _DeleteFolderAfterZip;
        public bool DeleteFolderAfterZip
        {
            get { return _DeleteFolderAfterZip; }
            set { SetProperty(ref _DeleteFolderAfterZip, value); }
        }

        private FolderModel _DataGridSelectedFolder;
        public FolderModel DataGridSelectedFolder
        {
            get { return _DataGridSelectedFolder; }
            set { SetProperty(ref _DataGridSelectedFolder, value); }
        }

        public DelegateCommand SelectSourceFolderCommand { get; set; }
        public DelegateCommand SelectTargetFolderCommand { get; set; }
        public DelegateCommand LoadFoldersCommand { get; set; }
        public DelegateCommand DeleteSelectedFolderCommand { get; set; }
        public DelegateCommand StartZipCommand { get; set; }
        public DelegateCommand<string> MaxSizeTextBoxEnterCommand { get; set; }


        public MainWindowViewModel()
        {
            SelectSourceFolderCommand = new DelegateCommand(selectSourceFolder, canSelectSourceFolder);
            SelectTargetFolderCommand = new DelegateCommand(selectTargetFolder, canSelectTargetFolder);
            LoadFoldersCommand = new DelegateCommand(loadFolders, canLoadFolders);
            DeleteSelectedFolderCommand = new DelegateCommand(deleteSelectedFolder, canDeleteSelectedFolder);
            StartZipCommand = new DelegateCommand(startZip, canStartZip);
            //Todo:把preCalculate触发改造成Command，用户回车，或者控件lostFocus
            //https://stackoverflow.com/questions/26353893/implementing-textbox-lostfocus-event-in-mvvm
            //https://stackoverflow.com/questions/28941294/how-to-use-lostfocus-as-a-command-in-wpf
            MaxSizeTextBoxEnterCommand = new DelegateCommand<string>(maxSizeTextBoxEnter);
        }

        private void maxSizeTextBoxEnter(string inputText)
        {
            int number=0;
            int.TryParse(inputText, out number);
            if(number!=0) MaxZipSize = number;
            preCalculate();
        }

        private bool canStartZip()
        {
            return true;
        }

        private void startZip()
        {
            //Todo:start zip
        }

        private bool canDeleteSelectedFolder()
        {
            return true;
        }

        private void deleteSelectedFolder()
        {
            FolderModelCollection.Remove(DataGridSelectedFolder);
        }

        private bool canLoadFolders()
        {
            return true;
        }

        private void loadFolders()
        {
            FolderModelCollection = new ObservableCollection<FolderModel>();
            loadFolderAsync(SourceFolderPath);
            #region Removed Code
            //if (FolderRule == SelectFolderRule.SelectedFolder)
            //{
            //    loadFolderAsync(SourceFolderPath);
            //}
            //else
            //{
            //    loadSubFoldersAsync(SourceFolderPath);
            //}
            #endregion
        }

        private bool canSelectTargetFolder()
        {
            return true;
        }

        private void selectTargetFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                //设置打开文件窗口的启动路径
                if (!string.IsNullOrEmpty(TargetFolderPath)) dialog.InitialDirectory = TargetFolderPath;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TargetFolderPath = dialog.FileName;
                }
            }
        }

        private bool canSelectSourceFolder()
        {
            return true;
        }

        private void selectSourceFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SourceFolderPath = dialog.FileName;
                    //设置输出文件夹的路径为Source文件夹的父级路径
                    TargetFolderPath = Directory.GetParent(SourceFolderPath).FullName;
                }
            }
        }

        private async Task loadFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;
            string folderName = Path.GetFileName(folderPath);
            int filesCount = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly).Length;
            bool hasSubFolder = Directory.GetDirectories(folderPath).Length != 0;
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Sum(file => file.Length));
            string folderSize = getSizeWithUnit(dirSize);
            FolderModel folder = new FolderModel()
            {
                FolderPath = folderPath,
                FolderName = folderName,
                FileCount = filesCount,
                HasSubFolder = hasSubFolder,
                FolderSize = folderSize
            };
            FolderModelCollection.Add(folder);
        }

        private async Task loadSubFoldersAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;
            string[] subFolderPathArray = Directory.GetDirectories(folderPath);
            foreach (string subFolderPath in subFolderPathArray)
            {
                await loadFolderAsync(subFolderPath);
            }
        }

        private string getSizeWithUnit(long size)
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

        private void preCalculate()
        {
            if (FolderModelCollection == null) return;
            //按文件大小打包
            if (SelectedZipRule == ZipRule.BySize)
            {
                foreach (FolderModel folderModel in FolderModelCollection)
                {
                    preCalculateZipFilesBySize(MaxZipSize, SelectedSizeUnit, folderModel);
                }
            }
            //按包含的文件数量打包
            else
            {

            }
        }

        private void preCalculateZipFilesBySize(int sizeLimitNum, SizeUnit unit, FolderModel folderModel)
        {
            if (sizeLimitNum == 0) return;
            long sizeLimit = 0; ;
            switch (unit)
            {
                case SizeUnit.MB:
                    sizeLimit = (long)sizeLimitNum * 1024 * 1024;
                    break;
                case SizeUnit.GB:
                    sizeLimit = (long)sizeLimitNum * 1024 * 1024 * 1024;
                    break;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(folderModel.FolderPath);
            List<FileInfo> allFilesInTheFolder = dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).ToList();
            int index = 1;
            List<ZipFileModel> zipFileModelList = new List<ZipFileModel>();
            while (allFilesInTheFolder.Count > 0)
            {
                string fileName = folderModel.FolderName + "_" + index.ToString("D2");
                ZipFileModel zipFileModel = new ZipFileModel()
                {
                    SizeLimit = sizeLimit,
                    FileName = fileName,
                    Path = Path.Combine(TargetFolderPath, fileName)
                };
                putFilesInZip(ref zipFileModel, ref allFilesInTheFolder);
                zipFileModelList.Add(zipFileModel);
                index++;
            }
            folderModel.ZipFileList = zipFileModelList;
            folderModel.OutputZipCount = zipFileModelList.Count;
        }

        private void putFilesInZip(ref ZipFileModel zipFileModel, ref List<FileInfo> filesForZip)
        {
            long zippedFileSize = 0;
            List<FileInfo> zipFiles = new List<FileInfo>();
            #region Removed Method
            //for (int i = 0; i < fileInfos.Count(); i++)
            //{
            //    zippedFileSize += fileInfos[i].Length;
            //    if (zippedFileSize > zipFileModel.SizeLimit)
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        fileList.Add(fileInfos[i]);
            //        //这里不能这样写，因为移除了一个元素，所以数组的元素index就会被重置。
            //        //fileInfos.Remove(fileInfos[i]);
            //    }
            //}
            #endregion
            foreach (FileInfo fileInfo in filesForZip)
            {
                zippedFileSize += fileInfo.Length;
                if (zippedFileSize < zipFileModel.SizeLimit)
                {
                    zipFiles.Add(fileInfo);
                }
                else
                {
                    break;
                }
            }
            foreach (FileInfo fileInfo in zipFiles)
            {
                filesForZip.Remove(fileInfo);
            }
            zipFileModel.Files = zipFiles;
        }

        private async Task createZipAsync(string zipFilePath, List<string> filePathList)
        {
            await Task.Run(() =>
            {

                try
                {
                    //创建并打开zip文件
                    using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var filePath in filePathList)
                        {
                            // 向zip文件中添加文件
                            zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.Optimal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Zip Error Occur", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw ex;
                }

            });
        }
    }
}
