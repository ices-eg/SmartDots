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
using DevExpress.Utils;
using System.Dynamic;

namespace SmartDots.ViewModel
{
    public class AgeReadingFileViewModel : AgeReadingBaseViewModel
    {
        private ObservableCollection<dynamic> dynamicFiles;
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
        private bool showNavButtons;
        private string sampleNumberAlias;
        private Visibility canAttachDetachSampleVisibility;
        private Visibility toolbarVisibility;

        public AgeReadingFileViewModel()
        {
            ShowNavButtons = Properties.Settings.Default.ShowFileNavButtons;
        }

        public List<File> Files { get; set; }


        public ObservableCollection<dynamic> DynamicFiles
        {
            get { return dynamicFiles; }
            set
            {
                dynamicFiles = value;
                if (dynamicFiles.Any())
                {
                    if (SelectedFile?.ID != dynamicFiles[0]?.ID)
                    {
                        SelectedFile = Files.FirstOrDefault(x => x.ID == dynamicFiles[0].ID);
                        LoadNewFile();
                    }
                    
                }
                RaisePropertyChanged("DynamicFiles");
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
                DynamicFiles?.Clear();
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

        public bool ShowNavButtons
        {
            get { return showNavButtons; }
            set
            {
                showNavButtons = value;
                RaisePropertyChanged("ShowNavButtons");
                RefreshToolbarVisibility();
            }
        }

        public void RefreshToolbarVisibility()
        {
            ToolbarVisibility = Visibility.Collapsed;
            if (canAttachDetachSampleVisibility == Visibility.Visible && (CanDetach || NeedsSampleLink)) ToolbarVisibility = Visibility.Visible;
            if (ShowNavButtons) ToolbarVisibility = Visibility.Visible;
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
            if (AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row != null)
            {
                var rowFile = Files.FirstOrDefault(x => x.ID == ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).ID);
                file = rowFile;
            }
            
            if (file != null )
            {
                var temp = GetFileWithAnnotationsAndSampleProperties(file.ID);
                if (temp != null)
                {
                    file.BoundOutcomes = temp.BoundOutcomes;
                    file.Sample = temp.Sample;
                    file.AnnotationCount = temp.AnnotationCount;
                    file.IsReadOnly = temp.IsReadOnly;
                    file.CanApprove = temp.CanApprove;
                    SelectedFile = file;
                    SelectedFile.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
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
            ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).SampleID = null;
            ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).Sample = null;
            ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).IsReadOnly = true;
            SelectedFile.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);

            UpdateList();
            AgeReadingViewModel.AgeReadingSampleViewModel.Sample = null;
            AgeReadingViewModel.AgeReadingSampleViewModel.SetSample();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
        }

        public void btnNext_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
        }

        public void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingFileView.FileList.MovePrevRow();
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
                    dynamic dynFile = CreateDynamicFile(fileResult);
                    AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row = dynFile;
                    fileResult.FetchProps(AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
                    UpdateList();
                }
            }

            catch (Exception e)
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

                ObservableCollection<dynamic> dynamicFiles = new ObservableCollection<dynamic>();
                List<File> filelist = new List<File>();

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

                List<string> columnNames = new List<string>();
                AgeReadingViewModel.AgeReadingFileView.FileGrid.Dispatcher.Invoke(() =>
                {
                    foreach (string image in fullImageNames)
                    {
                        var dtoFile = filesResult.Result.FirstOrDefault(x => x.Filename.ToLower() == image.ToLower());
                        if (dtoFile == null)
                        {
                            continue;
                        }
                        var file = (File)Helper.ConvertType(dtoFile, typeof(File));

                        if(file.SampleProperties != null)
                        {
                            Dictionary<string, string> values = file.SampleProperties.ToObject<Dictionary<string, string>>();
                            columnNames = values.Keys.ToList();
                        }
                        
                        if (dtoFile.Sample != null) file.Sample = (Sample)Helper.ConvertType(dtoFile.Sample, typeof(Sample));
                        if (file != null)
                        {
                            file.Filename = file.Filename;
                            int index = fullImageNames.IndexOf(image);
                            file.FullFileName = fullImageNames[index];
                            file.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
                        }

                        dynamic dynFile = CreateDynamicFile(file);
                        
                        dynamicFiles.Add(dynFile);
                        filelist.Add(file);
                    }

                    List<GridColumn> colsToDelete = new List<GridColumn>();
                    foreach (var column in AgeReadingViewModel.AgeReadingFileView.FileGrid.Columns.Where(x => x.Tag != null && x.Tag.ToString() == "Dynamic"))
                    {
                        colsToDelete.Add(column);
                    }

                    foreach (var column in colsToDelete)
                    {
                        AgeReadingViewModel.AgeReadingFileView.FileGrid.Columns.Remove(column);
                    }

                    List<GridColumn> columns = new List<GridColumn>();
                    
                    columns.AddRange(columnNames.Select(columnName => new GridColumn() { FieldName = columnName, AllowSorting = DefaultBoolean.True, Tag = "Dynamic", AllowEditing = DefaultBoolean.False }));
                    foreach (var col in columns)
                    {
                        AgeReadingViewModel.AgeReadingFileView.FileGrid.Columns.Add(col);
                    }

                    //if (filelist.Any())
                    //{
                    //    List<GridColumn> columns = new List<GridColumn>();
                    //    columns.AddRange(columnNames.Select(columnName => new GridColumn() { FieldName = columnName, AllowSorting = DefaultBoolean.True, Tag = "Dynamic" }));
                    //    foreach (var column in columns)
                    //    {
                    //        AgeReadingViewModel.AgeReadingFileView.FileGrid.Columns.Add(column);
                    //    }
                    //}


                    Files = filelist;
                    DynamicFiles = dynamicFiles;
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

        public dynamic CreateDynamicFile(File file)
        {
            dynamic dynFile = new ExpandoObject();
            dynFile.ID = file.ID;
            dynFile.Display = file.Display;
            dynFile.Status = file.Status;
            dynFile.SampleNumber = file.SampleNumber;
            dynFile.AnnotationCount = file.AnnotationCount;
            dynFile.Scale = file.Scale;
            dynFile.Sample = new ExpandoObject();
            if (file.Sample != null)
            {
                dynFile.Sample.StatusRank = file.Sample.StatusRank;
                dynFile.Sample.StatusColor = file.Sample.StatusColor;
                dynFile.Sample.StatusCode = file.Sample.StatusCode;
            }

            if (file.SampleProperties != null)
            {
                List<string> columnNames = new List<string>();
                Dictionary<string, string> values = file.SampleProperties.ToObject<Dictionary<string, string>>();
                columnNames = values.Keys.ToList();
                foreach (var column in columnNames)
                {
                    ((IDictionary<String, Object>)dynFile)[column] = values[column];

                }
            }

            return dynFile;
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

        

        private void ToggleFileOptions(bool toggle)
        {
            AgeReadingViewModel.EnableUI(!toggle);
            AgeReadingViewModel.AgeReadingEditorView.IsEnabled = !toggle;
            AgeReadingViewModel.AgeReadingFileView.IsEnabled = true;
            AgeReadingViewModel.AgeReadingFileView.FileGrid.IsEnabled = !toggle;
            AgeReadingViewModel.AgeReadingFileView.AnnotationsOperations.IsEnabled = !toggle;
            if (toggle)
            {
                AgeReadingViewModel.AgeReadingFileView.FileSettingsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AgeReadingViewModel.AgeReadingFileView.FileSettingsPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void FileSettings()
        {
            AgeReadingViewModel.AgeReadingFileView.ShowNavBtns.EditValue = Properties.Settings.Default.ShowFileNavButtons;

            ToggleFileOptions(true);
        }

        public void SaveSettings()
        {
            ToggleFileOptions(false);

            // code for saving the usersetting
            Properties.Settings.Default.ShowFileNavButtons = (bool) AgeReadingViewModel.AgeReadingFileView.ShowNavBtns.EditValue;
            ShowNavButtons = (bool) AgeReadingViewModel.AgeReadingFileView.ShowNavBtns.EditValue;

            Properties.Settings.Default.Save();
            RefreshToolbarVisibility();
        }

        public void CancelSettings()
        {
            ToggleFileOptions(false);
        }
    }
}
