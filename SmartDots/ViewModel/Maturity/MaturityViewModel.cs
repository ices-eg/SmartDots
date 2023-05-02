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
    public class MaturityViewModel : MaturityBaseViewModel
    {
        //private bool toolsetVisibile = true;
        private bool waitState;
        private bool isListening;
        private string headerInfo;
        private string sampleAlias;

        private MaturityAnalysis maturityAnalysis;

        public string HeaderInfo
        {
            get { return headerInfo; }
            set
            {
                headerInfo = value;
                RaisePropertyChanged("HeaderInfo");
            }
        }

        public MaturityAnalysis MaturityAnalysis
        {
            get { return maturityAnalysis; }
            set
            {
                maturityAnalysis = value;
                //get parameters

                //AgeReadingAnnotationViewModel.Parameters = analysis.AnalysisParameters;
                //AgeReadingAnnotationViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                //AgeReadingAnnotationViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                //EditAnnotationDialogViewModel.ShowNucleusColumn = analysis.ShowNucleusColumn;
                //EditAnnotationDialogViewModel.ShowEdgeColumn = analysis.ShowEdgeColumn;
                //EditAnnotationDialogViewModel.Parameters = analysis.AnalysisParameters;
                MaturityView.MainWindowViewModel.HeaderModule = "  MATURITY";
                MaturityView.MainWindowViewModel.HeaderInfo = $"  {MaturityAnalysis.HeaderInfo}";
                RaisePropertyChanged("MaturityAnalysis");

                MaturityView.MaturityViewModel.MaturitySampleViewModel.MaturitySamples = maturityAnalysis.MaturitySamples;
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

        public bool CanToggleApprove
        {
            get
            {
                if (MaturitySampleViewModel?.SelectedSample == null) return false;
                return MaturitySampleViewModel.SelectedSample.AllowApproveToggle;
            } 
        }

        public string ApproveButtonText
        {
            get
            {
                if (MaturityOwnAnnotationViewModel?.Annotation == null) return "APPROVE";
                if (!MaturityOwnAnnotationViewModel.Annotation.IsApproved) return "APPROVE";
                return "UNAPPROVE";
            }
        }

        public void Refresh()
        {
            MaturityView.btnApprove.Content = ApproveButtonText;
            MaturityView.btnApprove.IsEnabled = CanToggleApprove;
            //RaisePropertyChanged("ApproveButtonText");
            //RaisePropertyChanged("CanToggleApprove");
        }

        public string ImageSettings { get; set; } = "../../Resources/ok-32.png"; // Image that is used in the PanelControl Header

        public MaturityView MaturityView { get; set; }

        public MaturitySampleView MaturitySampleView { get; set; }
        public MaturitySampleViewModel MaturitySampleViewModel { get; set; }

        public MaturityFileView MaturityFileView { get; set; }
        public MaturityFileViewModel MaturityFileViewModel { get; set; }

        public MaturityEditorView MaturityEditorView { get; set; }
        public MaturityEditorViewModel MaturityEditorViewModel { get; set; }

        public MaturityStatusbarView MaturityStatusbarView { get; set; }
        public MaturityStatusbarViewModel MaturityStatusbarViewModel { get; set; }

        public MaturityOwnAnnotationView MaturityOwnAnnotationView { get; set; }
        public MaturityOwnAnnotationViewModel MaturityOwnAnnotationViewModel { get; set; }

        public MaturityAllAnnotationView MaturityAllAnnotationView { get; set; }
        public MaturityAllAnnotationViewModel MaturityAllAnnotationViewModel { get; set; }


        public MaturityViewModel()
        {
            //CloseSplashScreen();

            MaturitySampleView = new MaturitySampleView(this);
            MaturitySampleViewModel = MaturitySampleView.MaturitySampleViewModel;

            MaturityFileView = new MaturityFileView(this);
            MaturityFileViewModel = MaturityFileView.MaturityFileViewModel;

            MaturityEditorView = new MaturityEditorView(this);
            MaturityEditorViewModel = MaturityEditorView.MaturityEditorViewModel;

            MaturityStatusbarView = new MaturityStatusbarView(this);
            MaturityStatusbarViewModel = MaturityStatusbarView.MaturityStatusbarViewModel;

            MaturityOwnAnnotationView = new MaturityOwnAnnotationView(this);
            MaturityOwnAnnotationViewModel = MaturityOwnAnnotationView.MaturityOwnAnnotationViewModel;

            MaturityAllAnnotationView = new MaturityAllAnnotationView(this);
            MaturityAllAnnotationViewModel = MaturityAllAnnotationView.MaturityAllAnnotationViewModel;



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

        public bool LoadLookups(Guid analysisid)
        {
            //if (!MaturityOwnAnnotationViewModel.MaturitySexes.Any())
            //{
                var sexVocab = Global.API.GetVocab(analysisid, "sex");
                if (!sexVocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Sex vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    
                    return false;
                }
                
                var maturitySexes = new List<MaturitySex>();
                foreach (var dtoLookup in sexVocab.Result)
                {
                    maturitySexes.Add((MaturitySex)Helper.ConvertType(dtoLookup, typeof(MaturitySex)));
                }

                MaturityOwnAnnotationViewModel.MaturitySexes = maturitySexes;
            //}
            //if (!MaturityOwnAnnotationViewModel.Maturities.Any())
            //{
                var maturityVocab = Global.API.GetVocab(analysisid, "mat");
                if (!maturityVocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Maturity vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
                
                
                var maturities = new List<Maturity>();
                foreach (var dtoLookup in maturityVocab.Result)
                {
                    maturities.Add((Maturity)Helper.ConvertType(dtoLookup, typeof(Maturity)));
                }
                

                MaturityOwnAnnotationViewModel.Maturities = maturities;
            //}
            //if (!MaturityOwnAnnotationViewModel.MaturityQualities.Any())
            //{
                var maturityQualityVocab = Global.API.GetVocab(analysisid, "aqm");
                if (!maturityQualityVocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error loading Maturity quality vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
                
                var maturityQualities = new List<MaturityQuality>();
                foreach (var dtoLookup in maturityQualityVocab.Result)
                {
                    maturityQualities.Add((MaturityQuality)Helper.ConvertType(dtoLookup, typeof(MaturityQuality)));
                }

                MaturityOwnAnnotationViewModel.MaturityQualities = maturityQualities;
            //}


            return true;
        }



        public bool LoadMaturityAnalysis(Guid maturityAnalysisid)
        {
            try
            {
                ShowWaitSplashScreen();
                if (!LoadLookups(maturityAnalysisid))
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Unable to load maturity lookups", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                var dtoMaturityAnalysis = Global.API.GetMaturityAnalysis(maturityAnalysisid);
                if (!dtoMaturityAnalysis.Succeeded)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Error loading maturity analysis from the Web API\n" + dtoMaturityAnalysis.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var maturityAnalysis = (MaturityAnalysis)Helper.ConvertType(dtoMaturityAnalysis.Result, typeof(MaturityAnalysis));
                List<MaturitySample> maturitySamples = new List<MaturitySample>();
                foreach (var dtoMaturitySamples in dtoMaturityAnalysis.Result.MaturitySamples)
                {
                    maturitySamples.Add((MaturitySample)Helper.ConvertType(dtoMaturitySamples, typeof(MaturitySample)));
                }
                maturityAnalysis.MaturitySamples = maturitySamples;
                //if (maturityAnalysis.Folder == null)
                //{
                //    Helper.ShowWinUIMessageBox("Maturity analysis does not contain a folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return false;
                //}
                //List<AnalysisParameter> analysisParameters = new List<AnalysisParameter>();
                //foreach (var dtoAnalysisParameter in dtoMaturityAnalysis.Result.AnalysisParameters)
                //{
                //    analysisParameters.Add((AnalysisParameter)Helper.ConvertType(dtoAnalysisParameter, typeof(AnalysisParameter)));
                //}
                //analysis.AnalysisParameters = analysisParameters;
                MaturityAnalysis = maturityAnalysis;
                MaturitySampleViewModel.MaturitySamples = maturityAnalysis.MaturitySamples;
                MaturitySampleViewModel.SetDynamicMaturitySamples();

                if (MaturitySampleViewModel.MaturitySamples.All(x => x.UserHasApproved))
                {
                    var all = MaturityView.MaturityAnnotationsAll;
                    if (all != null)
                    {
                        MaturityView.DockLayoutManager.DockController.Activate(all);
                    }
                }
                else
                {
                    var own = MaturityView.MaturityAnnotationOwn;
                    if (own != null)
                    {
                        MaturityView.DockLayoutManager.DockController.Activate(own);
                    }
                }
                //if (!Helper.FolderExists(maturityAnalysis.Folder.Path)) throw new Exception("Could not find Folder " + maturityAnalysis.Folder.Path);
                //AgeReadingFileViewModel.CurrentFolder = Analysis.Folder;
                CloseSplashScreen();

                if (dtoMaturityAnalysis.WarningMessage != null)
                {
                    Helper.ShowWinUIMessageBox(dtoMaturityAnalysis.WarningMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

                return true;

            }
            catch (Exception e)
            {
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
                    var webapiresult = Global.API.ToggleMaturityAnalysisUserProgress(MaturityAnalysis.ID);
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
                MaturityView.DockLayoutManager.SaveLayoutToXml("MaturityLayout.xml");
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
                MaturityView.DockLayoutManager.RestoreLayoutFromXml(file);
            }
            catch (Exception e)
            {
                //if layout file is corrupt or does not exist
                //Helper.ShowWinUIMessageBox("No Layout file found", "Error", MessageBoxButton.OK, MessageBoxImage.Information, e);
                try
                {
                    MaturityView.DockLayoutManager.RestoreLayoutFromXml("DefaultMaturityLayout.xml");
                }
                catch (Exception ex)
                {
                    Helper.ShowWinUIMessageBox("Error loading Layout", "Error", MessageBoxButton.OK, MessageBoxImage.Error, ex);
                }
            }
        }

        public void EnableUI(bool enabled)
        {
            //AgeReadingEditorView.EditorStackPanel.IsEnabled = enabled;
            //AgeReadingFileView.IsEnabled = enabled;
            //AgeReadingAnnotationView.IsEnabled = enabled;
            //AgeReadingSampleView.IsEnabled = enabled;
            //AgeReadingView.Adjustments.IsEnabled = enabled;
            //AgeReadingView.BrightnessPanel.IsEnabled = enabled;
            //AgeReadingView.RednessPanel.IsEnabled = enabled;
            //AgeReadingView.GrowthPanel.IsEnabled = enabled;
            //AgeReadingView.CollectionAppBar.IsEnabled = enabled;
            //AgeReadingView.btnFileSettings.IsEnabled = enabled;
            //AgeReadingStatusbarView.imgZoomSlider.IsEnabled = enabled;
            //AgeReadingStatusbarView.btn200Percent.IsEnabled = enabled;
            //AgeReadingStatusbarView.btn150Percent.IsEnabled = enabled;
            //AgeReadingStatusbarView.btn100Percent.IsEnabled = enabled;
            //AgeReadingStatusbarView.btn50Percent.IsEnabled = enabled;
            //AgeReadingStatusbarView.btn25Percent.IsEnabled = enabled;
            //AgeReadingStatusbarView.btnFit.IsEnabled = enabled;
            WaitState = !enabled;
        }

        public void Save()
        {
            SaveLayout();
            MaturityEditorViewModel.SaveUserPreferences();
            if (MaturityOwnAnnotationViewModel?.Annotation != null && MaturityOwnAnnotationViewModel.Annotation.RequiresSaving)
            {
                MaturityOwnAnnotationViewModel.SaveAnnotation();
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
            MaturityView.MainWindowViewModel.SetActiveControl(MaturityView.MainWindowViewModel.ServerSelectionView);
            MaturityView.MainWindowViewModel.HeaderInfo = $"  {Global.API.Settings.EventAlias.ToUpper()} OVERVIEW";
            MaturityView.MainWindowViewModel.HeaderModule = "";
            MaturityView.MainWindowViewModel.ServerSelectionView.LoadGrid();
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


    }
}
