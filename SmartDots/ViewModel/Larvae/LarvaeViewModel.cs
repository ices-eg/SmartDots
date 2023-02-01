using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Model;
using SmartDots.Helpers;
using SmartDots.View;
using SmartDots.ViewModel.AgeReading;

namespace SmartDots.ViewModel
{
    public class LarvaeViewModel : LarvaeBaseViewModel
    {
        //private bool toolsetVisibile = true;
        private bool waitState;
        private bool loadingAnalysis;
        private bool isListening;
        private string headerInfo;
        private string sampleAlias;
        private string activeTab;

        private LarvaeAnalysis larvaeAnalysis;

        public string HeaderInfo
        {
            get { return headerInfo; }
            set
            {
                headerInfo = value;
                RaisePropertyChanged("HeaderInfo");
            }
        }

        public LarvaeAnalysis LarvaeAnalysis
        {
            get { return larvaeAnalysis; }
            set
            {
                larvaeAnalysis = value;
                //get parameters

                //AgeReadingAnnotationViewModel.Parameters = analysis.AnalysisParameters;
                //AgeReadingAnnotationViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                //AgeReadingAnnotationViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                //EditAnnotationDialogViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                //EditAnnotationDialogViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                //EditAnnotationDialogViewModel.Parameters = analysis.AnalysisParameters;
                if (larvaeAnalysis.Type.ToLower().Contains("egg"))
                {
                    LarvaeView.MainWindowViewModel.HeaderModule = "  EGG";
                }
                else
                {
                    LarvaeView.MainWindowViewModel.HeaderModule = "  LARVAE";
                }

                LarvaeView.MainWindowViewModel.HeaderInfo = $"  {LarvaeAnalysis.HeaderInfo}";
                RaisePropertyChanged("LarvaeAnalysis");

                LarvaeEditorViewModel.RaisePropertyChanged("LarvaeButtonsVisibility");
                LarvaeEditorViewModel.RaisePropertyChanged("EggButtonsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("LarvaeColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("LarvaeColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("EggColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("ExtraRowSize");
                LarvaeAllAnnotationViewModel.RaisePropertyChanged("IsLarvaeAnalysis");
                LarvaeAllAnnotationViewModel.RaisePropertyChanged("IsEggAnalysis");

                LarvaeSampleViewModel.LarvaeSamples = larvaeAnalysis.LarvaeSamples;
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

        public bool LoadingAnalysis
        {
            get { return loadingAnalysis; }
            set { loadingAnalysis = value; }
        }

        public bool CanToggleApprove
        {
            get
            {
                if (LarvaeSampleViewModel?.SelectedSample == null) return false;
                return LarvaeSampleViewModel.SelectedSample.AllowApproveToggle;
            }
        }

        public string ApproveButtonText
        {
            get
            {
                if (LarvaeOwnAnnotationViewModel?.Annotation == null) return "APPROVE";
                if (!LarvaeOwnAnnotationViewModel.Annotation.IsApproved) return "APPROVE";
                return "UNAPPROVE";
            }
        }

        public string ActiveTab
        {
            get { return activeTab; }
            set
            {
                if (activeTab != value && (value == "AnnotationOwn" || value == "AnnotationAll"))
                {
                    activeTab = value;
                    RaisePropertyChanged("ActiveTab");
                    LarvaeEditorViewModel.UpdateButtons();
                    LarvaeEditorViewModel.RefreshShapes();
                }
            }
        }

        public void Refresh()
        {
            LarvaeView.btnApprove.Content = ApproveButtonText;
            LarvaeView.btnApprove.IsEnabled = CanToggleApprove;
            //RaisePropertyChanged("ApproveButtonText");
            //RaisePropertyChanged("CanToggleApprove");
        }

        public string ImageSettings { get; set; } = "../../Resources/ok-32.png"; // Image that is used in the PanelControl Header

        public LarvaeView LarvaeView { get; set; }

        public LarvaeSampleView LarvaeSampleView { get; set; }
        public LarvaeSampleViewModel LarvaeSampleViewModel { get; set; }

        public LarvaeFileView LarvaeFileView { get; set; }
        public LarvaeFileViewModel LarvaeFileViewModel { get; set; }

        public LarvaeEditorView LarvaeEditorView { get; set; }
        public LarvaeEditorViewModel LarvaeEditorViewModel { get; set; }

        public LarvaeStatusbarView LarvaeStatusbarView { get; set; }
        public LarvaeStatusbarViewModel LarvaeStatusbarViewModel { get; set; }

        public LarvaeOwnAnnotationView LarvaeOwnAnnotationView { get; set; }
        public LarvaeOwnAnnotationViewModel LarvaeOwnAnnotationViewModel { get; set; }

        public LarvaeAllAnnotationView LarvaeAllAnnotationView { get; set; }
        public LarvaeAllAnnotationViewModel LarvaeAllAnnotationViewModel { get; set; }


        public LarvaeViewModel()
        {
            //CloseSplashScreen();

            LarvaeSampleView = new LarvaeSampleView(this);
            LarvaeSampleViewModel = LarvaeSampleView.LarvaeSampleViewModel;

            LarvaeFileView = new LarvaeFileView(this);
            LarvaeFileViewModel = LarvaeFileView.LarvaeFileViewModel;

            LarvaeEditorView = new LarvaeEditorView(this);
            LarvaeEditorViewModel = LarvaeEditorView.LarvaeEditorViewModel;

            LarvaeStatusbarView = new LarvaeStatusbarView(this);
            LarvaeStatusbarViewModel = LarvaeStatusbarView.LarvaeStatusbarViewModel;

            LarvaeOwnAnnotationView = new LarvaeOwnAnnotationView(this);
            LarvaeOwnAnnotationViewModel = LarvaeOwnAnnotationView.LarvaeOwnAnnotationViewModel;

            LarvaeAllAnnotationView = new LarvaeAllAnnotationView(this);
            LarvaeAllAnnotationViewModel = LarvaeAllAnnotationView.LarvaeAllAnnotationViewModel;



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

        public bool LoadLookups(Guid analysisid, string type)
        {
            var typeLetter = type.ToLower().Contains("lar") ? "l" : "e";


            var larvaePresencesVocab = Global.API.GetVocab(analysisid, $"{typeLetter}pr");
            if (!larvaePresencesVocab.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Larvae presence vocab from the Web API", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            var larvaePresences = new List<LarvaePresence>();
            foreach (var dtoLookup in larvaePresencesVocab.Result)
            {
                larvaePresences.Add((LarvaePresence)Helper.ConvertType(dtoLookup, typeof(LarvaePresence)));
            }

            LarvaeOwnAnnotationViewModel.LarvaePresences = larvaePresences;

            var larvaeQualityVocab = Global.API.GetVocab(analysisid, $"{typeLetter}aq");
            if (!larvaeQualityVocab.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Larvae quality vocab from the Web API", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            var larvaeQualities = new List<LarvaeEggQuality>();
            foreach (var dtoLookup in larvaeQualityVocab.Result)
            {
                larvaeQualities.Add((LarvaeEggQuality)Helper.ConvertType(dtoLookup, typeof(LarvaeEggQuality)));
            }

            LarvaeOwnAnnotationViewModel.LarvaeQualities = larvaeQualities;

            var larvaeDevelopmentStageVocab = Global.API.GetVocab(analysisid, $"{typeLetter}ds");
            if (!larvaeDevelopmentStageVocab.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Larvae development stages vocab from the Web API", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            var larvaeDevelopmentStages = new List<LarvaeDevelopmentStage>();
            foreach (var dtoLookup in larvaeDevelopmentStageVocab.Result)
            {
                larvaeDevelopmentStages.Add((LarvaeDevelopmentStage)Helper.ConvertType(dtoLookup, typeof(LarvaeDevelopmentStage)));
            }

            LarvaeOwnAnnotationViewModel.LarvaeDevelopmentStages = larvaeDevelopmentStages;

            var larvaeSpeciesVocab = Global.API.GetVocab(analysisid, $"{typeLetter}sp");
            if (!larvaeSpeciesVocab.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Larvae species vocab from the Web API", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            var larvaeSpecies = new List<LarvaeSpecies>();
            foreach (var dtoLookup in larvaeSpeciesVocab.Result)
            {
                larvaeSpecies.Add((LarvaeSpecies)Helper.ConvertType(dtoLookup, typeof(LarvaeSpecies)));
            }

            LarvaeOwnAnnotationViewModel.LarvaeSpecies = larvaeSpecies;

            if (type.ToLower().Contains("egg"))
            {
                var eggEmbryoSizeVocab = Global.API.GetVocab(analysisid, "ees");
                if (!eggEmbryoSizeVocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Egg embryo size vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

                var embryoSizes = new List<EggEmbryoSize>();
                foreach (var dtoLookup in eggEmbryoSizeVocab.Result)
                {
                    embryoSizes.Add((EggEmbryoSize)Helper.ConvertType(dtoLookup, typeof(EggEmbryoSize)));
                }

                LarvaeOwnAnnotationViewModel.EggEmbryoSizes = embryoSizes;

                var eggYolkSegmentationVocab = Global.API.GetVocab(analysisid, "eys");
                if (!eggYolkSegmentationVocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Egg yolk segmentation vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

                var eggYolkSegmentations = new List<EggYolkSegmentation>();
                foreach (var dtoLookup in eggYolkSegmentationVocab.Result)
                {
                    eggYolkSegmentations.Add((EggYolkSegmentation)Helper.ConvertType(dtoLookup, typeof(EggYolkSegmentation)));
                }

                LarvaeOwnAnnotationViewModel.EggYolkSegmentations = eggYolkSegmentations;
            }

            return true;
        }



        public bool LoadLarvaeAnalysis(Guid larvaeAnalysisid, string type)
        {
            try
            {
                LoadingAnalysis = true;
                ShowWaitSplashScreen();
                Helper.MultiUserColorsDict.Clear();

                var dtoLarvaeAnalysis = Global.API.GetLarvaeAnalysis(larvaeAnalysisid, type);
                if (!dtoLarvaeAnalysis.Succeeded)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Error loading analysis from the Web API\n" + dtoLarvaeAnalysis.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var larvaeAnalysis = (LarvaeAnalysis)Helper.ConvertType(dtoLarvaeAnalysis.Result, typeof(LarvaeAnalysis));
                if (!LoadLookups(larvaeAnalysisid, larvaeAnalysis.Type))
                {
                    Helper.ShowWinUIMessageBox("Unable to load lookups", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                List<LarvaeSample> larvaeSamples = new List<LarvaeSample>();
                foreach (var dtoLarvaeSamples in dtoLarvaeAnalysis.Result.LarvaeSamples)
                {
                    larvaeSamples.Add((LarvaeSample)Helper.ConvertType(dtoLarvaeSamples, typeof(LarvaeSample)));
                }

                larvaeAnalysis.LarvaeSamples = larvaeSamples;
                List<LarvaeParameter> larvaeParameters = new List<LarvaeParameter>();
                foreach (var dtoLarvaeParameters in dtoLarvaeAnalysis.Result.LarvaeParameters)
                {
                    larvaeParameters.Add((LarvaeParameter)Helper.ConvertType(dtoLarvaeParameters, typeof(LarvaeParameter)));
                }
                larvaeAnalysis.LarvaeParameters = larvaeParameters;
                //if (larvaeAnalysis.Folder == null)
                //{
                //    Helper.ShowWinUIMessageBox("Larvae analysis does not contain a folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return false;
                //}
                //List<AnalysisParameter> analysisParameters = new List<AnalysisParameter>();
                //foreach (var dtoAnalysisParameter in dtoLarvaeAnalysis.Result.AnalysisParameters)
                //{
                //    analysisParameters.Add((AnalysisParameter)Helper.ConvertType(dtoAnalysisParameter, typeof(AnalysisParameter)));
                //}
                //analysis.AnalysisParameters = analysisParameters;
                LarvaeAnalysis = larvaeAnalysis;
                //LarvaeSampleViewModel.LarvaeSamples = larvaeSamples;

                LarvaeSampleViewModel.SetDynamicLarvaeSamples();

                if (LarvaeSampleViewModel.LarvaeSamples.All(x => x.UserHasApproved))
                {
                    var all = LarvaeView.LarvaeAnnotationsAll;
                    if (all != null)
                    {
                        LarvaeView.DockLayoutManager.DockController.Activate(all);
                    }
                }
                else
                {
                    var own = LarvaeView.LarvaeAnnotationOwn;
                    if (own != null)
                    {
                        LarvaeView.DockLayoutManager.DockController.Activate(own);
                    }
                }
                //if (!Helper.FolderExists(larvaeAnalysis.Folder.Path)) throw new Exception("Could not find Folder " + larvaeAnalysis.Folder.Path);
                //AgeReadingFileViewModel.CurrentFolder = Analysis.Folder;
                CloseSplashScreen();

                if (dtoLarvaeAnalysis.WarningMessage != null)
                {
                    Helper.ShowWinUIMessageBox(dtoLarvaeAnalysis.WarningMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

                LoadingAnalysis = false;
                return true;

            }
            catch (Exception e)
            {
                LoadingAnalysis = false;
                CloseSplashScreen();
                Helper.ShowWinUIMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
                return false;
            }
        }


        public bool ValidateAnnotation()
        {
            //if (AgeReadingAnnotationViewModel.WorkingAnnotation == null) return true;
            ////check if AQ is inserted
            //if (AgeReadingAnnotationViewModel.WorkingAnnotation?.QualityID == null)
            //{
            //    Helper.ShowWinUIMessageBox("Please give the annotation a quality", "Info", MessageBoxButton.OK,
            //        MessageBoxImage.Information);
            //    return false;
            //}
            ////check if only 1 line is inserted
            //if (AgeReadingAnnotationViewModel.WorkingAnnotation?.CombinedLines.Count != 1)
            //{
            //    Helper.ShowWinUIMessageBox("Please make sure the annotation has 1 line", "Info", MessageBoxButton.OK,
            //        MessageBoxImage.Information);
            //    return false;
            //}
            return true;
        }

        public void SaveAnnotations()
        {
            try
            {
                //List<Annotation> outcomes = AgeReadingAnnotationViewModel.Outcomes.Where(x => x.IsChanged).ToList();
                //if (!outcomes.Any()) return;
                //WaitState = true;

                //List<DtoAnnotation> dboutcomes = new List<DtoAnnotation>();
                //foreach (var outcome in outcomes)
                //{
                //    var lines = new List<DtoLine>();
                //    if (outcome.CombinedLines.Any())
                //    {
                //        foreach (var line in outcome.CombinedLines[0].Lines)
                //        {
                //            lines.Add((DtoLine)Helper.ConvertType(line, typeof(DtoLine)));
                //        }
                //    }

                //    var dots = new List<DtoDot>();
                //    if (outcome.CombinedLines.Any() && !outcome.HasAq3())
                //    {
                //        foreach (var dot in outcome.CombinedLines[0].Dots)
                //        {
                //            dots.Add((DtoDot)Helper.ConvertType(dot, typeof(DtoDot)));
                //        }
                //    }


                //    DtoAnnotation dboutcome = (DtoAnnotation)Helper.ConvertType(outcome, typeof(DtoAnnotation));
                //    dboutcome.Dots = dots;
                //    dboutcome.Lines = lines;
                //    dboutcome.Nucleus = outcome.Nucleus;
                //    dboutcome.Edge = outcome.Edge;
                //    dboutcomes.Add(dboutcome);
                //}
                //var updateAnnotationsResult = Global.API.UpdateAnnotations(dboutcomes);
                //if (!updateAnnotationsResult.Succeeded)
                //{
                //    Helper.ShowWinUIMessageBox("Error Saving Annotations\n" + updateAnnotationsResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                //AgeReadingEditorViewModel.ShapeChangeFlag = false;

                //var dbfile = Global.API.GetFile(AgeReadingFileViewModel.SelectedFile.ID, false, true);
                //if (!dbfile.Succeeded)
                //{
                //    Helper.ShowWinUIMessageBox("Error loading File from Web API\n" + dbfile.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}
                //var file = (File)Helper.ConvertType(dbfile.Result, typeof(File));
                //file.Sample = (Sample)Helper.ConvertType(dbfile.Result.Sample, typeof(Sample));
                //var selectedFile = AgeReadingFileViewModel.Files.FirstOrDefault(x => x == AgeReadingFileViewModel.SelectedFile);
                //if (selectedFile != null)
                //{
                //    selectedFile.Sample = file.Sample;
                //    //selectedFile.AnnotationCount = file.AnnotationCount; this is not needed here
                //    selectedFile.IsReadOnly = file.IsReadOnly;
                //    selectedFile.Scale = file.Scale;
                //    selectedFile.FetchProps((dynamic)AgeReadingFileView.FileList.FocusedRowData.Row);
                //    var dynFile = AgeReadingFileViewModel.CreateDynamicFile(selectedFile);
                //    AgeReadingFileView.FileList.FocusedRowData.Row = dynFile;
                //}

                //AgeReadingAnnotationViewModel.UpdateList();
                //AgeReadingFileViewModel.UpdateList();

                //foreach (var outcome in outcomes)
                //{
                //    outcome.IsChanged = false;
                //}
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

        public void PromptAnalysisCompleteDialog()
        {
            var result =
                Helper.ShowWinUIDialog(
                    $"You have approved all samples in this {Global.API.Settings.EventAlias}. Do you want to mark this {Global.API.Settings.EventAlias} as finished?",
                    $"{Global.API.Settings.EventAlias} finished", MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ShowWaitSplashScreen();
                    var webapiresult = Global.API.ToggleLarvaeAnalysisUserProgress(LarvaeAnalysis.ID, LarvaeViewModel.LarvaeAnalysis.Type);
                    if (!webapiresult.Succeeded)
                    {

                        Helper.ShowWinUIMessageBox(webapiresult.ErrorMessage, $"Error toggling user {Global.API.Settings.EventAlias} progress", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        GoBack();
                        // show the all annotations tab
                    }
                    CloseSplashScreen();
                }
                catch (Exception e)
                {
                    CloseSplashScreen();
                }


            }

        }

        public bool SaveLayout()
        {
            try
            {
                LarvaeView.DockLayoutManager.SaveLayoutToXml("LarvaeLayout.xml");
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
                LarvaeView.DockLayoutManager.RestoreLayoutFromXml(file);
            }
            catch (Exception e)
            {
                //if layout file is corrupt or does not exist
                //Helper.ShowWinUIMessageBox("No Layout file found", "Error", MessageBoxButton.OK, MessageBoxImage.Information, e);
                try
                {
                    LarvaeView.DockLayoutManager.RestoreLayoutFromXml("DefaultLarvaeLayout.xml");
                }
                catch (Exception ex)
                {
                    Helper.ShowWinUIMessageBox("Error loading Layout", "Error", MessageBoxButton.OK, MessageBoxImage.Error, ex);
                }
            }
        }

        public void EnableUI(bool enabled)
        {
            LarvaeView.Next.IsEnabled = enabled;
            LarvaeView.FileNext.IsEnabled = enabled;
            LarvaeView.Previous.IsEnabled = enabled;
            LarvaeView.FilePrevious.IsEnabled = enabled;
            LarvaeView.btnApprove.IsEnabled = enabled;
            LarvaeEditorView.EditorStackPanel.IsEnabled = enabled;
            LarvaeFileView.IsEnabled = enabled;
            LarvaeOwnAnnotationView.IsEnabled = enabled;
            LarvaeAllAnnotationView.IsEnabled = enabled;
            LarvaeSampleView.IsEnabled = enabled;
            LarvaeEditorView.Adjustments.IsEnabled = enabled;
            LarvaeStatusbarView.IsEnabled = enabled;

            WaitState = !enabled;
        }

        public void Save()
        {
            SaveLayout();
            LarvaeEditorViewModel.SaveUserPreferences();
            if (LarvaeOwnAnnotationViewModel?.Annotation != null && LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving)
            {
                LarvaeOwnAnnotationViewModel.SaveAnnotation();
            }
        }

        public void HandleClosing()
        {
            //SaveLayout();
            //AgeReadingEditorViewModel.SaveUserPreferences();
            //if (AgeReadingEditorViewModel.ShapeChangeFlag && AgeReadingAnnotationViewModel.WorkingAnnotation != null)
            //{
            //    if (AgeReadingAnnotationViewModel.EditAnnotation() && AgeReadingAnnotationViewModel.Outcomes.Any())
            //        SaveAnnotations();
            //    else
            //    {
            //        return;
            //    }
            //}
            //else if (AgeReadingAnnotationViewModel.Outcomes.Any())
            //{
            //    SaveAnnotations();
            //}

            //Application.Current.Shutdown();
            //Environment.Exit(0);
        }

        public void ShowWaitSplashScreen()
        {
            try
            {
                if (!DXSplashScreen.IsActive) DXSplashScreen.Show<WaitSplashScreen>();
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
                if (DXSplashScreen.IsActive) DXSplashScreen.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            //AgeReadingEditorViewModel.RestoreUserPreferences();
            LoadLayout("DefaultLayout.xml");
        }

        public void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public void GoBack()
        {
            Save();
            //AgeReadingAnnotationViewModel.WorkingAnnotation = null;
            LarvaeView.MainWindowViewModel.SetActiveControl(LarvaeView.MainWindowViewModel.ServerSelectionView);
            LarvaeView.MainWindowViewModel.HeaderInfo = $"  {Global.API.Settings.EventAlias.ToUpper()} OVERVIEW";
            LarvaeView.MainWindowViewModel.HeaderModule = "";
            LarvaeView.MainWindowViewModel.ServerSelectionView.LoadGrid();
            //AgeReadingView.MainWindowViewModel.ServerSelectionView.LoadGrid();
            //AgeReadingView.MainWindowViewModel.HeaderInfo = Global.API.Settings.EventAlias + " overview";
            //AgeReadingView.Opacity = 0;
            //AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            //AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            //AgeReadingView.WinFormGrowth.Visibility = Visibility.Hidden;
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            //WaitState = true;
            //AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            //AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            //AgeReadingView.WinFormGrowth.Visibility = Visibility.Hidden;
            //RaisePropertyChanged(null); //triggers propertychanged on all properties
            //dialog.ShowDialog();
        }

        public void LarvaeAnnotationOwn_GotFocus(object sender, RoutedEventArgs e)
        {
            ActiveTab = "AnnotationOwn";
        }

        public void LarvaeAnnotationAll_GotFocus(object sender, RoutedEventArgs e)
        {
            ActiveTab = "AnnotationAll";
        }
    }
}
