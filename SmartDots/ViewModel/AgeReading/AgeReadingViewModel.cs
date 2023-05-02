using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Model;
using SmartDots.Helpers;
using SmartDots.View;
using SmartDots.ViewModel.AgeReading;

namespace SmartDots.ViewModel
{
    public class AgeReadingViewModel : BaseViewModel
    {
        private bool brightnessGraphVisibile = false;
        private bool rednessGraphVisibile = false;
        private bool growthGraphVisibile = false;
        private bool waitState;
        private bool isListening;
        private bool firstLoad = true;
        private string headerInfo;
        private string sampleAlias;

        private Analysis analysis;

        private ICommand closeToolsetPanelCommand;
        private ICommand closeBrightnessPanelCommand;
        private ICommand closeRednessPanelCommand;
        private ICommand closeGrowthPanelCommand;

        public bool BrightnessGraphVisibile
        {
            get { return brightnessGraphVisibile; }
            set
            {
                brightnessGraphVisibile = value;
                AgeReadingView.BrightnessPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged("BrightnessGraphVisibile");
            }
        }

        public bool RednessGraphVisibile
        {
            get { return rednessGraphVisibile; }
            set
            {
                rednessGraphVisibile = value;
                AgeReadingView.RednessPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged("RednessGraphVisibile");
            }
        }

        public bool GrowthGraphVisibile
        {
            get { return growthGraphVisibile; }
            set
            {
                growthGraphVisibile = value;
                AgeReadingView.GrowthPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged("GrowthGraphVisibile");
            }
        }

        public ICommand CloseBrightnessPanelCommand
        {
            get
            {
                if (closeBrightnessPanelCommand == null)
                {
                    closeBrightnessPanelCommand = new Command(p => true, p => BrightnessGraphVisibile = false);
                }
                return closeBrightnessPanelCommand;
            }
        }

        public ICommand CloseRednessPanelCommand
        {
            get
            {
                if (closeRednessPanelCommand == null)
                {
                    closeRednessPanelCommand = new Command(p => true, p => RednessGraphVisibile = false);
                }
                return closeRednessPanelCommand;
            }
        }

        public ICommand CloseGrowthPanelCommand
        {
            get
            {
                if (closeGrowthPanelCommand == null)
                {
                    closeGrowthPanelCommand = new Command(p => true, p => GrowthGraphVisibile = false);
                }
                return closeGrowthPanelCommand;
            }
        }

        public string HeaderInfo
        {
            get { return headerInfo; }
            set
            {
                headerInfo = value;
                RaisePropertyChanged("HeaderInfo");
            }
        }

        public Analysis Analysis
        {
            get { return analysis; }
            set
            {
                analysis = value;
                //get parameters

                AgeReadingAnnotationViewModel.Parameters = analysis.AnalysisParameters;
                AgeReadingAnnotationViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                AgeReadingAnnotationViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                EditAnnotationDialogViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                EditAnnotationDialogViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                EditAnnotationDialogViewModel.Parameters = analysis.AnalysisParameters;
                AgeReadingView.MainWindowViewModel.HeaderInfo = $"  {Analysis.HeaderInfo}";
                AgeReadingView.MainWindowViewModel.HeaderModule = "  AGEREADING";
                RaisePropertyChanged("Analysis");
            }
        }

        public string SampleAlias
        {
            get { return sampleAlias; }
            set
            {
                sampleAlias = value;
                RaisePropertyChanged("SampleAlias");
            }
        }

        public bool WaitState
        {
            get { return waitState; }
            set { waitState = value; }
        }

        public bool FirstLoad
        {
            get { return firstLoad; }
            set { firstLoad = value; }
        }

        public string ImageSettings { get; set; } = "../../Resources/ok-32.png"; // Image that is used in the PanelControl Header

        public AgeReadingStatusbarView AgeReadingStatusbarView { get; set; }
        public AgeReadingStatusbarViewModel AgeReadingStatusbarViewModel { get; set; }
        public AgeReadingAnnotationView AgeReadingAnnotationView { get; set; }
        public AgeReadingAnnotationViewModel AgeReadingAnnotationViewModel { get; set; }
        public AgeReadingFileView AgeReadingFileView { get; set; }
        public AgeReadingFileViewModel AgeReadingFileViewModel { get; set; }
        public AgeReadingSampleView AgeReadingSampleView { get; set; }
        public AgeReadingSampleViewModel AgeReadingSampleViewModel { get; set; }
        public AgeReadingEditorView AgeReadingEditorView { get; set; }
        public AgeReadingEditorViewModel AgeReadingEditorViewModel { get; set; }
        public AgeReadingView AgeReadingView { get; set; }
        public AttachSampleDialog AttachSampleDialog { get; set; }
        public AttachSampleDialogViewModel AttachSampleDialogViewModel { get; set; }
        public EditAnnotationDialog EditAnnotationDialog { get; set; }
        public EditAnnotationDialogViewModel EditAnnotationDialogViewModel { get; set; }
        public SelectAnalysisDialog SelectAnalysisDialog { get; set; }
        public SelectAnalysisDialogViewModel SelectAnalysisDialogViewModel { get; set; }

        public AgeReadingViewModel()
        {
            CloseSplashScreen();

            AgeReadingStatusbarView = new AgeReadingStatusbarView(this);
            AgeReadingStatusbarViewModel = AgeReadingStatusbarView.AgeReadingStatusbarViewModel;

            AgeReadingAnnotationView = new AgeReadingAnnotationView(this);
            AgeReadingAnnotationViewModel = AgeReadingAnnotationView.AgeReadingAnnotationViewModel;

            AgeReadingFileView = new AgeReadingFileView(this);
            AgeReadingFileViewModel = AgeReadingFileView.AgeReadingFileViewModel;

            AgeReadingSampleView = new AgeReadingSampleView(this);
            AgeReadingSampleViewModel = AgeReadingSampleView.AgeReadingSampleViewModel;

            AgeReadingEditorView = new AgeReadingEditorView(this);
            AgeReadingEditorViewModel = AgeReadingEditorView.AgeReadingEditorViewModel;

            AttachSampleDialog = new AttachSampleDialog(this);
            AttachSampleDialogViewModel = AttachSampleDialog.AttachSampleDialogViewModel;

            EditAnnotationDialog = new EditAnnotationDialog(this);
            EditAnnotationDialogViewModel = EditAnnotationDialog.EditAnnotationDialogViewModel;


            //if (App.Args != null) Connect(App.Args[0]);

            //tier needs to be higher than 0 for hardware acceleration
            //int renderingTier = (RenderCapability.Tier >> 16);
            //Helper.ShowWinUIMessageBox("Render Tier", "Render Tier: " + renderingTier, MessageBoxButton.OK, MessageBoxImage.Information);

            //WebAPI.OnError += ReportError; //causes problems due to threading (event gets triggered on first two steps, somehow)
        }

        private void ReportError(object sender, WebApiEventArgs e)
        {
            Helper.ShowWinUIMessageBox(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e.Error);
        }

        public bool LoadQualities()
        {
            if (AgeReadingAnnotationViewModel.Qualities != null &&
                AgeReadingAnnotationViewModel.Qualities.Any()) return true;

            var dtoQualities = Global.API.GetQualities();
            if (!dtoQualities.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Qualities from the Web API", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            var qualities = new List<Quality>();
            foreach (var dtoQuality in dtoQualities.Result)
            {
                qualities.Add((Quality)Helper.ConvertType(dtoQuality, typeof(Quality)));
            }
            AgeReadingAnnotationViewModel.MapAQColors(qualities);
            return true;
        }



        public bool LoadAnalysis(Guid analysisid)
        {
            try
            {
                Helper.MultiUserColorsDict.Clear();

                if (!LoadQualities())
                {
                    Helper.ShowWinUIMessageBox("Unable to load qualities", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                var dtoAnalysis = Global.API.GetAnalysis(analysisid);
                if (!dtoAnalysis.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading analysis from the Web API\n" + dtoAnalysis.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var analysis = (Analysis)Helper.ConvertType(dtoAnalysis.Result, typeof(Analysis));
                if (analysis.Folder == null)
                {
                    Helper.ShowWinUIMessageBox("Analysis does not contain a folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                List<AnalysisParameter> analysisParameters = new List<AnalysisParameter>();
                foreach (var dtoAnalysisParameter in dtoAnalysis.Result.AnalysisParameters)
                {
                    analysisParameters.Add((AnalysisParameter)Helper.ConvertType(dtoAnalysisParameter, typeof(AnalysisParameter)));
                }
                analysis.AnalysisParameters = analysisParameters;
                Analysis = analysis;
                if(!Helper.FolderExists(Analysis.Folder.Path)) throw new Exception("Could not find Folder " + Analysis.Folder.Path);
                AgeReadingFileViewModel.CurrentFolder = Analysis.Folder;
                return true;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
                return false;
            }
        }


        public bool ValidateAnnotation()
        {
            if (AgeReadingAnnotationViewModel.WorkingAnnotation == null) return true;
            //check if AQ is inserted
            if (AgeReadingAnnotationViewModel.WorkingAnnotation?.QualityID == null)
            {
                Helper.ShowWinUIMessageBox("Please give the annotation a quality", "Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }
            //check if only 1 line is inserted
            if (AgeReadingAnnotationViewModel.WorkingAnnotation?.CombinedLines.Count != 1)
            {
                Helper.ShowWinUIMessageBox("Please make sure the annotation has 1 line", "Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }
            return true;
        }

        public void SaveAnnotations()
        {
            try
            {
                List<Annotation> outcomes = AgeReadingAnnotationViewModel.Outcomes.Where(x => x.IsChanged).ToList();
                if (!outcomes.Any()) return;
                WaitState = true;

                List<DtoAnnotation> dboutcomes = new List<DtoAnnotation>();
                foreach (var outcome in outcomes)
                {
                    var lines = new List<DtoLine>();
                    if (outcome.CombinedLines.Any())
                    {
                        foreach (var line in outcome.CombinedLines[0].Lines)
                        {
                            lines.Add((DtoLine)Helper.ConvertType(line, typeof(DtoLine)));
                        }
                    }

                    var dots = new List<DtoDot>();
                    if (outcome.CombinedLines.Any() && !outcome.HasAq3())
                    {
                        foreach (var dot in outcome.CombinedLines[0].Dots)
                        {
                            dots.Add((DtoDot)Helper.ConvertType(dot, typeof(DtoDot)));
                        }
                    }


                    DtoAnnotation dboutcome = (DtoAnnotation)Helper.ConvertType(outcome, typeof(DtoAnnotation));
                    dboutcome.Dots = dots;
                    dboutcome.Lines = lines;
                    dboutcome.Nucleus = outcome.Nucleus;
                    dboutcome.Edge = outcome.Edge;
                    dboutcomes.Add(dboutcome);
                }
                var updateAnnotationsResult = Global.API.UpdateAnnotations(dboutcomes);
                if (!updateAnnotationsResult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error Saving Annotations\n" + updateAnnotationsResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AgeReadingEditorViewModel.ShapeChangeFlag = false;

                var dbfile = Global.API.GetFile(AgeReadingFileViewModel.SelectedFile.ID, false, true);
                if (!dbfile.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading File from Web API\n" + dbfile.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var file = (File)Helper.ConvertType(dbfile.Result, typeof(File));
                file.Sample = (Sample)Helper.ConvertType(dbfile.Result.Sample, typeof(Sample));
                var selectedFile = AgeReadingFileViewModel.Files.FirstOrDefault(x => x == AgeReadingFileViewModel.SelectedFile);
                if (selectedFile != null)
                {
                    selectedFile.Sample = file.Sample;
                    //selectedFile.AnnotationCount = file.AnnotationCount; this is not needed here
                    selectedFile.IsReadOnly = file.IsReadOnly;
                    selectedFile.Scale = file.Scale;
                    selectedFile.FetchProps((dynamic)AgeReadingFileView.FileList.FocusedRowData.Row);
                    var dynFile = AgeReadingFileViewModel.CreateDynamicFile(selectedFile);
                    AgeReadingFileView.FileList.FocusedRowData.Row = dynFile;
                }
                
                AgeReadingAnnotationViewModel.UpdateList();
                AgeReadingFileViewModel.UpdateList();

                foreach (var outcome in outcomes)
                {
                    outcome.IsChanged = false;
                }
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving Annotations", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
            finally
            {
                WaitState = false;
            }
        }

        public void MakeGraph()
        {
            AgeReadingView.BrightnessGraph.Plan = 1;
            AgeReadingView.BrightnessGraph.control = AgeReadingView;
            AgeReadingView.BrightnessGraph.Create();
            AgeReadingView.BrightnessGraph.MakeGraph();

            AgeReadingView.RednessGraph.Plan = 2;
            AgeReadingView.RednessGraph.control = AgeReadingView;
            AgeReadingView.RednessGraph.Create();
            AgeReadingView.RednessGraph.MakeGraph();

            AgeReadingView.GrowthGraph.Plan = 3;
            AgeReadingView.GrowthGraph.control = AgeReadingView;
            AgeReadingView.GrowthGraph.Create();
            AgeReadingView.GrowthGraph.MakeGraph();
        }

        public void UpdateGraphs()
        {
            if (!String.IsNullOrWhiteSpace(Global.API.Connection))
            {
                AgeReadingView.BrightnessGraph.MakeGraph();
                AgeReadingView.RednessGraph.MakeGraph();
                AgeReadingView.GrowthGraph.MakeGraph();
            }
        }

        public bool SaveLayout()
        {
            try
            {
                AgeReadingView.DockLayoutManager.SaveLayoutToXml("Layout.xml");
                return true;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving Layout", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
            return false;
        }

        public void LoadLayout(string file)
        {
            try
            {
                AgeReadingView.DockLayoutManager.RestoreLayoutFromXml(file);
            }
            catch (Exception e)
            {
                //if layout file is corrupt or does not exist
                //Helper.ShowWinUIMessageBox("No Layout file found", "Error", MessageBoxButton.OK, MessageBoxImage.Information, e);
                try
                {
                    AgeReadingView.DockLayoutManager.RestoreLayoutFromXml("DefaultLayout.xml");
                }
                catch (Exception ex)
                {
                    Helper.ShowWinUIMessageBox("Error loading Layout", "Error", MessageBoxButton.OK, MessageBoxImage.Error, ex);
                }
            }
        }

        public void EnableUI(bool enabled)
        {
            AgeReadingEditorView.EditorStackPanel.IsEnabled = enabled;
            AgeReadingFileView.IsEnabled = enabled;
            AgeReadingAnnotationView.IsEnabled = enabled;
            AgeReadingSampleView.IsEnabled = enabled;
            AgeReadingView.BrightnessPanel.IsEnabled = enabled;
            AgeReadingView.RednessPanel.IsEnabled = enabled;
            AgeReadingView.GrowthPanel.IsEnabled = enabled;
            //AgeReadingView.CollectionAppBar.IsEnabled = enabled;
            AgeReadingView.Previous.IsEnabled = enabled;
            AgeReadingView.Next.IsEnabled = enabled;
            AgeReadingStatusbarView.imgZoomSlider.IsEnabled = enabled;
            AgeReadingStatusbarView.btn200Percent.IsEnabled = enabled;
            AgeReadingStatusbarView.btn150Percent.IsEnabled = enabled;
            AgeReadingStatusbarView.btn100Percent.IsEnabled = enabled;
            AgeReadingStatusbarView.btn50Percent.IsEnabled = enabled;
            AgeReadingStatusbarView.btn25Percent.IsEnabled = enabled;
            AgeReadingStatusbarView.btnFit.IsEnabled = enabled;
            WaitState = !enabled;
        }

        public override void Save()
        {
            SaveLayout();
            AgeReadingEditorViewModel.SaveUserPreferences();
            if (AgeReadingAnnotationViewModel.WorkingAnnotation != null && AgeReadingAnnotationViewModel.WorkingAnnotation.QualityID == null && !AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed)
            {
                if (!AgeReadingAnnotationViewModel.EditAnnotation()) return; //SaveAnnotations();
            }
            else if (AgeReadingAnnotationViewModel.Outcomes.Any())
            {
                SaveAnnotations();
            }
        }

        public void HandleClosing()
        {
            SaveLayout();
            AgeReadingEditorViewModel.SaveUserPreferences();
            if (AgeReadingEditorViewModel.ShapeChangeFlag && AgeReadingAnnotationViewModel.WorkingAnnotation != null)
            {
                if (AgeReadingAnnotationViewModel.EditAnnotation() && AgeReadingAnnotationViewModel.Outcomes.Any())
                    SaveAnnotations();
                else
                {
                    return;
                }
            }
            else if (AgeReadingAnnotationViewModel.Outcomes.Any())
            {
                SaveAnnotations();
            }

            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        public void ShowWaitSplashScreen()
        {
            try
            {
                if (!DXSplashScreen.IsActive && !FirstLoad) DXSplashScreen.Show<WaitSplashScreen>();
            }
            catch (Exception ex)
            {
            }
        }

        public void CloseSplashScreen()
        {
            try
            {
                DXSplashScreen.DisableThreadingProblemsDetection = true;
                if (DXSplashScreen.IsActive && !FirstLoad) DXSplashScreen.Close();
            }
            catch (Exception ex)
            {
            }
        }

        //public void ToolsetBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    ToolsetVisibile = !ToolsetVisibile;
        //}

        public void ToggleGraphs()
        {
            int visiblegraphs = 0;
            if (AgeReadingView.BrightnessPanel.Visibility == Visibility.Visible) visiblegraphs++;
            if (AgeReadingView.RednessPanel.Visibility == Visibility.Visible) visiblegraphs++;
            if (AgeReadingView.GrowthPanel.Visibility == Visibility.Visible) visiblegraphs++;

            if (visiblegraphs >= 2)
            {
                AgeReadingView.BrightnessPanel.Visibility = Visibility.Collapsed;
                AgeReadingView.RednessPanel.Visibility = Visibility.Collapsed;
                AgeReadingView.GrowthPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AgeReadingView.BrightnessPanel.Visibility = Visibility.Visible;
                AgeReadingView.RednessPanel.Visibility = Visibility.Visible;
                AgeReadingView.GrowthPanel.Visibility = Visibility.Visible;
            }
        }

        public void GoBack()
        {
            Save();
            AgeReadingAnnotationViewModel.WorkingAnnotation = null;
            AgeReadingView.MainWindowViewModel.SetActiveControl(AgeReadingView.MainWindowViewModel.ServerSelectionView);
            AgeReadingView.MainWindowViewModel.ServerSelectionView.LoadGrid();
            AgeReadingView.MainWindowViewModel.HeaderInfo = $"  {Global.API.Settings.EventAlias.ToUpper()} OVERVIEW";
            AgeReadingView.MainWindowViewModel.HeaderModule = "";
            AgeReadingView.Opacity = 0;
            AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            AgeReadingView.WinFormGrowth.Visibility = Visibility.Hidden;
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            WaitState = true;
            AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            AgeReadingView.WinFormGrowth.Visibility = Visibility.Hidden;
            RaisePropertyChanged(null); //triggers propertychanged on all properties
            dialog.ShowDialog();
        }



        //todo future key combinations
        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            //{
            //    AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.Undo();
            //}

            //if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            //{
            //    AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.Undo();
            //}
        }

        //public void Window_StateChanged(object sender, EventArgs e)
        //{
        //    AgeReadingView.ShowInTaskbar = true;
        //}
    }
}
