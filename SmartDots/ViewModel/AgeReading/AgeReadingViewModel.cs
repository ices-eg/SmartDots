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
using DevExpress.Xpf.Charts;
using Annotation = SmartDots.Model.Annotation;
using Newtonsoft.Json.Linq;
using System.Windows.Media;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using System.Windows.Controls.Primitives;
using System.Windows.Annotations;
using System.Threading.Tasks;
using System.Text;

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
        private string growthAllMode = "Own Annotations (all samples)";
        private string growthAllScale = "mm (use scale)";

        private Analysis analysis;
        private ChartControl chartControl;

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
                ToggleGrowthGraphOptions(analysis.UseGrowthGraphAllSamples);
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
        public string GrowthAllMode
        {
            get { return growthAllMode; }
            set
            {
                growthAllMode = value;
                UpdateGraphs(false, false, false, true);
                RaisePropertyChanged("GrowthAllMode");
            }
        }

        public string GrowthAllScale
        {
            get { return growthAllScale; }
            set
            {
                growthAllScale = value;
                UpdateGraphs(false, false, false, true);
                RaisePropertyChanged("GrowthAllScale");
            }
        }

        public AgeReadingStatusbarView AgeReadingStatusbarView { get; set; }
        public AgeReadingStatusbarViewModel AgeReadingStatusbarViewModel { get; set; }
        public AgeReadingAnnotationView AgeReadingAnnotationView { get; set; }
        public AgeReadingAnnotationViewModel AgeReadingAnnotationViewModel { get; set; }
        public AgeReadingFileView AgeReadingFileView { get; set; }
        public AgeReadingFileViewModel AgeReadingFileViewModel { get; set; }
        public AgeReadingSampleView AgeReadingSampleView { get; set; }
        public AgeReadingSampleViewModel AgeReadingSampleViewModel { get; set; }
        public AgeReadingEditorView AgeReadingEditorView { get; set; }
        public AgeReadingKeyMappingView AgeReadingKeyMappingView { get; set; }
        public AgeReadingEditorViewModel AgeReadingEditorViewModel { get; set; }
        public AgeReadingView AgeReadingView { get; set; }
        public AttachSampleDialog AttachSampleDialog { get; set; }
        public AttachSampleDialogViewModel AttachSampleDialogViewModel { get; set; }
        public EditAnnotationDialog EditAnnotationDialog { get; set; }
        public EditAnnotationDialogViewModel EditAnnotationDialogViewModel { get; set; }
        public SelectAnalysisDialog SelectAnalysisDialog { get; set; }
        public SelectAnalysisDialogViewModel SelectAnalysisDialogViewModel { get; set; }
        public AgeReadingKeyMappingViewModel AgeReadingKeyMappingViewModel { get; set; }

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

            AgeReadingKeyMappingView = new AgeReadingKeyMappingView(this);
            AgeReadingKeyMappingViewModel = AgeReadingKeyMappingView.AgeReadingKeyMappingViewModel;


            LoadUserPreferences();


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

        //public void ShowToast()
        //{
        //    CustomNotificationViewModel vm = ViewModelSource.Create(() => new CustomNotificationViewModel());
        //    vm.Caption = "Custom Notification";
        //    vm.Content = String.Format("Time: {0}", DateTime.Now);

        //    INotification notification = AgeReadingView.ServiceWithCustomNotifications.CreateCustomNotification(vm);
        //    notification.ShowAsync();
        //    return;
        //    //AgeReadingView.ServiceWithCustomNotifications.CreateCustomNotification("Test");
        //}

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

        private void ToggleGrowthGraphOptions(bool useGrowthGraphAllSamples)
        {
            AgeReadingView.AllAnnotations.Visibility = useGrowthGraphAllSamples ? Visibility.Visible : Visibility.Collapsed;
            AgeReadingView.AllAnnotationsA.Visibility = useGrowthGraphAllSamples ? Visibility.Visible : Visibility.Collapsed;
            AgeReadingView.OwnAnnotations.Visibility = useGrowthGraphAllSamples ? Visibility.Visible : Visibility.Collapsed;
            AgeReadingView.OwnAnnotationsA.Visibility = useGrowthGraphAllSamples ? Visibility.Visible : Visibility.Collapsed;
            if (!useGrowthGraphAllSamples)
            {
                AgeReadingView.GrowthAllMode.SelectedItem = AgeReadingView.SelectedAnnotations;
            }
            
        }

        public bool LoadAnalysis(Guid analysisid)
        {
            try
            {
                Helper.MultiUserColorsDict.Clear();

                Global.CurrentEventType = "agereading";
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
                if (!Helper.FolderExists(Analysis.Folder.Path)) throw new Exception("Could not find Folder " + Analysis.Folder.Path);
                AgeReadingFileViewModel.CurrentFolder = Analysis.Folder;
                AgeReadingView.GrowthAllMode.EditValue = GrowthAllMode;
                AgeReadingView.GrowthAllScale.EditValue = GrowthAllScale;
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

            //AgeReadingView.GrowthGraph.Plan = 3;
            //AgeReadingView.GrowthGraph.control = AgeReadingView;
            //AgeReadingView.GrowthGraph.Create();
            //AgeReadingView.GrowthGraph.MakeGraph();
        }

        public void UpdateGraphs(bool brightness = true, bool redness = true, bool growth = true, bool growthAll = true)
        {

            if (!String.IsNullOrWhiteSpace(Global.API.Connection))
            {
                bool brightnessVisible = Helper.IsUserVisible(AgeReadingView.BrightnessPanel, App.Current.MainWindow);
                bool rednessVisible = Helper.IsUserVisible(AgeReadingView.RednessPanel, App.Current.MainWindow);
                bool growthVisible = Helper.IsUserVisible(AgeReadingView.GrowthPanel, App.Current.MainWindow);


                if (brightness && AgeReadingView.BrightnessPanel.Visibility == Visibility.Visible && brightnessVisible) AgeReadingView.BrightnessGraph.MakeGraph();
                if (redness && AgeReadingView.RednessPanel.Visibility == Visibility.Visible && rednessVisible) AgeReadingView.RednessGraph.MakeGraph();
                //if (growth && AgeReadingView.GrowthPanel.Visibility == Visibility.Visible) AgeReadingView.GrowthGraph.MakeGraph();
                if (growthAll && AgeReadingView.GrowthPanel.Visibility == Visibility.Visible && growthVisible) MakeGrowthAllGraph();
            }
        }

        private void MakeGrowthAllGraph()
        {
            if (AgeReadingFileViewModel.Files == null) return;

            var blueBrush = (SolidColorBrush)Application.Current.TryFindResource("BrushSmartFishDarkBlue");
            var orangeBrush = (SolidColorBrush)Application.Current.TryFindResource("BrushSmartFishOrange");

            var chart = new DevExpress.Xpf.Charts.ChartControl() { Name = "GrowthAllChart" };
            chart.CustomDrawCrosshair += Chart_CustomDrawCrosshair;
            chart.Legend = new Legend();
            chart.Legend.Visibility = Visibility.Visible;
            chart.Legend.VerticalPosition = VerticalPosition.Bottom;
            chart.Legend.HorizontalPosition = HorizontalPosition.Right;
            bool aggregate = GrowthAllMode.Contains("Aggregated");
            string legendText1 = "Own";
            string legendText2 = "Other";
            if (GrowthAllMode == "Own Annotations (all samples)")
            {
                legendText1 = "Current sample";
                legendText2 = "Other samples";
            }
            if (aggregate)
            {
                legendText1 = "Selected Annotations";
            }
            chart.Legend.CustomItems.Add(new CustomLegendItem()
            {
                Text = legendText1,
                MarkerBrush = orangeBrush
            });


            var yAxis = "Distance (px)";

            double sideMarginsValueY = 20;
            double sideMarginsValueX = 0.20;

            if (GrowthAllScale == "mm (use scale)")
            {
                yAxis = "Distance (mm)";
                sideMarginsValueY = 0.3;
            }

            var diagram = new XYDiagram2D
            {
                Name = "GrowthAllGraph",
                AxisX = new AxisX2D()
                {
                    GridLinesVisible = true,
                    VisualRange = new DevExpress.Xpf.Charts.Range()
                    {
                        SideMarginsValue = sideMarginsValueX,
                        MinValue = 0

                    },
                    Title = new AxisTitle
                    {
                        Content = "Age"
                    },
                    NumericScaleOptions = new ContinuousNumericScaleOptions()
                    {
                        AutoGrid = false,
                        GridSpacing = 1
                    }
                },
                AxisY = new AxisY2D()
                {
                    VisualRange = new DevExpress.Xpf.Charts.Range()
                    {
                        SideMarginsValue = sideMarginsValueY,
                    },
                    Title = new AxisTitle
                    {
                        Content = yAxis
                    },
                    NumericScaleOptions = new ContinuousNumericScaleOptions()
                    {
                        AutoGrid = true,
                    },

                },

            };



            var allAnnotations = new List<Annotation>();
            allAnnotations.AddRange(AgeReadingAnnotationViewModel.Outcomes);
            allAnnotations.AddRange(AgeReadingFileViewModel.Files.Where(x => x.BoundOutcomes != null).SelectMany(x => x.BoundOutcomes).ToList());
            allAnnotations = allAnnotations.Distinct().ToList();

            var drawnAnnotations = new List<Annotation>();

            foreach (var file in AgeReadingFileViewModel.Files)
            {
                if (GrowthAllScale == "mm (use scale)" && (file.Scale == 0 || file.Scale == null)) continue;
                var annotations = allAnnotations.Where(x => x.FileID == file.ID).ToList();
                if (GrowthAllMode == "Selected Annotation(s)")
                {
                    annotations = annotations.Where(x => AgeReadingAnnotationViewModel.SelectedAnnotations.Select(y => y.ID).Contains(x.ID)).ToList();
                }
                else if (GrowthAllMode == "Own Annotations (all samples)")
                {
                    annotations = annotations.Where(x => x.LabTechnician == Global.API.CurrentUser.AccountName).ToList();
                }

                drawnAnnotations.AddRange(annotations);
            }


            if (aggregate)
            {
                chart.Legend.CustomItems.Add(new CustomLegendItem()
                {
                    Text = "Max Other",
                    MarkerBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0))
                });
                chart.Legend.CustomItems.Add(new CustomLegendItem()
                {
                    Text = "Avg Other",
                    MarkerBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0))
                });
                chart.Legend.CustomItems.Add(new CustomLegendItem()
                {
                    Text = "Min Other",
                    MarkerBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255))
                });
            }
            else
            {
                chart.Legend.CustomItems.Add(new CustomLegendItem()
                {
                    Text = legendText2,
                    MarkerBrush = blueBrush
                });
            }

            var selectedAnnotations = AgeReadingAnnotationViewModel.SelectedAnnotations;

            var otherAnnotations = new List<Annotation>();

            foreach (var annotation in drawnAnnotations)
            {
                var file = AgeReadingFileViewModel.Files.FirstOrDefault(x => x.ID == annotation.FileID);

                var series = new LineSeries2D
                {
                    LineStyle = new LineStyle() { Thickness = 2 },
                    DisplayName = file.Filename + " | " + annotation.LabTechnician,
                    Brush = blueBrush,
                    MarkerVisible = true,
                };

                if (
                    (GrowthAllMode == "Own Annotations (all samples)" && file == AgeReadingFileViewModel.SelectedFile)
                    ||
                    ((GrowthAllMode == "All Annotations (all samples)" || GrowthAllMode == "Selected Annotation(s)") && annotation.LabTechnician == Global.API.CurrentUser.AccountName)
                    )
                {
                    series.Brush = orangeBrush;
                }
                else if (aggregate)
                {
                    if(selectedAnnotations.Any(x => x.ID == annotation.ID))
                    {
                        series.Brush = orangeBrush;
                    }
                    else
                    {
                        otherAnnotations.Add(annotation);
                        continue;
                    }
                }



                List<decimal> values = new List<decimal>();

                if (annotation.CombinedLines.Any())
                {
                    values = annotation.CombinedLines.FirstOrDefault().CalculateLineGrowth();
                    if (GrowthAllScale == "mm (use scale)")
                    {
                        values = values.Select(x => x / file.Scale.GetValueOrDefault()).ToList();
                    }
                }




                for (int i = 0; i < values.Count; i++)
                {
                    series.Points.Add(new SeriesPoint(i.ToString(), Math.Round((double)values[i], 2)));
                }

                diagram.Series.Add(series);
            }

            if (aggregate)
            {
                var values = new List<List<decimal>>();

                for (int i = 0; i < otherAnnotations.Count; i++)
                {
                    var file = AgeReadingFileViewModel.Files.FirstOrDefault(x => x.ID == otherAnnotations[i].FileID);
                    var combinedLine = otherAnnotations[i].CombinedLines.FirstOrDefault();
                    if (combinedLine != null)
                    {
                        var lineGrowth = combinedLine.CalculateLineGrowth();
                        if (GrowthAllScale == "mm (use scale)")
                        {
                            lineGrowth = lineGrowth.Select(x => x / file.Scale.GetValueOrDefault()).ToList();
                        }
                        values.Add(lineGrowth);
                    }
                }
                if (!values.Any()) return;
                var maxCount = values.Max(x => x.Count);

                List<decimal> maxValues = new List<decimal>() { 0 };
                List<decimal> minValues = new List<decimal>() { 0 };
                List<decimal> avgValues = new List<decimal>() { 0 };
                for (int i = 1; i < maxCount; i++)
                {
                    // idea: also save the annotation and user for the max and min values
                    decimal maxVal = values.FirstOrDefault(x => x.Count == maxCount)[i];
                    decimal minVal = values.FirstOrDefault(x => x.Count == maxCount)[i];
                    decimal sumVal = 0;
                    int count = 0;
                    for (int j = 0; j < values.Count; j++)
                    {
                        if (values[j].Count > i)
                        {
                            if (values[j][i] > maxVal) maxVal = values[j][i];
                            if (values[j][i] < minVal) minVal = values[j][i];
                            sumVal += values[j][i];
                            count++;
                        }
                    }
                    maxValues.Add(maxVal);
                    minValues.Add(minVal);
                    avgValues.Add(sumVal / count);
                    //series.Points.Add(new SeriesPoint(i.ToString(), Math.Round((double)values[i], 2)));
                }

                var maxSeries = new LineSeries2D
                {
                    LineStyle = new LineStyle() { Thickness = 2 },
                    DisplayName = "Max Other",
                    Brush = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                    MarkerVisible = true,
                };

                for (int i = 0; i < maxValues.Count; i++)
                {
                    maxSeries.Points.Add(new SeriesPoint(i.ToString(), Math.Round((double)maxValues[i], 2)));
                }

                var minSeries = new LineSeries2D
                {
                    LineStyle = new LineStyle() { Thickness = 2 },
                    DisplayName = "Min Other",
                    Brush = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    MarkerVisible = true,
                };

                for (int i = 0; i < minValues.Count; i++)
                {
                    minSeries.Points.Add(new SeriesPoint(i.ToString(), Math.Round((double)minValues[i], 2)));
                }

                var avgSeries = new LineSeries2D
                {
                    LineStyle = new LineStyle() { Thickness = 2 },
                    DisplayName = "Avg Other",
                    Brush = new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                    MarkerVisible = true,
                };

                for (int i = 0; i < avgValues.Count; i++)
                {
                    avgSeries.Points.Add(new SeriesPoint(i.ToString(), Math.Round((double)avgValues[i], 2)));
                }

                diagram.Series.Add(maxSeries);
                diagram.Series.Add(avgSeries);
                diagram.Series.Add(minSeries);
            }





            chart.DataSource = diagram;
            chart.Diagram = diagram;

            chart.UpdateData();

            chartControl = chart;

            AgeReadingView.GrowthGraphPanel.Children.Clear();
            AgeReadingView.GrowthGraphPanel.Children.Add(chart);
        }

        private void Chart_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {
            foreach (CrosshairElementGroup group in e.CrosshairElementGroups)
            {
                if (group.CrosshairElements.Count > 1)
                {
                    var elements = group.CrosshairElements.OrderByDescending(x => x.AxisLabelElement.AxisValue).ToList();
                    for (int i = 0; i < group.CrosshairElements.Count; i++)
                    {
                        group.CrosshairElements[i] = elements[i];
                    }

                }
            }


            //var blueColor = "#FF293C45";
            //var orangeColor = "#FFF05026";

            //if (GrowthAllMode == "Selected Annotation(s)") return;
            //e.CrosshairElementGroups.First().HeaderElement.Text = "Age " + e.CrosshairElementGroups.First().HeaderElement.Text;
            //e.CrosshairElementGroups.First().CrosshairElements.OrderByDescending(x => x.AxisLabelElement.AxisValue);
            //var test = e.CrosshairElementGroups.First().CrosshairElements.First().LabelElement.MarkerBrush.ToString();
            //var test2 = e.CrosshairElementGroups.First().CrosshairElements.Count(x => x.LabelElement.MarkerBrush.ToString() == blueColor);
            //if (e.CrosshairElementGroups.First().CrosshairElements.Count(x => x.LabelElement.MarkerBrush.ToString() == blueColor) >= 3)
            //{
            //    var values = e.CrosshairElementGroups.First().CrosshairElements.Select(x => float.Parse(x.AxisLabelElement.Text)).ToList();
            //    // Calculate the average value of values
            //    var averageValue = values.Average();
            //    var maxValue = values.Max();
            //    var minValue = values.Min();


            //    e.CrosshairElementGroups.First().CrosshairElements[0].LabelElement.Text = $"Max: {maxValue.ToString("F2")}";
            //    e.CrosshairElementGroups.First().CrosshairElements[1].LabelElement.Text = $"Average: {averageValue.ToString("F2")}";
            //    e.CrosshairElementGroups.First().CrosshairElements[1].LabelElement.Text = $"Average: {averageValue.ToString("F2")}";
            //    e.CrosshairElementGroups.First().CrosshairElements[2].LabelElement.Text = $"Min: {minValue.ToString("F2")}";

            //    // Remove all other CrosshairElements with an index higher than 3
            //    for (int i = e.CrosshairElementGroups.First().CrosshairElements.Count - 1; i >= 0; i--)
            //    {
            //        if (i >= 3)
            //        {
            //            e.CrosshairElementGroups.First().CrosshairElements[i].LabelElement.Visible = false;
            //        }
            //    }

            //}
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
            SaveUserPreferences();
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
            SaveUserPreferences();
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
                UpdateGraphs();
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
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            WaitState = true;
            AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            RaisePropertyChanged(null); //triggers propertychanged on all properties
            dialog.ShowDialog();
        }

        public void SaveUserPreferences()
        {
            try
            {
                Properties.Settings.Default.DotColor = AgeReadingEditorViewModel.DotColor.ToString();
                Properties.Settings.Default.DotWidth = (int)AgeReadingEditorViewModel.DotWidth;
                Properties.Settings.Default.DotShape = AgeReadingEditorViewModel.DotShape;
                Properties.Settings.Default.DotType = AgeReadingEditorViewModel.DotType;
                Properties.Settings.Default.LineColor = AgeReadingEditorViewModel.LineColor.ToString();
                Properties.Settings.Default.LineWidth = (int)AgeReadingEditorViewModel.LineWidth;
                Properties.Settings.Default.MeasureColor = AgeReadingEditorViewModel.MeasureColor.ToString();
                Properties.Settings.Default.MeasureLineWidth = (int)AgeReadingEditorViewModel.MeasureLineWidth;
                Properties.Settings.Default.MeasureFontSize = (int)AgeReadingEditorViewModel.MeasureFontSize;
                Properties.Settings.Default.AgeReadingKeyAction1 = AgeReadingEditorViewModel.ShortcutActions[1].Item3;
                Properties.Settings.Default.AgeReadingKeyAction2 = AgeReadingEditorViewModel.ShortcutActions[2].Item3;
                Properties.Settings.Default.AgeReadingKeyAction3 = AgeReadingEditorViewModel.ShortcutActions[3].Item3;
                Properties.Settings.Default.AgeReadingKeyAction4 = AgeReadingEditorViewModel.ShortcutActions[4].Item3;
                Properties.Settings.Default.AgeReadingKeyAction5 = AgeReadingEditorViewModel.ShortcutActions[5].Item3;
                Properties.Settings.Default.AgeReadingKeyAction6 = AgeReadingEditorViewModel.ShortcutActions[6].Item3;
                Properties.Settings.Default.AgeReadingKeyAction7 = AgeReadingEditorViewModel.ShortcutActions[7].Item3;
                Properties.Settings.Default.AgeReadingKeyAction8 = AgeReadingEditorViewModel.ShortcutActions[8].Item3;
                Properties.Settings.Default.AgeReadingKeyAction9 = AgeReadingEditorViewModel.ShortcutActions[9].Item3;
                Properties.Settings.Default.AgeReadingKeyAction10 = AgeReadingEditorViewModel.ShortcutActions[10].Item3;
                Properties.Settings.Default.AgeReadingKeyAction11 = AgeReadingEditorViewModel.ShortcutActions[11].Item3;
                Properties.Settings.Default.AgeReadingKeyAction12 = AgeReadingEditorViewModel.ShortcutActions[12].Item3;
                Properties.Settings.Default.AgeReadingKeyAction13 = AgeReadingEditorViewModel.ShortcutActions[13].Item3;
                Properties.Settings.Default.AgeReadingKeyAction14 = AgeReadingEditorViewModel.ShortcutActions[14].Item3;
                Properties.Settings.Default.AgeReadingKeyAction15 = AgeReadingEditorViewModel.ShortcutActions[15].Item3;
                Properties.Settings.Default.AgeReadingKeyAction16 = AgeReadingEditorViewModel.ShortcutActions[16].Item3;
                Properties.Settings.Default.AgeReadingKeyAction17 = AgeReadingEditorViewModel.ShortcutActions[17].Item3;
                Properties.Settings.Default.AgeReadingKeyAction18 = AgeReadingEditorViewModel.ShortcutActions[18].Item3;
                Properties.Settings.Default.GrowthAllMode = GrowthAllMode;
                Properties.Settings.Default.GrowthAllScale = GrowthAllScale;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void LoadUserPreferences()
        {
            try
            {
                AgeReadingEditorViewModel.DotColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.DotColor);
                AgeReadingEditorViewModel.DotWidth = Properties.Settings.Default.DotWidth;
                AgeReadingEditorViewModel.DotShape = Properties.Settings.Default.DotShape;
                AgeReadingEditorViewModel.DotType = Properties.Settings.Default.DotType;
                AgeReadingEditorViewModel.LineColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.LineColor);
                AgeReadingEditorViewModel.LineWidth = Properties.Settings.Default.LineWidth;
                AgeReadingEditorViewModel.MeasureColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.MeasureColor);
                AgeReadingEditorViewModel.MeasureLineWidth = Properties.Settings.Default.MeasureLineWidth;
                AgeReadingEditorViewModel.MeasureFontSize = Properties.Settings.Default.MeasureFontSize;
                AgeReadingEditorViewModel.ShortcutActions[1] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[1].Item1, AgeReadingEditorViewModel.ShortcutActions[1].Item2, Properties.Settings.Default.AgeReadingKeyAction1);
                AgeReadingEditorViewModel.ShortcutActions[2] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[2].Item1, AgeReadingEditorViewModel.ShortcutActions[2].Item2, Properties.Settings.Default.AgeReadingKeyAction2);
                AgeReadingEditorViewModel.ShortcutActions[3] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[3].Item1, AgeReadingEditorViewModel.ShortcutActions[3].Item2, Properties.Settings.Default.AgeReadingKeyAction3);
                AgeReadingEditorViewModel.ShortcutActions[4] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[4].Item1, AgeReadingEditorViewModel.ShortcutActions[4].Item2, Properties.Settings.Default.AgeReadingKeyAction4);
                AgeReadingEditorViewModel.ShortcutActions[5] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[5].Item1, AgeReadingEditorViewModel.ShortcutActions[5].Item2, Properties.Settings.Default.AgeReadingKeyAction5);
                AgeReadingEditorViewModel.ShortcutActions[6] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[6].Item1, AgeReadingEditorViewModel.ShortcutActions[6].Item2, Properties.Settings.Default.AgeReadingKeyAction6);
                AgeReadingEditorViewModel.ShortcutActions[7] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[7].Item1, AgeReadingEditorViewModel.ShortcutActions[7].Item2, Properties.Settings.Default.AgeReadingKeyAction7);
                AgeReadingEditorViewModel.ShortcutActions[8] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[8].Item1, AgeReadingEditorViewModel.ShortcutActions[8].Item2, Properties.Settings.Default.AgeReadingKeyAction8);
                AgeReadingEditorViewModel.ShortcutActions[9] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[9].Item1, AgeReadingEditorViewModel.ShortcutActions[9].Item2, Properties.Settings.Default.AgeReadingKeyAction9);
                AgeReadingEditorViewModel.ShortcutActions[10] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[10].Item1, AgeReadingEditorViewModel.ShortcutActions[10].Item2, Properties.Settings.Default.AgeReadingKeyAction10);
                AgeReadingEditorViewModel.ShortcutActions[11] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[11].Item1, AgeReadingEditorViewModel.ShortcutActions[11].Item2, Properties.Settings.Default.AgeReadingKeyAction11);
                AgeReadingEditorViewModel.ShortcutActions[12] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[12].Item1, AgeReadingEditorViewModel.ShortcutActions[12].Item2, Properties.Settings.Default.AgeReadingKeyAction12);
                AgeReadingEditorViewModel.ShortcutActions[13] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[13].Item1, AgeReadingEditorViewModel.ShortcutActions[13].Item2, Properties.Settings.Default.AgeReadingKeyAction13);
                AgeReadingEditorViewModel.ShortcutActions[14] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[14].Item1, AgeReadingEditorViewModel.ShortcutActions[14].Item2, Properties.Settings.Default.AgeReadingKeyAction14);
                AgeReadingEditorViewModel.ShortcutActions[15] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[15].Item1, AgeReadingEditorViewModel.ShortcutActions[15].Item2, Properties.Settings.Default.AgeReadingKeyAction15);
                AgeReadingEditorViewModel.ShortcutActions[16] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[16].Item1, AgeReadingEditorViewModel.ShortcutActions[16].Item2, Properties.Settings.Default.AgeReadingKeyAction16);
                AgeReadingEditorViewModel.ShortcutActions[17] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[17].Item1, AgeReadingEditorViewModel.ShortcutActions[17].Item2, Properties.Settings.Default.AgeReadingKeyAction17);
                AgeReadingEditorViewModel.ShortcutActions[18] = new Tuple<Action, string, string>(AgeReadingEditorViewModel.ShortcutActions[18].Item1, AgeReadingEditorViewModel.ShortcutActions[18].Item2, Properties.Settings.Default.AgeReadingKeyAction18);
                GrowthAllMode = Properties.Settings.Default.GrowthAllMode;
                GrowthAllScale = Properties.Settings.Default.GrowthAllScale;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error loading user preferences", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
        }

        public void RestoreUserPreferences()
        {
            try
            {
                AgeReadingEditorViewModel.DotColor = (Color)ColorConverter.ConvertFromString("#FF00FF00");
                AgeReadingEditorViewModel.DotWidth = 8;
                AgeReadingEditorViewModel.DotShape = "Dot";
                AgeReadingEditorViewModel.DotType = "Seawater";
                AgeReadingEditorViewModel.LineColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
                AgeReadingEditorViewModel.LineWidth = 2;
                AgeReadingEditorViewModel.MeasureColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
                AgeReadingEditorViewModel.MeasureLineWidth = 1;
                AgeReadingEditorViewModel.MeasureFontSize = 12;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error restoring user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void GrowthCsvDownload()
        {
         

            var series = chartControl.Diagram.Series;

            if (series.Count == 0) return;

            StringBuilder csv = new StringBuilder();

            string unit = GrowthAllScale.Contains("mm") ? "(mm)" : "(px)";

            var maxCount = series.Select(x => x.Points.Count).Max();

            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append("Annotation");
            for (int i = 0; i < maxCount; i++)
            {
                headerBuilder.Append($";Age{i} {unit}");
            }

            csv.AppendLine(headerBuilder.ToString());

            foreach (var serie in series)
            {
                StringBuilder seriesBuilder = new StringBuilder();

                seriesBuilder.Append(serie.DisplayName.Replace(";", ""));
                foreach (var point in serie.Points)
                {
                    seriesBuilder.Append($";{point.Value}");
                }

                csv.AppendLine(seriesBuilder.ToString());

            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "GrowthData"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV file (*.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                System.IO.File.WriteAllText(filename, csv.ToString());
            }

            //save to disk
            

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
