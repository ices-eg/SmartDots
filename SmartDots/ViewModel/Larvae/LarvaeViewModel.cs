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

                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("LarvaeColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("LarvaeColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("EggColumnsVisibility");
                LarvaeOwnAnnotationViewModel.RaisePropertyChanged("ExtraRowSize");
                LarvaeAllAnnotationViewModel.RaisePropertyChanged("IsLarvaeAnalysis");
                LarvaeAllAnnotationViewModel.RaisePropertyChanged("IsEggAnalysis");

                LarvaeSampleViewModel.LarvaeSamples = larvaeAnalysis.LarvaeEggSamples;
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
        }

        private void ReportError(object sender, WebApiEventArgs e)
        {
            Helper.ShowWinUIMessageBox(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e.Error);
        }

        public bool LoadLookups()
        {
            Dictionary<LarvaeAnnotationProperty, List<DtoLookupItem>> dict = new Dictionary<LarvaeAnnotationProperty, List<DtoLookupItem>>();
            foreach (var prop in LarvaeAnalysis.LarvaeEggAnnotationProperties)
            {
                var vocab = Global.API.GetVocab(LarvaeAnalysis.ID, prop.VocabCode);
                if (!vocab.Succeeded)
                {
                    Helper.ShowWinUIMessageBox($"Error loading {prop.Label} vocab from the Web API", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
                dict.Add(prop, vocab.Result);
            }

            LarvaeOwnAnnotationViewModel.Dict = dict;

            return true;
        }

        public bool LoadLarvaeAnalysis(Guid larvaeAnalysisid, string type)
        {
            try
            {
                LoadingAnalysis = true;
                ShowWaitSplashScreen();
                Helper.MultiUserColorsDict.Clear();

                Global.LarvaeEggCurrentEventType = type;

                var dtoLarvaeAnalysis = Global.API.GetLarvaeAnalysis(larvaeAnalysisid, type);
                if (!dtoLarvaeAnalysis.Succeeded)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Error loading analysis from the Web API\n" + dtoLarvaeAnalysis.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var larvaeAnalysis = (LarvaeAnalysis)Helper.ConvertType(dtoLarvaeAnalysis.Result, typeof(LarvaeAnalysis));
                
                if (dtoLarvaeAnalysis.Result.LarvaeEggSamples == null)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Samples are missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                List<LarvaeSample> larvaeSamples = new List<LarvaeSample>();
                foreach (var dtoLarvaeSamples in dtoLarvaeAnalysis.Result.LarvaeEggSamples)
                {
                    larvaeSamples.Add((LarvaeSample)Helper.ConvertType(dtoLarvaeSamples, typeof(LarvaeSample)));
                }

                larvaeAnalysis.LarvaeEggSamples = larvaeSamples;
                if (dtoLarvaeAnalysis.Result.LarvaeEggParameters == null)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Parameters are missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                List<LarvaeParameter> larvaeParameters = new List<LarvaeParameter>();
                foreach (var dtoLarvaeParameters in dtoLarvaeAnalysis.Result.LarvaeEggParameters)
                {
                    larvaeParameters.Add((LarvaeParameter)Helper.ConvertType(dtoLarvaeParameters, typeof(LarvaeParameter)));
                }
                larvaeAnalysis.LarvaeEggParameters = larvaeParameters;

                if (dtoLarvaeAnalysis.Result.LarvaeEggAnnotationProperties == null)
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Annotation properties are missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                List<LarvaeAnnotationProperty> larvaeAnnotationProperties = new List<LarvaeAnnotationProperty>();
                foreach (var dtoLarvaeAnnotationProperties in dtoLarvaeAnalysis.Result.LarvaeEggAnnotationProperties)
                {
                    larvaeAnnotationProperties.Add((LarvaeAnnotationProperty)Helper.ConvertType(dtoLarvaeAnnotationProperties, typeof(LarvaeAnnotationProperty)));
                }
                larvaeAnalysis.LarvaeEggAnnotationProperties = larvaeAnnotationProperties;

                LarvaeAnalysis = larvaeAnalysis;

                if (!LoadLookups())
                {
                    CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Unable to load lookups", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

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
                    var webapiresult = Global.API.ToggleLarvaeAnalysisUserProgress(LarvaeAnalysis.ID, LarvaeAnalysis.Type);
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
            LoadLayout("DefaultLayout.xml");
        }

        public void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public void GoBack()
        {
            Save();
            LarvaeView.MainWindowViewModel.SetActiveControl(LarvaeView.MainWindowViewModel.ServerSelectionView);
            LarvaeView.MainWindowViewModel.HeaderInfo = $"  {Global.API.Settings.EventAlias.ToUpper()} OVERVIEW";
            LarvaeView.MainWindowViewModel.HeaderModule = "";
            LarvaeView.MainWindowViewModel.ServerSelectionView.LoadGrid();
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
