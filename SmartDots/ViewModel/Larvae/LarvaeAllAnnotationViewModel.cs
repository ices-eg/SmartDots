using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using SmartDots.Helpers;
using SmartDots.Model;

namespace SmartDots.ViewModel
{
    public class LarvaeAllAnnotationViewModel : LarvaeBaseViewModel
    {
        private ObservableCollection<LarvaeAnnotation> larvaeAnnotations;
        private ObservableCollection<LarvaeAnnotationParameterResult> larvaeAnnotationParameterResult;
        private ObservableCollection<LarvaeAnnotationParameterResult> selectedLarvaeAnnotationParameterResults = new ObservableCollection<LarvaeAnnotationParameterResult>();

        public ObservableCollection<LarvaeAnnotation> Annotations
        {
            get { return larvaeAnnotations; }
            set
            {
                larvaeAnnotations = value;

                RaisePropertyChanged("Annotations");
                LarvaeViewModel.LarvaeAllAnnotationView.AnnotationOverviewList.BestFitColumns();

            }
        }

        public ObservableCollection<LarvaeAnnotationParameterResult> LarvaeAnnotationParameterResult
        {
            get { return larvaeAnnotationParameterResult; }
            set
            {
                larvaeAnnotationParameterResult = value;

                //ObservableCollection<LarvaeAnnotationParameterResult> selected = new ObservableCollection<LarvaeAnnotationParameterResult>();

                foreach (var lapr in larvaeAnnotationParameterResult)
                {
                    if (lapr.Annotation == null)
                    {
                        lapr.Annotation =
                            LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.Annotations.FirstOrDefault(x =>
                                x.ID == lapr.LarvaeAnnotationID);
                    }

                    var l = LarvaeViewModel.LarvaeOwnAnnotationViewModel?.Annotation?.LarvaeAnnotationParameterResult
                        ?.FirstOrDefault(x => x.ID == lapr.ID);
                    
                    if (lapr.CalculatedResult == null) LarvaeViewModel.LarvaeEditorViewModel.CalculateResult(lapr);


                    if (lapr.Annotation.UserID != null)
                    {
                        if (!Helper.MultiUserColorsDict.ContainsKey((Guid)lapr.Annotation.UserID))
                        {
                            Helper.MultiUserColorsDict.Add((Guid)lapr.Annotation.UserID, Helper.MultiUserColors.FirstOrDefault(x => !Helper.MultiUserColorsDict.Select(y => y.Value).Contains(x)));
                        }

                        lapr.MultiUserColor = Helper.MultiUserColorsDict[lapr.Annotation.UserID];
                    }
                }


                Annotations = new ObservableCollection<LarvaeAnnotation>(larvaeAnnotationParameterResult.Select(x => x.Annotation).Distinct());

                RaisePropertyChanged("LarvaeAnnotationParameterResult");

            }
        }

        public ObservableCollection<LarvaeAnnotationParameterResult> SelectedLarvaeAnnotationParameterResults
        {
            get
            {
                return selectedLarvaeAnnotationParameterResults;
            }
            set
            {
                selectedLarvaeAnnotationParameterResults = value;

                RaisePropertyChanged("SelectedLarvaeAnnotationParameterResults");
            }
        }

        public bool IsLarvaeAnalysis
        {
            get
            {
                return LarvaeViewModel != null && (bool)(LarvaeViewModel?.LarvaeAnalysis.Type.ToLower().Contains("lar"));
            }
        }

        public bool IsEggAnalysis
        {
            get
            {
                return LarvaeViewModel != null && (bool)(LarvaeViewModel?.LarvaeAnalysis.Type.ToLower().Contains("egg"));
            }
        }
    }
}
