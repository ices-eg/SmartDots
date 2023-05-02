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
    public class MaturityOwnAnnotationViewModel : MaturityBaseViewModel
    {
        private MaturityAnnotation annotation;
        private string propertyValues;
        private List<MaturityQuality> maturityQualities = new List<MaturityQuality>();
        //private MaturityQuality maturityQuality;
        private List<MaturitySex> maturitySexes = new List<MaturitySex>();
        private MaturitySex maturitySex;
        private List<Maturity> maturities = new List<Maturity>();
        private Maturity maturity;


        public MaturityAnnotation Annotation
        {
            get { return annotation; }
            set
            {
                EmptyValues();
                annotation = value;
                GetAllPropertiesWithValues();


                RaisePropertyChanged("Annotation");
                RaisePropertyChanged("MaturityQuality");
                RaisePropertyChanged("MaturitySex");
                RaisePropertyChanged("Maturity");
                RaisePropertyChanged("Comment");
                RaisePropertyChanged("CanEdit");
                MaturityViewModel.Refresh();
            }
        }

        public bool CanEdit
        {
            get
            {
                if (MaturityViewModel != null)
                {
                    return !MaturityViewModel.MaturitySampleViewModel.SelectedSample.IsReadOnly;
                }

                return false;
            }
        }

        public List<MaturityQuality> MaturityQualities
        {
            get { return maturityQualities; }
            set
            {
                maturityQualities = value;
                RaisePropertyChanged("MaturityQualities");
            }
        }

        public MaturityQuality MaturityQuality
        {
            get { return MaturityQualities.FirstOrDefault(x => x.ID == Annotation?.MaturityQualityID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.MaturityQualityID = MaturityQualities.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.MaturityQuality = MaturityQualities.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("MaturityQuality");
            }
        }

        public List<MaturitySex> MaturitySexes
        {
            get { return maturitySexes; }
            set
            {
                maturitySexes = value;
                RaisePropertyChanged("MaturitySexes");
            }
        }

        public MaturitySex MaturitySex
        {
            get { return MaturitySexes.FirstOrDefault(x => x.ID == Annotation?.SexID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.SexID = MaturitySexes.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.Sex = MaturitySexes.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("MaturitySex");
            }
        }

        public List<Maturity> Maturities
        {
            get { return maturities; }
            set
            {
                maturities = value;
                RaisePropertyChanged("Maturities");
            }
        }

        public Maturity Maturity
        {
            get { return Maturities.FirstOrDefault(x => x.ID == Annotation?.MaturityID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.MaturityID = Maturities.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.Maturity = Maturities.FirstOrDefault(x => x.ID == value?.ID)?.Code;

                RaisePropertyChanged("Maturity");
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

            MaturityViewModel.MaturityOwnAnnotationView.Grid.Dispatcher.Invoke(() =>
            {
                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Clear();
                int rowHeight = 24;
                int startpos = 0;
                if (MaturityViewModel.MaturitySampleViewModel.SelectedSample.AnnotationProperties != null)
                {
                    foreach (KeyValuePair<string, string> pair in MaturityViewModel.MaturitySampleViewModel.SelectedSample.AnnotationProperties)
                    {
                        MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(rowHeight)
                        });

                        Label key = new Label() { Content = pair.Key, Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                        Label value = new Label() { Content = pair.Value, Padding = new Thickness(0) };

                        Grid.SetColumn(key, 0);
                        Grid.SetRow(key, startpos);

                        Grid.SetColumn(value, 1);
                        Grid.SetRow(value, startpos);
                        MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(key);
                        MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(value);
                        startpos++;
                    }

                }
                //if (Annotation != null)
                //{
                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                MaturityViewModel.MaturityOwnAnnotationView.MaturitySexesLookup.SetValue(Grid.RowProperty, startpos);

                StackPanel sp = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, FlowDirection = FlowDirection.LeftToRight, Orientation = Orientation.Horizontal };
                Label sexKey = new Label() { Content = "Sex", Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                Label sRequired = new Label() { Content = "*", Padding = new Thickness(0), Margin = new Thickness(2, 0, 0, 0), FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)), ToolTip = "Required for approval" };
                sp.Children.Add(sexKey);
                sp.Children.Add(sRequired);


                Grid.SetColumn(sp, 0);
                Grid.SetRow(sp, startpos);

                MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(sp);
                startpos++;

                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                MaturityViewModel.MaturityOwnAnnotationView.MaturitiesLookup.SetValue(Grid.RowProperty, startpos);

                StackPanel mp = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, FlowDirection = FlowDirection.LeftToRight, Orientation = Orientation.Horizontal };
                Label maturityKey = new Label() { Content = "Maturity", Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                Label mRequired = new Label() { Content = "*", Padding = new Thickness(0), Margin = new Thickness(2, 0, 0, 0), FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)), ToolTip = "Required for approval" };
                mp.Children.Add(maturityKey);
                mp.Children.Add(mRequired);

                Grid.SetColumn(mp, 0);
                Grid.SetRow(mp, startpos);

                MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(mp);
                startpos++;


                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight)
                });
                MaturityViewModel.MaturityOwnAnnotationView.MaturityQualitiesLookup.SetValue(Grid.RowProperty, startpos);

                StackPanel qp = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, FlowDirection = FlowDirection.LeftToRight, Orientation = Orientation.Horizontal };
                Label qualityKey = new Label() { Content = "Quality", Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                Label qRequired = new Label() { Content = "*", Padding = new Thickness(0), Margin = new Thickness(2, 0, 0, 0), FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)), ToolTip = "Required for approval" };
                qp.Children.Add(qualityKey);
                qp.Children.Add(qRequired);

                Grid.SetColumn(qp, 0);
                Grid.SetRow(qp, startpos);

                MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(qp);
                startpos++;

                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(rowHeight * 3)
                });
                MaturityViewModel.MaturityOwnAnnotationView.MaturityComments.SetValue(Grid.RowProperty, startpos);

                Label commentsKey = new Label() { Content = "Comments", Padding = new Thickness(0), FontWeight = FontWeights.Bold };

                Grid.SetColumn(commentsKey, 0);
                Grid.SetRow(commentsKey, startpos);

                MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Add(commentsKey);
                startpos++;

                MaturityViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(28)
                });
                //}
            });
        }

        public void EmptyValues()
        {
            MaturityViewModel.MaturityOwnAnnotationView.Grid.Dispatcher.Invoke(() =>
            {
                foreach (var label in MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.OfType<Label>().ToList())
                {
                    MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Remove(label);
                }
                foreach (var sp in MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.OfType<StackPanel>().ToList())
                {
                    MaturityViewModel.MaturityOwnAnnotationView.Grid.Children.Remove(sp);
                }
            });
        }

        public void CreateNewAnnotation()
        {
            if (Annotation == null)
            {
                Annotation = new MaturityAnnotation()
                {
                    ID = Guid.NewGuid(),
                    Date = DateTime.Now,
                    MaturitySampleID = MaturityViewModel.MaturitySampleViewModel.SelectedSample.ID,
                    UserID = Global.API.CurrentUser.ID,
                    User = Global.API.CurrentUser.AccountName
                };
            }
        }

        public void SaveAnnotation()
        {
            try
            {
                MaturityViewModel.ShowWaitSplashScreen();

                DtoMaturityAnnotation dtoMaturityAnnotation = (DtoMaturityAnnotation)Helper.ConvertType(Annotation, typeof(DtoMaturityAnnotation));
                var savennotationResult = Global.API.SaveMaturityAnnotation(dtoMaturityAnnotation);
                if (!savennotationResult.Succeeded)
                {
                    MaturityViewModel.CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Error Saving Maturity annotation\n" + savennotationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Annotation.RequiresSaving = false;

                if (savennotationResult.Result != null)
                {
                    var maturitySample = (MaturitySample)Helper.ConvertType(savennotationResult.Result, typeof(MaturitySample));

                    var sample = MaturityViewModel.MaturitySampleViewModel.SelectedSample;

                    sample.StatusCode = maturitySample.StatusCode;
                    sample.StatusColor = maturitySample.StatusColor;
                    sample.StatusRank = maturitySample.StatusRank;
                    sample.IsReadOnly = maturitySample.IsReadOnly;
                    sample.UserHasApproved = maturitySample.UserHasApproved;

                    var dynSample = MaturityViewModel.MaturitySampleViewModel.DynamicSamples.FirstOrDefault(x => x.ID == sample.ID);

                    dynSample.StatusRank = sample.StatusRank;
                    dynSample.StatusColor = sample.StatusColor;
                    dynSample.StatusCode = sample.StatusCode;
                    dynSample.Status = sample.Status;
                    dynSample.IsReadOnly = sample.IsReadOnly;
                    sample.UserHasApproved = maturitySample.UserHasApproved;

                    MaturityViewModel.MaturitySampleViewModel.UpdateList();

                    maturitySample.ConvertDbAnnotations(savennotationResult.Result.Annotations.ToList());
                    foreach (var an in maturitySample.Annotations)
                    {
                        an.Maturity = Maturities
                            .FirstOrDefault(x => x.ID == an.MaturityID)?.Code;
                        an.Sex = MaturitySexes
                            .FirstOrDefault(x => x.ID == an.SexID)?.Code;
                        an.MaturityQuality = MaturityQualities
                            .FirstOrDefault(x => x.ID == an.MaturityQualityID)?.Code;
                    }

                    MaturityViewModel.MaturityAllAnnotationViewModel.Annotations = maturitySample.Annotations;
                    MaturityViewModel.MaturityAllAnnotationView.AnnotationList.BestFitColumns();

                }

                RaisePropertyChanged("CanEdit");
                MaturityViewModel.RaisePropertyChanged("CanToggleApprove");
                MaturityViewModel.Refresh();
            }
            catch (Exception e)
            {
                MaturityViewModel.CloseSplashScreen();
                Console.WriteLine(e);
            }
            MaturityViewModel.CloseSplashScreen();
            

            if (MaturityViewModel.MaturitySampleViewModel.MaturitySamples.All(x => x.UserHasApproved))
            {
                MaturityViewModel.PromptAnalysisCompleteDialog();
            }
        }

        public void MaturityLookup_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (!MaturityViewModel.MaturitySampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }

        }

        public void ToggleApprove()
        {
            if (Annotation == null || (Annotation.MaturityID == null || Annotation.SexID == null || Annotation.MaturityQualityID == null ||
                                       Annotation.MaturityID == Guid.Empty || Annotation.SexID == Guid.Empty || Annotation.MaturityQualityID == Guid.Empty
                ))
            {
                Helper.ShowWinUIMessageBox("Error toggling approved state for annotation\n" + "Please make sure to fill in the required fields first: Sex, Maturity, Quality", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Annotation.IsApproved = !Annotation.IsApproved;
            if (!MaturityViewModel.MaturitySampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }
        }
    }
}
