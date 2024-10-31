using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace SmartDots.ViewModel
{
    public class LarvaeFileViewModel : LarvaeBaseViewModel
    {
        private ObservableCollection<LarvaeFile> larvaeFiles;
        private LarvaeFile selectedFile;
        private bool changingFile;
        private bool showNavButtons;
        private Visibility toolbarVisibility;

        public ObservableCollection<LarvaeFile> LarvaeFiles
        {
            get
            {
                return larvaeFiles;
            }
            set
            {
                larvaeFiles = value;
                SelectedFile = larvaeFiles?.FirstOrDefault();
                RaisePropertyChanged("LarvaeFiles");
            }
        }

        public LarvaeFile SelectedFile
        {
            get { return selectedFile; }
            set
            {
                selectedFile = value;
                RaisePropertyChanged("SelectedFile");
                if (LarvaeViewModel.LarvaeView != null)
                {
                    LarvaeViewModel.LarvaeEditorViewModel.UndoRedo?.EmptyStacks();
                    LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();

                    LoadFile();
                }
            }
        }

        public bool ChangingFile
        {
            get { return changingFile; }
            set { changingFile = value; }
        }

        public void LoadImage(string imagepath)
        {
            var bitmap = new BitmapImage();

            if (imagepath.StartsWith("http"))
            {
                var buffer = new WebClient().DownloadData(imagepath);

                using (var stream = new MemoryStream(buffer))
                {
                    string[] parts = imagepath.Split(new char[] { '\\', '/' });
                    string filename = parts[parts.Length - 1];

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();

                    Directory.CreateDirectory($@"temp\{LarvaeViewModel.LarvaeAnalysis.ID.ToString()}");

                    SaveMemoryStreamToFile(stream, $@"temp\{LarvaeViewModel.LarvaeAnalysis.ID.ToString()}\{filename}");
                }
            }
            else
            {
                bitmap = new BitmapImage(new Uri(imagepath, UriKind.RelativeOrAbsolute))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
            }

            LarvaeViewModel.LarvaeEditorViewModel.LarvaeImage = bitmap;
            LarvaeViewModel.LarvaeEditorViewModel.OriginalImage = BitmapConverter.BitmapImage2Bitmap(bitmap);
            LarvaeViewModel.LarvaeView.Opacity = 1;

            LarvaeViewModel.LarvaeStatusbarViewModel.IsFittingImage = true;
        }

        public void UpdateList()
        {
            LarvaeViewModel.LarvaeFileView.LarvaeFileGrid.RefreshData();
        }

        public void FileList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (LarvaeViewModel.LarvaeFileView.FileList.FocusedRowData.Row != null)
            {
                var file = LarvaeFiles.FirstOrDefault(x => x.ID == ((dynamic)LarvaeViewModel.LarvaeFileView.FileList.FocusedRowData.Row).ID);
                if (file != null && SelectedFile != null && file.ID != SelectedFile.ID)
                {
                    SelectedFile = file;
                }
            }
        }

        public void LoadFile()
        {
            try
            {
                LarvaeViewModel.ShowWaitSplashScreen();
            }
            catch (Exception ex)
            {
            }

            ChangingFile = true;

            LarvaeViewModel.LarvaeEditorViewModel.HideMeasureScalePanel();

            LarvaeFile file = SelectedFile;
            LarvaeViewModel.LarvaeEditorViewModel.Brightness = 0;
            LarvaeViewModel.LarvaeEditorViewModel.Contrast = 0;
            LarvaeViewModel.LarvaeEditorViewModel.RemoveShapes();

            if (file != null)
            {
                UpdateList();

                string[] parts = file.Path.Split(new char[] { '\\', '/' });
                string filename = parts[parts.Length - 1];

                if (System.IO.File.Exists($@"temp\{LarvaeViewModel.LarvaeAnalysis.ID.ToString()}\{filename}"))
                {
                    
                    try
                    {
                        LoadImage($@"temp\{LarvaeViewModel.LarvaeAnalysis.ID.ToString()}\{filename}");
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            LoadImage(file.Path);
                        }
                        catch (Exception ex)
                        {
                            ChangingFile = false;
                            return;
                        }
                    }
                }
                else
                {
                    try
                    {
                        LoadImage(file.Path);
                    }
                    catch (Exception ex)
                    {
                        ChangingFile = false;
                        return;
                    }
                }

            }
            else
            {
                if(LarvaeViewModel.LarvaeEditorViewModel.LarvaeImage != null) LarvaeViewModel.LarvaeEditorViewModel.LarvaeImage = null;
                if (LarvaeViewModel.LarvaeEditorViewModel.OriginalImage != null) LarvaeViewModel.LarvaeEditorViewModel.OriginalImage = null;
            }
            try
            {
                LarvaeViewModel.CloseSplashScreen();
                if (selectedFile != null && selectedFile.Scale != null && selectedFile.Scale != 0.0m)
                {
                    LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): {selectedFile.Scale.ToString()}";
                }
                else if (selectedFile == null)
                {
                    LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"";
                }
                else
                {
                    LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): ?";
                }
                ChangingFile = false;
                RefreshNavigationButtons();
            }
            catch (Exception ex)
            {
            }
        }

        public void RefreshNavigationButtons()
        {
            var index = LarvaeViewModel.LarvaeFileView.FileList.FocusedRowHandle;
            if (index <= 0)
            {
                LarvaeViewModel.LarvaeView.FilePrevious.IsEnabled = false;
            }
            else
            {
                LarvaeViewModel.LarvaeView.FilePrevious.IsEnabled = true;
            }

            if (LarvaeFiles == null || index >= LarvaeFiles.Count - 1)
            {
                LarvaeViewModel.LarvaeView.FileNext.IsEnabled = false;
            }
            else
            {
                LarvaeViewModel.LarvaeView.FileNext.IsEnabled = true;
            }
        }
        public void SaveMemoryStreamToFile(MemoryStream stream, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                stream.WriteTo(fileStream);
            }
        }
    }
}
