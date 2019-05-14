using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.Model.Extension;
using SmartDots.View;
using File = SmartDots.Model.File;
using MessageBox = System.Windows.MessageBox;
using System.Net;
using System.Windows.Controls;
using Line = System.Windows.Shapes.Line;

namespace SmartDots.ViewModel
{
    public class AgeReadingFileViewModel : AgeReadingBaseViewModel
    {
        private ObservableCollection<File> files;
        private File selectedFile;
        private Folder currentFolder;
        private bool changingFile;
        private bool needsSampleLink;
        private bool canDetach;
        private string nextFileLocation;
        private string nextLocalFileLocation;
        private bool loadingNextPicture;
        private bool useSampleStatus;
        private bool loadingfolder;
        private string sampleNumberAlias;
        private Visibility canAttachDetachSampleVisibility;
        private Visibility toolbarVisibility;


        public ObservableCollection<File> Files
        {
            get { return files; }
            set
            {
                files = value;
                if (files.Any())
                {
                    if (SelectedFile?.ID != files[0]?.ID)
                    {
                        SelectedFile = files[0];
                        LoadNewFile();
                    }
                    
                }
                RaisePropertyChanged("Files");
            }
        }

        public File SelectedFile
        {
            get { return selectedFile; }
            set
            {
                ChangingFile = true;
                selectedFile = value;
                RaisePropertyChanged("SelectedFile");
                if (AgeReadingViewModel.AgeReadingView != null)
                {
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes = selectedFile.BoundOutcomes ??
                                                                                 new ObservableCollection<Annotation>();
                    AgeReadingViewModel.AgeReadingEditorViewModel.OriginalMeasureShapes = new ObservableCollection<Line>();
                    AgeReadingViewModel.AgeReadingEditorViewModel.TextShapes = new ObservableCollection<TextBlock>();
                    AgeReadingViewModel.AgeReadingView.BrightnessSlider.EditValue = 0;
                    AgeReadingViewModel.AgeReadingEditorViewModel.Brightness = 0;
                    AgeReadingViewModel.AgeReadingView.ContrastSlider.EditValue = 0;
                    AgeReadingViewModel.AgeReadingEditorViewModel.Contrast = 0;
                    AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = null;
                }
                //experimental
                if (SelectedFileLocation == nextFileLocation)
                {
                    while (loadingNextPicture)
                    {
                        //wait until download is finished, this empty while loop results in a massive performance boost
                    }
                    try
                    {
                        LoadImage(nextLocalFileLocation);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            LoadImage(SelectedFileLocation);
                        }
                        catch (Exception ex)
                        {
                            changingFile = false;
                            return;
                        }
                    }
                }
                else
                {
                    try
                    {
                        LoadImage(SelectedFileLocation);
                    }
                    catch (Exception e) //ComException, this happens when network connection is lost while reading
                    {
                        changingFile = false;
                        return;
                    }
                }

                ChangingFile = false;


                AgeReadingViewModel.AgeReadingEditorViewModel.Mode = EditorModeEnum.DrawLine;
                AgeReadingViewModel.AgeReadingSampleViewModel.SetSample();
                AgeReadingViewModel.UpdateGraphs();
                NeedsSampleLink = SelectedFile != null && SelectedFile?.SampleID == null;
                CanDetach = SelectedFile?.SampleID != null && !AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any();


            }
        }

        public string SelectedFileLocation
        {
            get
            {
                if (currentFolder.Path.StartsWith("\\"))
                    return Path.Combine(CurrentFolder.Path, SelectedFile.FullFileName);
                else
                {
                    if (CurrentFolder.Path.EndsWith("/")) return CurrentFolder.Path + SelectedFile.FullFileName;
                    else
                    {
                        return CurrentFolder.Path + "/" + SelectedFile.FullFileName;
                    }
                }
            }
        }

        public Folder CurrentFolder
        {
            get { return currentFolder; }
            set
            {
                LoadingFolder = true;
                Files?.Clear();
                currentFolder = value;
                Helper.DoAsync(LoadImages);
                AgeReadingViewModel.AgeReadingView.Opacity = 1;
                RaisePropertyChanged("CurrentFolder");
            }
        }

        public bool NeedsSampleLink
        {
            get { return needsSampleLink; }
            set
            {
                needsSampleLink = value;
                RaisePropertyChanged("NeedsSampleLink");
                RaisePropertyChanged("CanAttachVisibility");
            }
        }

        public bool CanDetach
        {
            get { return canDetach; }
            set
            {
                canDetach = value;
                RaisePropertyChanged("CanDetach");
                RaisePropertyChanged("CanDetachVisibility");
            }
        }

        public bool ChangingFile
        {
            get { return changingFile; }
            set { changingFile = value; }
        }

        public bool LoadingFolder { get { return loadingfolder; } set { loadingfolder = value; } }

        public Visibility CanAttachDetachSampleVisibility
        {
            get { return canAttachDetachSampleVisibility; }
            set
            {
                canAttachDetachSampleVisibility = value;
                if (canAttachDetachSampleVisibility != Visibility.Visible) ToolbarVisibility = Visibility.Collapsed;
                else
                {
                    ToolbarVisibility = Visibility.Visible;
                }
                RaisePropertyChanged("CanAttachDetachSampleVisibility");
            }
        }

        public Visibility CanAttachVisibility
        {
            get
            {
                if (CanAttachDetachSampleVisibility == Visibility.Visible && NeedsSampleLink)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility CanDetachVisibility
        {
            get
            {
                if (CanAttachDetachSampleVisibility == Visibility.Visible && CanDetach)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility ToolbarVisibility
        {
            get { return toolbarVisibility; }
            set
            {
                toolbarVisibility = value;
                RaisePropertyChanged("ToolbarVisibility");
            }
        }

        public bool UseSampleStatus
        {
            get { return useSampleStatus; }
            set
            {
                useSampleStatus = value;
                RaisePropertyChanged("UseSampleStatus");
            }
        }

        public string NextFileLocation
        {
            get { return nextFileLocation; }
            set { nextFileLocation = value; }
        }

        public string SampleNumberAlias
        {
            get { return sampleNumberAlias; }
            set
            {
                sampleNumberAlias = value;
                RaisePropertyChanged("SampleNumberAlias");
            }
        }

        public void LoadImage(string imagepath)
        {
            var bitmap = new BitmapImage();


            if (imagepath.StartsWith("http"))
            {
                var buffer = new WebClient().DownloadData(imagepath);

                using (var stream = new MemoryStream(buffer))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
            }
            else
            {
                bitmap = new BitmapImage(new Uri(imagepath, UriKind.RelativeOrAbsolute))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
            }
            AgeReadingViewModel.AgeReadingEditorViewModel.OtolithImage = bitmap;
            AgeReadingViewModel.AgeReadingEditorViewModel.OriginalImage = bitmap;
            AgeReadingViewModel.AgeReadingView.Opacity = 1;
            AgeReadingViewModel.AgeReadingView.WinFormBrightness.Visibility = Visibility.Visible;
            AgeReadingViewModel.AgeReadingView.WinFormRedness.Visibility = Visibility.Visible;
            AgeReadingViewModel.AgeReadingView.WinFormGrowth.Visibility = Visibility.Visible;
        }

        public void SetNextPicture()
        {
            try
            {
                Directory.CreateDirectory("temp");
                DirectoryInfo di = new DirectoryInfo("temp");
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        System.IO.File.Delete("temp/" + file.Name);
                    }
                    catch (Exception e)
                    {
                        //current file will be locked
                    }
                }
                //Load the image in a seperate thread
                var filename = Files.SkipWhile(item => item != SelectedFile).Skip(1).FirstOrDefault()?.Filename;
                var extension =
                    Path.GetExtension(
                        Files.SkipWhile(item => item != SelectedFile).Skip(1).FirstOrDefault()?.FullFileName);
                var path = "";

                if (currentFolder.Path.StartsWith("\\")) path = Path.Combine(CurrentFolder.Path, filename + extension);
                else
                {
                    if (CurrentFolder.Path.EndsWith("/")) path = CurrentFolder.Path + SelectedFile.FullFileName;
                    else
                    {
                        path = CurrentFolder.Path + "/" + SelectedFile.FullFileName;
                    }
                }
                nextFileLocation = path;
                nextLocalFileLocation = "temp/" + filename + extension;
                if (!System.IO.File.Exists(nextLocalFileLocation)) CopyFileAsync(path, nextLocalFileLocation);
            }
            catch (Exception e)
            {
                // ignored
                loadingNextPicture = false;
            }
        }

        public bool FolderExists(string path)
        {
            if (path.StartsWith("http"))
            {
                //todo have to implement
                return true;
            }
            else
            {
                return Directory.Exists(path);
            }
        }

        public async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            loadingNextPicture = true;
            if (sourceFile.StartsWith("http"))
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(sourceFile, destinationFile);
                    }
                    catch (Exception)
                    {
                        //
                    }
                    
                }
            }
            else
            {
                using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                    await sourceStream.CopyToAsync(destinationStream);
            }
            
            loadingNextPicture = false;
        }

        public void UpdateList()
        {
            AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
            NeedsSampleLink = SelectedFile != null && SelectedFile?.SampleID == null;
            CanDetach = SelectedFile?.SampleID != null &&
                        !AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any();
        }

        public void Attach()
        {
            try
            {
                var analysisSamples = WebAPI.GetAnalysisSamples(AgeReadingViewModel.Analysis.ID).Result;

                AgeReadingViewModel.AttachSampleDialog = new AttachSampleDialog(AgeReadingViewModel);
                AgeReadingViewModel.AttachSampleDialogViewModel =
                    AgeReadingViewModel.AttachSampleDialog.AttachSampleDialogViewModel; //todo problem
                AgeReadingViewModel.AttachSampleDialogViewModel.Samples = analysisSamples;
                ShowDialog(AgeReadingViewModel.AttachSampleDialog);
                AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
                AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();

            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error mapping SampleStates", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void FileList_BeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any() &&
                !AgeReadingViewModel.AgeReadingFileViewModel.LoadingFolder &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation?.QualityID == null &&
                !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
                )
            {
                //savechecks
                if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.EditAnnotation())
                    //AgeReadingViewModel.SaveAnnotations();
                    return;
            }

            else
            {
                AgeReadingViewModel.SaveAnnotations();
            }
        }

        public void FileList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if(!LoadingFolder) LoadNewFile();
            LoadingFolder = false;
        }

        public void LoadNewFile()
        {
            try
            {
                AgeReadingViewModel.ShowWaitSplashScreen();
            }
            catch (Exception ex)
            {
            }

            File file = SelectedFile;
            if (AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row != null) file = AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row as File;
            
            if (file != null )
            {
                var temp = GetFileWithAnnotationsAndSampleProperties(file.ID);
                if (temp != null)
                {
                    file.BoundOutcomes = temp.BoundOutcomes;
                    file.Sample = temp.Sample;
                    file.AnnotationCount = temp.AnnotationCount;
                    file.IsReadOnly = temp.IsReadOnly;
                    SelectedFile = file;
                    SelectedFile.FetchProps();
                    UpdateList();
                }

                Helper.DoAsync(SetNextPicture);
                AgeReadingViewModel.AgeReadingStatusbarViewModel.IsFittingImage = true;
            }

            try
            {
                AgeReadingViewModel.CloseSplashScreen();
            }
            catch (Exception ex)
            {
            }
        }

        public void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            Attach();
        }

        public void btnDetach_Click(object sender, RoutedEventArgs e)
        {
            Guid fileid = SelectedFile.ID;

            var file = WebAPI.GetFile(fileid, false, false);
            if (!file.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading File from Web API\n" + file.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            file.Result.SampleID = null;
            file.Result.Sample = null;
            file.Result.SampleNumber = null;
            var updateFileResult = WebAPI.UpdateFile(file.Result);
            if (!updateFileResult.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error saving File to Web API\n" + updateFileResult.ErrorMessage , "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            SelectedFile.SampleID = null;
            SelectedFile.Sample = null;
            SelectedFile.IsReadOnly = true;
            SelectedFile.FetchProps();

            UpdateList();
            AgeReadingViewModel.AgeReadingSampleViewModel.Sample = null;
            AgeReadingViewModel.AgeReadingSampleViewModel.SetSample();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
        }

        

        public File GetFileWithAnnotationsAndSampleProperties(Guid fileid)
        {
            var file = WebAPI.GetFile(fileid, true, true);
            if (!file.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading File from Web API\n" + file.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
            var fileResult = (File) Helper.ConvertType(file.Result, typeof(File));
            fileResult.ConvertDbAnnotations(file.Result.Annotations.ToList());
            try
            {
                if (file.Result.Sample != null)
                {
                    fileResult.Sample = (Sample)Helper.ConvertType(file.Result.Sample, typeof(Sample));

                }
            }
            catch (Exception)
            {
            }
            return fileResult;
        }

        public void LoadImages()
        {
            try
            {
                AgeReadingViewModel.ShowWaitSplashScreen();
                //todo check if real image

                ObservableCollection<File> filelist = new ObservableCollection<File>();

                List<string> fullImageNames = new List<string>();

                if (WebAPI.Settings.ScanFolder)
                {
                    if (CurrentFolder.Path.StartsWith("http"))
                    {
                        WebClient w = new WebClient();
                        w.Headers["user-agent"] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)";
                        string s = w.DownloadString(CurrentFolder.Path);

                        // 2.
                        foreach (LinkItem i in LinkFinder.Find(s))
                        {
                            fullImageNames.Add(i.Href.Split('/').Last());
                        }
                    }
                    else
                    {
                        fullImageNames = Directory.EnumerateFiles(CurrentFolder.Path).ToList();

                    }
                    fullImageNames = fullImageNames.Where(file => file.ToLower().EndsWith(".tif") || file.ToLower().EndsWith(".jpg")
                                           || file.ToLower().EndsWith(".jpeg") || file.ToLower().EndsWith(".png") ||
                                           file.ToLower().EndsWith(".gif") || file.ToLower().EndsWith(".bmp")).ToList();
                    List<string> tempImageNames = new List<string>();
                    foreach (var image in fullImageNames)
                    {
                        tempImageNames.Add(image.Split('\\').Last());
                    }
                    fullImageNames = tempImageNames;
                }
                
                
                var filesResult = WebAPI.GetFiles(AgeReadingViewModel.Analysis.ID, fullImageNames);
                if (!filesResult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Files from Web API\n" + filesResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!WebAPI.Settings.ScanFolder)
                {
                    fullImageNames = ((List<DtoFile>) filesResult.Result).Select(x => x.Filename).ToList();
                }
                AgeReadingViewModel.AgeReadingFileView.FileGrid.Dispatcher.Invoke(() =>
                {
                    foreach (string image in fullImageNames)
                    {
                        var dtoFile = filesResult.Result.FirstOrDefault(x => x.Filename.ToLower() == image.ToLower());
                        if (dtoFile == null)
                        {
                            //todo
                        }
                        var file = (File)Helper.ConvertType(dtoFile, typeof(File));
                        if (dtoFile.Sample != null) file.Sample = (Sample)Helper.ConvertType(dtoFile.Sample, typeof(Sample));
                        if (file != null)
                        {
                            file.Filename = file.Filename;
                            int index = fullImageNames.IndexOf(image);
                            file.FullFileName = fullImageNames[index];
                            file.FetchProps();
                        }
                        filelist.Add(file);
                    }
                    Files = filelist;
                });
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error loading images\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error,e);
            }
            finally
            {
                AgeReadingViewModel.CloseSplashScreen();
                AgeReadingViewModel.FirstLoad = false;
                LoadingFolder = false;
            }
        }

        public void OnCustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "Status")
            {
                var current = e.Value1 as StatusIcon;
                var other = e.Value2 as StatusIcon;
                int index1 = current.Rank;
                int index2 = other.Rank;
                e.Result = index1.CompareTo(index2);
                e.Handled = true;
            }
        }

        public void Refresh()
        {
            CanDetach = SelectedFile?.SampleID != null && !AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any();
        }
    }
}
