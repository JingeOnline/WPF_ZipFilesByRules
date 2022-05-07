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
using System.Windows.Data;
using System.Diagnostics;

namespace WPF_ZipByLimit.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private FoldersOrFiles _ZipUnit = FoldersOrFiles.ZipFiles;
        public FoldersOrFiles ZipUnit
        {
            get { return _ZipUnit; }
            set { SetProperty(ref _ZipUnit, value); }
        }

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

        private string _UserInputFolderPath;
        public string UserInputFolderPath
        {
            get { return _UserInputFolderPath; }
            set { SetProperty(ref _UserInputFolderPath, value); }
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
        public DelegateCommand AddSourceFolderCommand { get; set; }
        public DelegateCommand SelectTargetFolderCommand { get; set; }
        public DelegateCommand DeleteSelectedFolderCommand { get; set; }
        public DelegateCommand StartZipCommand { get; set; }
        public DelegateCommand<string> MaxSizeTextBoxEnterCommand { get; set; }

        /// <summary>
        /// Used for DataGrid Group sort and filter.
        /// </summary>
        public CollectionViewSource DataGridViewSource { get; set; }

        public MainWindowViewModel()
        {
            SelectSourceFolderCommand = new DelegateCommand(selectSourceFolder, canSelectSourceFolder);
            AddSourceFolderCommand = new DelegateCommand(addSourceFolder, canAddSourceFolder);
            SelectTargetFolderCommand = new DelegateCommand(selectTargetFolder, canSelectTargetFolder);
            DeleteSelectedFolderCommand = new DelegateCommand(deleteSelectedFolder, canDeleteSelectedFolder);
            StartZipCommand = new DelegateCommand(startZip, canStartZip);
            MaxSizeTextBoxEnterCommand = new DelegateCommand<string>(maxSizeTextBoxEnter);


            FolderModelCollection = new ObservableCollection<FolderModel>();
        }

        private bool canAddSourceFolder()
        {
            return true;
        }

        private async void addSourceFolder()
        {
            try
            {
                if (!string.IsNullOrEmpty(UserInputFolderPath))
                {
                    UserInputFolderPath = UserInputFolderPath.Trim();
                    UserInputFolderPath = UserInputFolderPath.Trim('"');
                    string path = Path.GetFullPath(UserInputFolderPath);
                    await loadFolderAsync(path);
                }
            }
            catch
            {
                MessageBox.Show("Cannot find the path, please enter again.");
            }
        }

        private void maxSizeTextBoxEnter(string inputText)
        {
            int number = 0;
            int.TryParse(inputText, out number);
            if (number != 0) MaxZipSize = number;
        }

        private bool canStartZip()
        {
            return true;
        }

        private void startZip()
        {
            startZipFiles();
        }

        private async Task startZipFiles()
        {
            if (ZipUnit == FoldersOrFiles.ZipFiles)
            {
                foreach (FolderModel folderModel in FolderModelCollection)
                {
                    int index = 0;
                    foreach (ZipFileModel zipFileModel in folderModel.ZipFileList)
                    {
                        index++;
                        folderModel.CurrentZipFile = zipFileModel.FileName;
                        folderModel.Progress = (double)index * 100 / folderModel.OutputZipCount;
                        List<string> filesPaths = zipFileModel.ContainedFiles.Select(x => x.FullName).ToList();
                        await createZipAsync(zipFileModel.Path, filesPaths);
                    }
                }
            }
            else
            {
                IEnumerable<IGrouping<ZipFileModel,FolderModel>>  groupResult = FolderModelCollection.GroupBy(x=>x.ZipResultFile);
                foreach(IGrouping<ZipFileModel, FolderModel> group in groupResult)
                {
                    List<string> filePaths = new List<string>();
                    foreach(FolderModel folderModel in group)
                    {
                        filePaths.Add(folderModel.FolderPath);
                    }
                }

            }

            MessageBox.Show("Zip finish.", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool canDeleteSelectedFolder()
        {
            return true;
        }

        private void deleteSelectedFolder()
        {
            FolderModelCollection.Remove(DataGridSelectedFolder);
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

        private async void selectSourceFolder()
        {
            List<string> sourceFolderPathes = new List<string>();
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    sourceFolderPathes = dialog.FileNames.ToList();
                }
            }
            //设置输出文件夹的路径为Source文件夹的父级路径
            if (sourceFolderPathes.Count != 0)
            {
                //TargetFolderPath = Directory.GetParent(sourceFolderPathes[0]).FullName;
                foreach (string path in sourceFolderPathes)
                {
                    await loadFolderAsync(path);
                }
            }
        }

        private async Task loadFolderAsync(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;
            string folderName = Path.GetFileName(folderPath);
            //获取文件夹内的文件数量，不包含下级文件夹
            int filesCount = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly).Length;
            //获取文件夹内的文件数量，包含所有下级文件夹
            int filesCountTotal = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length;
            //获取文件夹的大小，不包含子级文件夹的文件
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Sum(file => file.Length));
            string folderSize = getDisplaySizeWithUnit(dirSize);
            //获取文件夹的大小，包含所有子级文件夹的文件
            long folderSizeTotal = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
            string folderSizeTotalToDisplay = getDisplaySizeWithUnit(folderSizeTotal);
            //所有下级文件夹的数量
            int subFolderCount = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories).Length;

            FolderModel folder = new FolderModel()
            {
                FolderPath = folderPath,
                FolderName = folderName,
                SubFolderCount = subFolderCount,
                FileCount = filesCount,
                FolderSize = folderSize,
                FileCountTotal = filesCountTotal,
                FolderSizeTotal = folderSizeTotal,
                FolderSizeTotalToDisplay = folderSizeTotalToDisplay,
            };
            FolderModelCollection.Add(folder);
            //设置压缩包的输出路径
            TargetFolderPath = Directory.GetParent(FolderModelCollection[0].FolderPath)?.FullName;
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

        private string getDisplaySizeWithUnit(long size)
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

        private long getSizeByUnit(int sizeLimitNum, SizeUnit unit)
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

        private void preCalculate()
        {
            if (FolderModelCollection == null) return;

            //只压缩所选文件夹的顶层文件，忽略包含的子文件夹，忽略子文件夹内的文件
            if (ZipUnit == FoldersOrFiles.ZipFiles)
            {
                //按压缩包大小打包
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

            //以所选文件夹为单位，打包到压缩文件中，包含该文件夹内的所有子文件夹和子文件
            else
            {
                //按压缩包大小打包
                if (SelectedZipRule == ZipRule.BySize)
                {
                    preCalculateZipFoldersBySize(MaxZipSize, SelectedSizeUnit);
                }
                //按包含的文件夹数量打包
                else
                {

                }
            }

        }

        private void preCalculateZipFoldersBySize(int maxZipSize, SizeUnit unit)
        {
            if (maxZipSize == 0) return;
            long sizeLimit = getSizeByUnit(maxZipSize, unit);
            //计算当前压缩包的大小
            long zipFileSize = 0;
            //用来给输出的压缩包命名
            int index = 1;
            //输出的压缩包
            ZipFileModel zipFileModel = getNewZipFileModel(index, sizeLimit);
            //把超过大小的文件夹，IsOverSized = true
            FolderModelCollection.Where(x => x.FolderSizeTotal > sizeLimit).ToList().ForEach(x => x.IsOverSized = true);

            foreach (FolderModel folderModel in FolderModelCollection)
            {
                //如果文件夹过大，或者已经被指定了Zip压缩包，则忽略
                if (folderModel.ZipResultFile != null || folderModel.IsOverSized) continue;

                //尝试从所有文件夹集合中找出能够填满该Zip文件的文件夹
                tryPutFolderIntoZipFile(sizeLimit, FolderModelCollection, zipFileModel);
                index++;
                folderModel.ZipResultFile = getNewZipFileModel(index, sizeLimit);
            }

            foreach (FolderModel folderModel in FolderModelCollection)
            {
                Debug.WriteLine(folderModel.FolderName + ", " + folderModel.ZipResultFile?.FileName);
            }
        }

        //通过迭代的方式，找到能够满足压缩大小限制的所有文件夹，并把这些文件夹的ZipResultFile属性赋值为当前的压缩文件
        private void tryPutFolderIntoZipFile(long leftSpace, IEnumerable<FolderModel> folderModelList, ZipFileModel zipFileModel)
        {
            List<FolderModel> folderUnzipedList = folderModelList.Where(x => x.ZipResultFile == null).OrderByDescending(x => x.FolderSizeTotal).ToList();
            for (int i = 0; i < folderUnzipedList.Count(); i++)
            {
                if (folderUnzipedList[i].FolderSizeTotal < leftSpace)
                {
                    folderUnzipedList[i].ZipResultFile = zipFileModel;
                    Debug.WriteLine("LeftSpace=" + leftSpace + " TryPut - " + folderUnzipedList[i].FolderName + ", " + folderUnzipedList[i].ZipResultFile?.FileName);
                    tryPutFolderIntoZipFile(leftSpace - folderUnzipedList[i].FolderSizeTotal, folderModelList, zipFileModel);
                    return;
                }
            }
        }

        private ZipFileModel getNewZipFileModel(int index, long sizeLimit)
        {
            string zipFileName = Path.GetDirectoryName(TargetFolderPath) + "_" + index.ToString("D2") + ".zip";
            string zipFilePath = Path.Combine(TargetFolderPath, zipFileName);
            ZipFileModel zipFileModel = new ZipFileModel() { FileName = zipFileName, Path = zipFilePath, SizeLimit = sizeLimit };
            return zipFileModel;
        }

        private void preCalculateZipFilesBySize(int maxZipSize, SizeUnit unit, FolderModel folderModel)
        {
            if (maxZipSize == 0) return;
            long sizeLimit = getSizeByUnit(maxZipSize, unit);

            folderModel.OverSizedFileList = new List<FileInfo>();
            folderModel.OverSizedFileCount = 0;
            //获得文件夹中每个文件的信息
            DirectoryInfo dirInfo = new DirectoryInfo(folderModel.FolderPath);
            List<FileInfo> allFilesInTheFolder = dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).ToList();
            //用来给输出的压缩包命名
            int index = 1;
            //输出的压缩包
            List<ZipFileModel> zipFileModelList = new List<ZipFileModel>();
            while (allFilesInTheFolder.Count > 0)
            {
                string zipFileName = folderModel.FolderName + "_" + index.ToString("D2") + ".zip";
                ZipFileModel zipFileModel = new ZipFileModel()
                {
                    SizeLimit = sizeLimit,
                    FileName = zipFileName,
                    Path = Path.Combine(TargetFolderPath, zipFileName)
                };
                folderModel.OverSizedFileList.AddRange(putFilesInZip(ref zipFileModel, ref allFilesInTheFolder));
                zipFileModelList.Add(zipFileModel);
                index++;
            }
            folderModel.ZipFileList = zipFileModelList;
            folderModel.OutputZipCount = zipFileModelList.Count;
            folderModel.OverSizedFileCount = folderModel.OverSizedFileList.Count;
        }

        /// <summary>
        /// 指定压缩文件的大小，把一组文件预装入压缩文件中。
        /// </summary>
        /// <param name="zipFileModel">压缩包模型实例，需指定名称和大小，执行该方法后，会被预装入文件</param>
        /// <param name="filesForZip">文件列表，装入压缩包的文件将被移出</param>
        /// <returns>返回无法装入的文件列表</returns>
        private List<FileInfo> putFilesInZip(ref ZipFileModel zipFileModel, ref List<FileInfo> filesForZip)
        {
            long zippedFileSize = 0;
            List<FileInfo> overSizedFiles = new List<FileInfo>();
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
                //如果单个文件就比压缩包限制还大
                if (fileInfo.Length > zipFileModel.SizeLimit)
                {
                    overSizedFiles.Add(fileInfo);
                    continue;
                }

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
            //移除已经装入的文件
            foreach (FileInfo fileInfo in zipFiles)
            {
                filesForZip.Remove(fileInfo);
            }
            //移除过大无法装入的文件
            foreach (FileInfo fileInfo in overSizedFiles)
            {
                filesForZip.Remove(fileInfo);
            }
            zipFileModel.ContainedFiles = zipFiles;
            return overSizedFiles;
        }

        /// <summary>
        /// 把一组文件打包成zip压缩文件并输出到指定路径
        /// </summary>
        /// <param name="zipFilePath">压缩包的输出路径</param>
        /// <param name="filePathList">待压缩的文件路径列表</param>
        /// <returns></returns>
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
