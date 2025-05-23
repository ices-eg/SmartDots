﻿using System;
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
    public class MaturitySampleViewModel : MaturityBaseViewModel
    {
        private ObservableCollection<dynamic> dynamicSamples;
        private MaturitySample selectedSample;
        private bool changingFile;


        public MaturitySampleViewModel()
        {
        }

        public List<MaturitySample> MaturitySamples { get; set; }


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
                        SelectedSample = MaturitySamples.FirstOrDefault(x => x.ID == dynamicSamples[0].ID);
                        LoadSample();
                    }

                }

                RaisePropertyChanged("DynamicSamples");


            }
        }

        public MaturitySample SelectedSample
        {
            get { return selectedSample; }
            set
            {
                MaturityViewModel.Save();
                ChangingSample = true;
                selectedSample = value;
                RaisePropertyChanged("SelectedSample");
                MaturityViewModel.RaisePropertyChanged("CanToggleApprove");
                if (MaturityViewModel.MaturityView != null)
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





                MaturityViewModel.MaturityFileViewModel.MaturityFiles = selectedSample.Files;



                if (selectedSample.Annotations != null)
                {
                    foreach (var annotation in selectedSample.Annotations)
                    {
                        annotation.Maturity = MaturityViewModel.MaturityOwnAnnotationViewModel.Maturities
                            .FirstOrDefault(x => x.ID == annotation.MaturityID)?.Code;
                        annotation.Sex = MaturityViewModel.MaturityOwnAnnotationViewModel.MaturitySexes
                            .FirstOrDefault(x => x.ID == annotation.SexID)?.Code;
                        annotation.MaturityQuality = MaturityViewModel.MaturityOwnAnnotationViewModel.MaturityQualities
                            .FirstOrDefault(x => x.ID == annotation.MaturityQualityID)?.Code;
                    }

                    var ownAnnotation = selectedSample.Annotations.FirstOrDefault(x => x.UserID == Global.API.CurrentUser.ID);
                    MaturityViewModel.MaturityOwnAnnotationViewModel.Annotation = ownAnnotation;

                    MaturityViewModel.MaturityAllAnnotationViewModel.Annotations = selectedSample.Annotations;
                    MaturityViewModel.MaturityAllAnnotationView.AnnotationList.BestFitColumns();
                }

                ChangingSample = false;

                MaturityViewModel.MaturitySampleView.SampleList.BestFitColumns();


                //MaturityViewModel.MaturityAn.MaturityFiles = selectedSample.Files;


            }
        }

        public bool ChangingSample
        {
            get { return changingFile; }
            set { changingFile = value; }
        }

        public void SetDynamicMaturitySamples()
        {
            DynamicSamples = new ObservableCollection<dynamic>();



            ObservableCollection<dynamic> dynamicSamples = new ObservableCollection<dynamic>();
            List<string> columnNames = new List<string>();
            MaturityViewModel.MaturitySampleView.MaturitySampleGrid.Dispatcher.Invoke(() =>
            {
                List<GridColumn> colsToDelete = new List<GridColumn>();
                foreach (var column in MaturityViewModel.MaturitySampleView.MaturitySampleGrid.Columns.Where(x => x.Tag != null && x.Tag.ToString() == "Dynamic"))
                {
                    colsToDelete.Add(column);
                }

                foreach (var column in colsToDelete)
                {
                    MaturityViewModel.MaturitySampleView.MaturitySampleGrid.Columns.Remove(column);
                }

                foreach (MaturitySample mSample in MaturitySamples)
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

                columns.AddRange(columnNames.Select(columnName => new GridColumn() { FieldName = columnName, AllowSorting = DefaultBoolean.True, Tag = "Dynamic", AllowEditing = DefaultBoolean.False, AllowBestFit = DefaultBoolean.True }));
                foreach (var col in columns)
                {
                    MaturityViewModel.MaturitySampleView.MaturitySampleGrid.Columns.Add(col);
                }

                DynamicSamples = dynamicSamples;


            });
        }

        public void UpdateList()
        {
            MaturityViewModel.MaturitySampleView.MaturitySampleGrid.RefreshData();

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
                MaturityViewModel.ShowWaitSplashScreen();
            }
            catch (Exception ex)
            {
            }

            MaturitySample sample = SelectedSample;
            if (MaturityViewModel.MaturitySampleView.SampleList.FocusedRowData.Row != null)
            {
                sample = MaturitySamples.FirstOrDefault(x => x.ID == ((dynamic)MaturityViewModel.MaturitySampleView.SampleList.FocusedRowData.Row).ID);
            }

            if (sample != null && DynamicSamples.Any())
            {
                var temp = GetMaturitySample(sample.ID);
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

                    var maturityFiles = new ObservableCollection<MaturityFile>();
                    foreach (var file in temp.Files)
                    {
                        maturityFiles.Add((MaturityFile)Helper.ConvertType(file, typeof(MaturityFile)));
                    }
                    sample.Files = maturityFiles;

                    var maturityAnnotations = new ObservableCollection<MaturityAnnotation>();
                    if (temp.Annotations != null)
                    {
                        foreach (var annotation in temp.Annotations)
                        {
                            maturityAnnotations.Add((MaturityAnnotation)Helper.ConvertType(annotation, typeof(MaturityAnnotation)));
                        }
                    }

                    sample.Annotations = maturityAnnotations;

                    SelectedSample = sample;

                    UpdateList();
                    MaturityViewModel.MaturitySampleView.SampleList.BestFitColumns();

                    RefreshNavigationButtons();

                }

                //Helper.DoAsync(SetNextPicture);
                //AgeReadingViewModel.AgeReadingStatusbarViewModel.IsFittingImage = true;
            }

            try
            {
                MaturityViewModel.CloseSplashScreen();
            }
            catch (Exception ex)
            {
            }
        }

        public MaturitySample GetMaturitySample(Guid sampleid)
        {
            var sample = Global.API.GetMaturitySample(sampleid);
            if (!sample.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading Maturity sample from Web API\n" + sample.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
            var sampleResult = (MaturitySample)Helper.ConvertType(sample.Result, typeof(MaturitySample));
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
            MaturityViewModel.MaturitySampleView.SampleList.MoveNextRow();
        }

        public void Previous(object sender, RoutedEventArgs e)
        {
            MaturityViewModel.MaturitySampleView.SampleList.MovePrevRow();
        }

        public void RefreshNavigationButtons()
        {
            var index = MaturityViewModel.MaturitySampleView.SampleList.FocusedRowHandle;
            if (index <= 0)
            {
                MaturityViewModel.MaturityView.Previous.IsEnabled = false;
            }
            else
            {
                MaturityViewModel.MaturityView.Previous.IsEnabled = true;
            }

            if (index >= DynamicSamples.Count - 1)
            {
                MaturityViewModel.MaturityView.Next.IsEnabled = false;
            }
            else
            {
                MaturityViewModel.MaturityView.Next.IsEnabled = true;
            }
        }

        public void DownloadImages()
        {
            try
            {
                if (MaturitySamples.Count == 1 || MaturitySamples.Any(x => x.ID != SelectedSample.ID && (x.Files != null && x.Files.Count > 0)))
                {
                    var images = MaturitySamples
                    .Where(x => x?.Files != null)
                    .SelectMany(x => x.Files)
                    .Where(x => x.Path != null)
                    .Select(x => x.Path)
                    .ToList();

                    var result = Helper.DownloadImages(MaturityViewModel.MaturityAnalysis.ID, images);

                    if (!string.IsNullOrEmpty(result))
                    {
                        MaturityViewModel.MaturityView.MainWindowViewModel.ShowSuccessToast("Image Download", result);
                    }
                }
                else
                {
                    Helper.ShowWinUIMessageBox("Preloading of images is currently not supported.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }



    }
}
