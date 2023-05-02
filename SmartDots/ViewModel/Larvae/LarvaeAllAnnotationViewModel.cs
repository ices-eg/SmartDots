using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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
        private ObservableCollection<dynamic> dynamicAnnotations;
        private ObservableCollection<LarvaeAnnotationParameterResult> larvaeAnnotationParameterResult;
        private ObservableCollection<LarvaeAnnotationParameterResult> selectedLarvaeAnnotationParameterResults = new ObservableCollection<LarvaeAnnotationParameterResult>();

        public ObservableCollection<LarvaeAnnotation> Annotations
        {
            get { return larvaeAnnotations; }
            set
            {
                larvaeAnnotations = value;

                RaisePropertyChanged("Annotations");

                ObservableCollection<dynamic> dynAnnotations = new ObservableCollection<dynamic>();
                foreach (var an in larvaeAnnotations)
                {
                    dynamic dynAnnotation = new ExpandoObject();
                    dynAnnotation.User = an.User;
                    dynAnnotation.Date = an.Date.ToString("yyyy-MM-dd");
                    foreach (var prop in LarvaeViewModel.LarvaeOwnAnnotationViewModel.Dict)
                    {
                        if (an.PropertyValues.ContainsKey(prop.Key.Property))
                        {
                            ((IDictionary<String, Object>)dynAnnotation)[prop.Key.Label] =
                                prop.Value.FirstOrDefault(x => x.ID == an.PropertyValues[prop.Key.Property])?.Code;
                        }
                        else
                        {
                            ((IDictionary<String, Object>)dynAnnotation)[prop.Key.Label] = "";
                        }
                    }
                    dynAnnotation.Comment = an.Comments;
                    //dynAnnotation.Approved = an.IsApproved;
                    //if (an.IsApproved)
                    //{
                    //    ((IDictionary<String, Object>)dynAnnotation)["ApprovedPicture"] =  "/SmartDots;component/Resources/ok-16.png";
                    //}
                    //else
                    //{
                    //    ((IDictionary<String, Object>)dynAnnotation)["ApprovedPicture"] = "";
                    //}

                    dynAnnotation.Approved = an.IsApproved ? "Yes" : "No";

                    dynAnnotations.Add(dynAnnotation);

                }

                DynamicAnnotations = dynAnnotations;
                LarvaeAnnotationParameterResult = new ObservableCollection<LarvaeAnnotationParameterResult>(Annotations.SelectMany(x => x.LarvaeAnnotationParameterResult));
            }
        }

        public ObservableCollection<dynamic> DynamicAnnotations
        {
            get { return dynamicAnnotations; }
            set
            {
                dynamicAnnotations = value;

                RaisePropertyChanged("DynamicAnnotations");
                LarvaeViewModel.LarvaeAllAnnotationView.AnnotationOverviewList.BestFitColumns();

            }
        }

        public ObservableCollection<LarvaeAnnotationParameterResult> LarvaeAnnotationParameterResult
        {
            get { return larvaeAnnotationParameterResult; }
            set
            {
                larvaeAnnotationParameterResult = value;

                foreach (var lapr in larvaeAnnotationParameterResult)
                {
                    if (lapr.Annotation == null)
                    {
                        lapr.Annotation =
                            LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.Annotations.FirstOrDefault(x =>
                                x.ID == lapr.AnnotationID);
                    }

                    if (lapr.File == null)
                    {
                        lapr.File =
                            LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.Files.FirstOrDefault(x =>
                                x.ID == lapr.FileID);
                    }

                    //var l = LarvaeViewModel.LarvaeOwnAnnotationViewModel?.Annotation?.LarvaeAnnotationParameterResult
                    //    ?.FirstOrDefault(x => x.ID == lapr.ID);
                    
                    if (lapr.CalculatedResult == null) LarvaeViewModel.LarvaeEditorViewModel.CalculateResult(lapr);


                    if (lapr.Annotation != null && lapr.Annotation.UserID != null)
                    {
                        if (!Helper.MultiUserColorsDict.ContainsKey((Guid)lapr.Annotation.UserID))
                        {
                            Helper.MultiUserColorsDict.Add((Guid)lapr.Annotation.UserID, Helper.MultiUserColors.FirstOrDefault(x => !Helper.MultiUserColorsDict.Select(y => y.Value).Contains(x)));
                        }

                        lapr.MultiUserColor = Helper.MultiUserColorsDict[lapr.Annotation.UserID];
                    }
                }

                //Annotations = new ObservableCollection<LarvaeAnnotation>(larvaeAnnotationParameterResult.Select(x => x.Annotation).Distinct());

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

        
    }
}
