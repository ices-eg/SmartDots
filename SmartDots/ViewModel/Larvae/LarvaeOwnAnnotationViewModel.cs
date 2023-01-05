using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp;
using SmartDots.Helpers;
using SmartDots.Model;

namespace SmartDots.ViewModel
{
    public class LarvaeOwnAnnotationViewModel : LarvaeBaseViewModel
    {
        private LarvaeAnnotation annotation;
        private List<LarvaeAnnotationParameterResult> selectedParameters = new List<LarvaeAnnotationParameterResult>();
        private string propertyValues;
        private List<LarvaeQuality> larvaeQualities = new List<LarvaeQuality>();
        private List<LarvaeDevelopmentStage> larvaeDevelopmentStages = new List<LarvaeDevelopmentStage>();
        private List<LarvaePresence> larvaePresences = new List<LarvaePresence>();
        //private LarvaeQuality larvaeQuality;


        public LarvaeAnnotation Annotation
        {
            get { return annotation; }
            set
            {
                annotation = value;

                if (annotation != null)
                {
                    foreach (var param in LarvaeViewModel.LarvaeAnalysis.LarvaeParameters)
                    {
                        if (!annotation.LarvaeAnnotationParameterResult.Any(x => x.LarvaeParameterID == param.ID))
                        {
                            annotation.LarvaeAnnotationParameterResult.Add(new LarvaeAnnotationParameterResult()
                            {
                                ID = Guid.NewGuid(),
                                LarvaeParameterID = param.ID,
                                Parameter = LarvaeViewModel.LarvaeAnalysis.LarvaeParameters.FirstOrDefault(x => x.ID == param.ID),
                                LarvaeAnnotationID = annotation.ID,
                                Lines = new List<LarvaeLine>(),
                                Dots = new List<LarvaeDot>()
                            });
                        }
                    }
                }

                


                RaisePropertyChanged("Annotation");
                RaisePropertyChanged("LarvaeQuality");
                RaisePropertyChanged("LarvaeDevelopmentStage");
                RaisePropertyChanged("AnalFinPresence");
                RaisePropertyChanged("DorsalFinPresence");
                RaisePropertyChanged("PelvicFinPresence");
                RaisePropertyChanged("Comment");
                RaisePropertyChanged("CanEdit");
                LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();

                LarvaeViewModel.Refresh();
            }
        }

        public List<LarvaeAnnotationParameterResult> SelectedParameters
        {
            get
            {
                return selectedParameters;
            }
            set
            {
                selectedParameters = value;
                
                RaisePropertyChanged("SelectedParameters");
                UpdateSelectionMode();

            }
        }

        public void UpdateSelectionMode()
        {
            if (selectedParameters != null && selectedParameters.Count == 1)
            {
                if (selectedParameters[0].Parameter.ShapeType.ToLower() == "dot")
                {
                    LarvaeViewModel.LarvaeEditorViewModel.Mode = EditorModeEnum.DrawDot;
                }
                else if (selectedParameters[0].Parameter.ShapeType.ToLower() == "line")
                {
                    LarvaeViewModel.LarvaeEditorViewModel.Mode = EditorModeEnum.DrawLine;
                }
            }
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();
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

        public List<LarvaeDevelopmentStage> LarvaeDevelopmentStages
        {
            get { return larvaeDevelopmentStages; }
            set
            {
                larvaeDevelopmentStages = value;
                RaisePropertyChanged("LarvaeDevelopmentStages");
            }
        }

        public List<LarvaePresence> LarvaePresences
        {
            get { return larvaePresences; }
            set
            {
                larvaePresences = value;
                RaisePropertyChanged("LarvaePresences");
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

        public LarvaeDevelopmentStage LarvaeDevelopmentStage
        {
            get { return LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == Annotation?.DevelopmentStageID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.DevelopmentStageID = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.DevelopmentStage = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("LarvaeDevelopmentStage");
            }
        }

        public LarvaePresence AnalFinPresence
        {
            get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.AnalFinPresenceID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.AnalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.AnalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("AnalFinPresence");
            }
        }

        public LarvaePresence DorsalFinPresence
        {
            get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.DorsalFinPresenceID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.DorsalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.DorsalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("DorsalFinPresence");
            }
        }

        public LarvaePresence PelvicFinPresence
        {
            get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.PelvicFinPresenceID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                Annotation.PelvicFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.PelvicFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                RaisePropertyChanged("PelvicFinPresence");
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
            if (Annotation == null || (Annotation.LarvaeQualityID == null || Annotation.LarvaeQualityID == Guid.Empty
                ))
            {
                Helper.ShowWinUIMessageBox("Error toggling approved state for annotation\n" + "Please make sure to fill in the required fields first: Anal, Dorsal and Pelvic fin presences, Development stage, Quality", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Annotation.IsApproved = !Annotation.IsApproved;
            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }
        }

        public void AnnotateRadio_Change(object sender, RoutedEventArgs e)
        {
            var radio = ((RadioButton) e.OriginalSource);
            if (radio.Name == "AnnotateRadio" && radio.IsChecked == true)
            {
                // code to annotate
                LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.SelectionMode = MultiSelectMode.None;
            }
            else if (radio.Name == "ShowRadio" && radio.IsChecked == true)
            {
                // code to show annotations
                LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.SelectionMode = MultiSelectMode.MultipleRow;
            }
        }

        public void GridControl_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = null;
            //AgeReadingViewModel.AgeReadingAnnotationView.CreatorColumn.Visible = true;
            //AgeReadingViewModel.AgeReadingAnnotationView.CreatorColorColumn.Visible = false;
            //AgeReadingViewModel.AgeReadingAnnotationView.CreatorColorColumn.VisibleIndex = 2;

            //if (SelectedAnnotations.Count == 1)
            //{
            //    WorkingAnnotation = SelectedAnnotations[0];

            //    foreach (var combinedline in WorkingAnnotation.CombinedLines)
            //    {
            //        combinedline.RecalculatePoints();
            //        combinedline.CalculateDotIndices();
            //    }
            //    try
            //    {
            //        AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = WorkingAnnotation?.CombinedLines.FirstOrDefault();
            //    }
            //    catch (Exception)
            //    {
            //        // ignored
            //    }
            //}
            //else
            //{
            //    WorkingAnnotation = null;

            //    if (SelectedAnnotations.Count > 1)
            //    {

            //        if (Global.API.Settings != null && !Global.API.Settings.IgnoreMultiUserColor)
            //        {
            //            AgeReadingViewModel.AgeReadingAnnotationView.CreatorColumn.Visible = false;
            //            AgeReadingViewModel.AgeReadingAnnotationView.CreatorColorColumn.Visible = true;
            //            foreach (var annotation in Outcomes.Where(x => string.IsNullOrEmpty(x.MultiUserColor)))
            //            {
            //                if (annotation.LabTechnicianID != null)
            //                {
            //                    if (!Helper.MultiUserDotColorsDict.ContainsKey((Guid)annotation.LabTechnicianID))
            //                    {
            //                        Helper.MultiUserDotColorsDict.Add((Guid)annotation.LabTechnicianID, Helper.MultiUserDotColors.FirstOrDefault(x => !Helper.MultiUserDotColorsDict.Select(y => y.Value).Contains(x)));
            //                    }
            //                    annotation.MultiUserColor = Helper.MultiUserDotColorsDict.FirstOrDefault(x => x.Key == (Guid)annotation.LabTechnicianID).Value;
            //                }
            //            }
            //        }
            //    }



            //    AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            //    AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            //}
            LarvaeViewModel.LarvaeEditorViewModel.UndoRedo.EmptyStacks();
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();
            //AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
        }
    }
}
