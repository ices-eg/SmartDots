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
    public class LarvaeSampleViewModel : LarvaeBaseViewModel
    {
        private ObservableCollection<dynamic> dynamicSamples;
        private LarvaeSample selectedSample;
        private bool changingFile;


        public LarvaeSampleViewModel()
        {
        }

        public List<LarvaeSample> LarvaeSamples { get; set; }


        public ObservableCollection<dynamic> DynamicSamples
        {
            get { return dynamicSamples; }
            set
            {
                dynamicSamples = value;
                if (dynamicSamples.Any())
                {
                    if (SelectedSample?.ID != dynamicSamples[0]?.ID)
                    {
                        SelectedSample = LarvaeSamples.FirstOrDefault(x => x.ID == dynamicSamples[0].ID);
                        LoadSample();
                    }

                }

                RaisePropertyChanged("DynamicSamples");


            }
        }

        public LarvaeSample SelectedSample
        {
            get { return selectedSample; }
            set
            {
                LarvaeViewModel.Save();
                ChangingSample = true;
                selectedSample = value;
                RaisePropertyChanged("SelectedSample");
                LarvaeViewModel.RaisePropertyChanged("CanToggleApprove");
                if (LarvaeViewModel.LarvaeView != null)
                {
                    //AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes = selectedFile.BoundOutcomes ??
                    // new ObservableCollection<Annotation>();
                    //AgeReadingViewModel.AgeReadingEditorViewModel.OriginalMeasureShapes = new ObservableCollection<Line>();
                    //AgeReadingViewModel.AgeReadingEditorViewModel.TextShapes = new ObservableCollection<TextBlock>();
                    //AgeReadingViewModel.AgeReadingView.BrightnessSlider.EditValue = 0;
                    //AgeReadingViewModel.AgeReadingEditorViewModel.Brightness = 0;
                    //AgeReadingViewModel.AgeReadingView.ContrastSlider.EditValue = 0;
                    //AgeReadingViewModel.AgeReadingEditorViewModel.Contrast = 0;
                    //AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = null;
                }


                


                LarvaeViewModel.LarvaeFileViewModel.LarvaeFiles = selectedSample.Files;

                

                if (selectedSample.Annotations != null)
                {
                    foreach (var annotation in selectedSample.Annotations)
                    {
                        annotation.LarvaeQuality = LarvaeViewModel.LarvaeOwnAnnotationViewModel.LarvaeQualities
                            .FirstOrDefault(x => x.ID == annotation.LarvaeQualityID)?.Code;
                    }

                    var ownAnnotation = selectedSample.Annotations.FirstOrDefault(x => x.UserID == Global.API.CurrentUser.ID);
                    LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation = ownAnnotation;

                    LarvaeViewModel.LarvaeAllAnnotationViewModel.Annotations = selectedSample.Annotations;
                    LarvaeViewModel.LarvaeAllAnnotationView.AnnotationList.BestFitColumns();
                }

                ChangingSample = false;

                LarvaeViewModel.LarvaeSampleView.SampleList.BestFitColumns();


                //LarvaeViewModel.LarvaeAn.LarvaeFiles = selectedSample.Files;


            }
        }

        public bool ChangingSample
        {
            get { return changingFile; }
            set { changingFile = value; }
        }

        public void SetDynamicLarvaeSamples()
        {
            DynamicSamples = new ObservableCollection<dynamic>();



            ObservableCollection<dynamic> dynamicSamples = new ObservableCollection<dynamic>();
            List<string> columnNames = new List<string>();
            LarvaeViewModel.LarvaeSampleView.LarvaeSampleGrid.Dispatcher.Invoke(() =>
            {
                List<GridColumn> colsToDelete = new List<GridColumn>();
                foreach (var column in LarvaeViewModel.LarvaeSampleView.LarvaeSampleGrid.Columns.Where(x => x.Tag != null && x.Tag.ToString() == "Dynamic"))
                {
                    colsToDelete.Add(column);
                }

                foreach (var column in colsToDelete)
                {
                    LarvaeViewModel.LarvaeSampleView.LarvaeSampleGrid.Columns.Remove(column);
                }

                foreach (LarvaeSample mSample in LarvaeSamples)
                {

                    dynamic dynFile = new ExpandoObject();
                    dynFile.ID = mSample.ID;
                    dynFile.StatusRank = mSample.StatusRank;
                    dynFile.StatusColor = mSample.StatusColor;
                    dynFile.StatusCode = mSample.StatusCode;
                    dynFile.Status = mSample.Status;

                    if (mSample.SampleProperties != null)
                    {
                        Dictionary<string, string> values = mSample.SampleProperties;
                        columnNames = values.Keys.ToList();
                        foreach (var column in columnNames)
                        {
                            ((IDictionary<String, Object>)dynFile)[column] = values[column];

                        }
                    }

                    dynamicSamples.Add(dynFile);
                }

                List<GridColumn> columns = new List<GridColumn>();

                columns.AddRange(columnNames.Select(columnName => new GridColumn() { FieldName = columnName, AllowSorting = DefaultBoolean.True, Tag = "Dynamic", AllowEditing = DefaultBoolean.False, AllowBestFit = DefaultBoolean.True}));
                foreach (var col in columns)
                {
                    LarvaeViewModel.LarvaeSampleView.LarvaeSampleGrid.Columns.Add(col);
                }

                DynamicSamples = dynamicSamples;


            });
        }

        public void UpdateList()
        {
            LarvaeViewModel.LarvaeSampleView.LarvaeSampleGrid.RefreshData();

        }

        public void SampleList_BeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            //if (AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any() &&
            //    !AgeReadingViewModel.AgeReadingFileViewModel.LoadingFolder &&
            //    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
            //    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation?.QualityID == null &&
            //    !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
            //    )
            //{
            //    //savechecks
            //    if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.EditAnnotation())
            //        //AgeReadingViewModel.SaveAnnotations();
            //        return;
            //}

            //else
            //{
            //    AgeReadingViewModel.SaveAnnotations();
            //}
        }

        public void SampleList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            LoadSample();
        }

        public void LoadSample()
        {
            try
            {
                LarvaeViewModel.ShowWaitSplashScreen();
            }
            catch (Exception ex)
            {
            }

            LarvaeSample sample = SelectedSample;
            if (LarvaeViewModel.LarvaeSampleView.SampleList.FocusedRowData.Row != null)
            {
                sample = LarvaeSamples.FirstOrDefault(x => x.ID == ((dynamic)LarvaeViewModel.LarvaeSampleView.SampleList.FocusedRowData.Row).ID);
            }

            if (sample != null && DynamicSamples.Any())
            {
                var temp = GetLarvaeSample(sample.ID);
                if (temp != null)
                {
                    sample.StatusCode = temp.StatusCode;
                    sample.StatusColor = temp.StatusColor;
                    sample.StatusRank = temp.StatusRank;

                    sample.Files = temp.Files;

                    sample.AnnotationProperties = temp.AnnotationProperties;

                    var dynSample = DynamicSamples.FirstOrDefault(x => x.ID == sample.ID);

                    dynSample.StatusRank = sample.StatusRank;
                    dynSample.StatusColor = sample.StatusColor;
                    dynSample.StatusCode = sample.StatusCode;
                    dynSample.Status = sample.Status;

                    if (temp.SampleProperties != null)
                    {
                        Dictionary<string, string> values = temp.SampleProperties;
                        var columnNames = values.Keys.ToList();
                        foreach (var column in columnNames)
                        {
                            ((IDictionary<string, string>)sample.SampleProperties)[column] = values[column];
                            ((IDictionary<String, Object>)dynSample)[column] = values[column];
                        }
                    }

                    var larvaeFiles = new ObservableCollection<LarvaeFile>();
                    foreach (var file in temp.Files)
                    {
                        larvaeFiles.Add((LarvaeFile)Helper.ConvertType(file, typeof(LarvaeFile)));
                    }
                    sample.Files = larvaeFiles;

                    var larvaeAnnotations = new ObservableCollection<LarvaeAnnotation>();
                    if (temp.Annotations != null)
                    {
                        foreach (var annotation in temp.Annotations)
                        {
                            larvaeAnnotations.Add((LarvaeAnnotation)Helper.ConvertType(annotation, typeof(LarvaeAnnotation)));
                        }
                    }
                    
                    sample.Annotations = larvaeAnnotations;

                    SelectedSample = sample;

                    UpdateList();
                    LarvaeViewModel.LarvaeSampleView.SampleList.BestFitColumns();

                    RefreshNavigationButtons();

                }

                //Helper.DoAsync(SetNextPicture);
                //AgeReadingViewModel.AgeReadingStatusbarViewModel.IsFittingImage = true;
            }

            try
            {
                LarvaeViewModel.CloseSplashScreen();
            }
            catch (Exception ex)
            {
            }
        }

        public LarvaeSample GetLarvaeSample(Guid sampleid)
        {
            var sample = Global.API.GetLarvaeSample(sampleid);
            if (!sample.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Larvae sample from Web API\n" + sample.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
            var sampleResult = (LarvaeSample)Helper.ConvertType(sample.Result, typeof(LarvaeSample));
            sampleResult.ConvertDbFiles(sample.Result.Files.ToList());
            sampleResult.ConvertDbAnnotations(sample.Result.Annotations.ToList());

            return sampleResult;
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

        public void Next(object sender, RoutedEventArgs e)
        {
            LarvaeViewModel.LarvaeSampleView.SampleList.MoveNextRow();
        }

        public void Previous(object sender, RoutedEventArgs e)
        {
            LarvaeViewModel.LarvaeSampleView.SampleList.MovePrevRow();
        }

        public void RefreshNavigationButtons()
        {
            var index = LarvaeViewModel.LarvaeSampleView.SampleList.FocusedRowHandle;
            if (index <= 0)
            {
                LarvaeViewModel.LarvaeView.Previous.IsEnabled = false;
            }
            else
            {
                LarvaeViewModel.LarvaeView.Previous.IsEnabled = true;
            }

            if (index >= DynamicSamples.Count - 1)
            {
                LarvaeViewModel.LarvaeView.Next.IsEnabled = false;
            }
            else
            {
                LarvaeViewModel.LarvaeView.Next.IsEnabled = true;
            }
        }

    }
}
