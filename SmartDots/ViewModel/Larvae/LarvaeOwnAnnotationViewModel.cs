using System;
using System.Collections.Generic;
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
    public class LarvaeOwnAnnotationViewModel : LarvaeBaseViewModel
    {
        private LarvaeAnnotation annotation;
        private string propertyValues;
        private List<LarvaeQuality> larvaeQualities = new List<LarvaeQuality>();
        //private LarvaeQuality larvaeQuality;


        public LarvaeAnnotation Annotation
        {
            get { return annotation; }
            set
            {
                EmptyValues();
                annotation = value;
                GetAllPropertiesWithValues();


                RaisePropertyChanged("Annotation");
                RaisePropertyChanged("LarvaeQuality");
                RaisePropertyChanged("LarvaeSex");
                RaisePropertyChanged("Larvae");
                RaisePropertyChanged("Comment");
                RaisePropertyChanged("CanEdit");
                LarvaeViewModel.Refresh();
            }
        }

        public bool CanEdit
        {
            get
            {
                if (LarvaeViewModel != null)
                {
                    return !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly;
                }

                return false;
            }
        }

        public List<LarvaeQuality> LarvaeQualities
        {
            get { return larvaeQualities; }
            set
            {
                larvaeQualities = value;
                RaisePropertyChanged("LarvaeQualities");
            }
        }

        public LarvaeQuality LarvaeQuality
        {
            get { return LarvaeQualities.FirstOrDefault(x => x.ID == Annotation?.LarvaeQualityID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.LarvaeQualityID = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.LarvaeQuality = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("LarvaeQuality");
            }
        }

        public string Comment
        {
            get { return Annotation?.Comments; }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.Comments = value;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("Comment");
            }
        }

        public string PropertyValues
        {
            get { return propertyValues; }
            set
            {
                propertyValues = value;
                RaisePropertyChanged("PropertyValues");
            }
        }

        public void GetAllPropertiesWithValues()
        {

            LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Dispatcher.Invoke(() =>
            {
                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Clear();
                int rowHeight = 24;
                int startpos = 0;
                if (LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.AnnotationProperties != null)
                {
                    foreach (KeyValuePair<string, string> pair in LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.AnnotationProperties)
                    {
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(rowHeight)
                        });

                        Label key = new Label() { Content = pair.Key, Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                        Label value = new Label() { Content = pair.Value, Padding = new Thickness(0) };

                        Grid.SetColumn(key, 0);
                        Grid.SetRow(key, startpos);

                        Grid.SetColumn(value, 1);
                        Grid.SetRow(value, startpos);
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(key);
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(value);
                        startpos++;
                    }

                }
                //if (Annotation != null)
                //{
                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                LarvaeViewModel.LarvaeOwnAnnotationView.LarvaeSexesLookup.SetValue(Grid.RowProperty, startpos);

                Label sexKey = new Label() { Content = "Sex", Padding = new Thickness(0), FontWeight = FontWeights.Bold };

                Grid.SetColumn(sexKey, 0);
                Grid.SetRow(sexKey, startpos);

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(sexKey);
                startpos++;

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                LarvaeViewModel.LarvaeOwnAnnotationView.MaturitiesLookup.SetValue(Grid.RowProperty, startpos);

                Label larvaeKey = new Label() { Content = "Larvae", Padding = new Thickness(0), FontWeight = FontWeights.Bold };

                Grid.SetColumn(larvaeKey, 0);
                Grid.SetRow(larvaeKey, startpos);

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(larvaeKey);
                startpos++;


                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                LarvaeViewModel.LarvaeOwnAnnotationView.LarvaeQualitiesLookup.SetValue(Grid.RowProperty, startpos);

                Label qualityKey = new Label() { Content = "Quality", Padding = new Thickness(0), FontWeight = FontWeights.Bold };

                Grid.SetColumn(qualityKey, 0);
                Grid.SetRow(qualityKey, startpos);

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(qualityKey);
                startpos++;

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight * 3)
                });
                LarvaeViewModel.LarvaeOwnAnnotationView.LarvaeComments.SetValue(Grid.RowProperty, startpos);

                Label commentsKey = new Label() { Content = "Comments", Padding = new Thickness(0), FontWeight = FontWeights.Bold };

                Grid.SetColumn(commentsKey, 0);
                Grid.SetRow(commentsKey, startpos);

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(commentsKey);
                startpos++;

                LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(28)
                });
                //}
            });
        }

        public void EmptyValues()
        {
            LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Dispatcher.Invoke(() =>
            {
                foreach (var label in LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.OfType<Label>().ToList())
                {
                    LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Remove(label);
                }
            });
        }

        public void CreateNewAnnotation()
        {
            if (Annotation == null)
            {
                Annotation = new LarvaeAnnotation()
                {
                    ID = Guid.NewGuid(),
                    Date = DateTime.Now,
                    LarvaeSampleID = LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.ID,
                    UserID = Global.API.CurrentUser.ID,
                    User = Global.API.CurrentUser.AccountName
                };
            }
        }

        public void SaveAnnotation()
        {
            try
            {
                LarvaeViewModel.ShowWaitSplashScreen();

                DtoLarvaeAnnotation dtoLarvaeAnnotation = (DtoLarvaeAnnotation)Helper.ConvertType(Annotation, typeof(DtoLarvaeAnnotation));
                var savennotationResult = Global.API.SaveLarvaeAnnotation(dtoLarvaeAnnotation);
                if (!savennotationResult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error Saving Larvae annotation\n" + savennotationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LarvaeViewModel.CloseSplashScreen();
                    return;
                }

                Annotation.RequiresSaving = false;

                if (savennotationResult.Result != null)
                {
                    var larvaeSample = (LarvaeSample)Helper.ConvertType(savennotationResult.Result, typeof(LarvaeSample));

                    var sample = LarvaeViewModel.LarvaeSampleViewModel.SelectedSample;

                    sample.StatusCode = larvaeSample.StatusCode;
                    sample.StatusColor = larvaeSample.StatusColor;
                    sample.StatusRank = larvaeSample.StatusRank;
                    sample.IsReadOnly = larvaeSample.IsReadOnly;
                    sample.UserHasApproved = larvaeSample.UserHasApproved;

                    var dynSample = LarvaeViewModel.LarvaeSampleViewModel.DynamicSamples.FirstOrDefault(x => x.ID == sample.ID);

                    dynSample.StatusRank = sample.StatusRank;
                    dynSample.StatusColor = sample.StatusColor;
                    dynSample.StatusCode = sample.StatusCode;
                    dynSample.Status = sample.Status;
                    dynSample.IsReadOnly = sample.IsReadOnly;
                    sample.UserHasApproved = larvaeSample.UserHasApproved;

                    LarvaeViewModel.LarvaeSampleViewModel.UpdateList();

                    larvaeSample.ConvertDbAnnotations(savennotationResult.Result.Annotations.ToList());
                    foreach (var an in larvaeSample.Annotations)
                    {
                        
                        an.LarvaeQuality = LarvaeQualities
                            .FirstOrDefault(x => x.ID == an.LarvaeQualityID)?.Code;
                    }

                    LarvaeViewModel.LarvaeAllAnnotationViewModel.Annotations = larvaeSample.Annotations;
                    LarvaeViewModel.LarvaeAllAnnotationView.AnnotationList.BestFitColumns();

                }

                RaisePropertyChanged("CanEdit");
                LarvaeViewModel.RaisePropertyChanged("CanToggleApprove");
                LarvaeViewModel.Refresh();
            }
            catch (Exception e)
            {
                LarvaeViewModel.CloseSplashScreen();
                Console.WriteLine(e);
            }
            LarvaeViewModel.CloseSplashScreen();
            

            if (LarvaeViewModel.LarvaeSampleViewModel.LarvaeSamples.All(x => x.UserHasApproved))
            {
                LarvaeViewModel.PromptAnalysisCompleteDialog();
            }
        }

        public void LarvaeLookup_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }

        }

        public void ToggleApprove()
        {
            if (Annotation == null || (Annotation.LarvaeID == null || Annotation.SexID == null || Annotation.LarvaeQualityID == null ||
                                       Annotation.LarvaeID == Guid.Empty || Annotation.SexID == Guid.Empty || Annotation.LarvaeQualityID == Guid.Empty
                ))
            {
                Helper.ShowWinUIMessageBox("Error toggling approved state for annotation\n" + "Please make sure to fill in the required fields first: Sex, Larvae, Quality", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Annotation.IsApproved = !Annotation.IsApproved;
            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }
        }
    }
}
