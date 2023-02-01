using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.CodeView;
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
        private List<LarvaeSpecies> larvaeSpecies = new List<LarvaeSpecies>();
        private List<LarvaeEggQuality> larvaeQualities = new List<LarvaeEggQuality>();
        private List<LarvaeDevelopmentStage> larvaeDevelopmentStages = new List<LarvaeDevelopmentStage>();
        private List<LarvaePresence> larvaePresences = new List<LarvaePresence>();
        private List<EggEmbryoSize> eggEmbryoSizes = new List<EggEmbryoSize>();
        private List<EggYolkSegmentation> eggYolkSegmentations = new List<EggYolkSegmentation>();


        public LarvaeAnnotation Annotation
        {
            get { return annotation; }
            set
            {
                annotation = value;

                if (annotation == null)
                {
                    CreateNewAnnotation();
                }
                else
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
                        else
                        {
                            LarvaeViewModel.LarvaeEditorViewModel.CalculateResult(annotation.LarvaeAnnotationParameterResult.FirstOrDefault(x => x.LarvaeParameterID == param.ID));
                        }
                    }
                }




                RaisePropertyChanged("Annotation");
                RaisePropertyChanged("Species");
                RaisePropertyChanged("Quality");
                RaisePropertyChanged("LarvaeDevelopmentStage");
                RaisePropertyChanged("AnalFinPresence");
                RaisePropertyChanged("DorsalFinPresence");
                RaisePropertyChanged("PelvicFinPresence");
                RaisePropertyChanged("EggEmbryoPresence");
                RaisePropertyChanged("EggOilGlobulePresence");
                RaisePropertyChanged("EggEmbryoSize");
                RaisePropertyChanged("EggYolkSegmentation");
                RaisePropertyChanged("Comment");
                RaisePropertyChanged("CanEdit");
                LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();
                LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationList.BestFitColumns();

                LarvaeViewModel.Refresh();

                SelectedParameters = new List<LarvaeAnnotationParameterResult>();
                LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
                UpdateSelectionMode();
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
                else if (selectedParameters[0].Parameter.ShapeType.ToLower() == "circle")
                {
                    LarvaeViewModel.LarvaeEditorViewModel.Mode = EditorModeEnum.DrawCircle;
                }
                else if (selectedParameters[0].Parameter.ShapeType.ToLower() == "line")
                {
                    LarvaeViewModel.LarvaeEditorViewModel.Mode = EditorModeEnum.DrawLine;
                }
            }
            else if (selectedParameters != null && selectedParameters.Count > 1)
            {
                LarvaeViewModel.LarvaeEditorViewModel.Mode = EditorModeEnum.None;
            }
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();

            LarvaeViewModel.LarvaeEditorViewModel.RefreshShapes();
        }

        public bool CanEdit
        {
            get
            {
                if (LarvaeViewModel != null)
                {
                    return !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                           && LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.SelectionMode == MultiSelectMode.Cell;
                }

                return false;
            }
        }

        public Visibility LarvaeColumnsVisibility
        {
            get
            {
                if (LarvaeViewModel != null && (bool)LarvaeViewModel.LarvaeAnalysis.Type?.ToLower().Contains("lar"))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility EggColumnsVisibility
        {
            get
            {
                if (LarvaeViewModel != null && (bool)LarvaeViewModel.LarvaeAnalysis.Type?.ToLower().Contains("egg"))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public int ExtraRowSize
        {
            get
            {
                if (LarvaeViewModel != null && (bool)LarvaeViewModel.LarvaeAnalysis.Type?.ToLower().Contains("egg"))
                {
                    return 0;
                }

                return 24;
            }
        }

        public List<LarvaeSpecies> LarvaeSpecies
        {
            get { return larvaeSpecies; }
            set
            {
                larvaeSpecies = value;
                RaisePropertyChanged("LarvaeSpecies");
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

        public List<LarvaeEggQuality> LarvaeQualities
        {
            get { return larvaeQualities; }
            set
            {
                larvaeQualities = value;
                RaisePropertyChanged("LarvaeQualities");
            }
        }

        public List<EggEmbryoSize> EggEmbryoSizes
        {
            get { return eggEmbryoSizes; }
            set
            {
                eggEmbryoSizes = value;
                RaisePropertyChanged("EggEmbryoSizes");
            }
        }

        public List<EggYolkSegmentation> EggYolkSegmentations
        {
            get { return eggYolkSegmentations; }
            set
            {
                eggYolkSegmentations = value;
                RaisePropertyChanged("EggYolkSegmentations");
            }
        }

        public LarvaeSpecies Species
        {
            get { return LarvaeSpecies.FirstOrDefault(x => x.ID == Annotation?.SpeciesID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.SpeciesID) return;

                Annotation.SpeciesID = LarvaeSpecies.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.Species = LarvaeSpecies.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("Species");
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
                if (value?.ID == Annotation.DevelopmentStageID) return;
                Annotation.DevelopmentStageID = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.DevelopmentStage = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
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
                if (value?.ID == Annotation.AnalFinPresenceID) return;

                Annotation.AnalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.AnalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
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
                if (value?.ID == Annotation.DorsalFinPresenceID) return;

                Annotation.DorsalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.DorsalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
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
                if (value?.ID == Annotation.PelvicFinPresenceID) return;

                Annotation.PelvicFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.PelvicFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("PelvicFinPresence");
            }
        }

        public LarvaeEggQuality Quality
        {
            get { return LarvaeQualities.FirstOrDefault(x => x.ID == Annotation?.QualityID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.QualityID) return;

                Annotation.QualityID = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.Quality = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("Quality");
            }
        }

        public LarvaePresence EggEmbryoPresence
        {
            get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.EmbryoPresenceID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.EmbryoPresenceID) return;

                Annotation.EmbryoPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.EmbryoPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("EggEmbryoPresence");
            }
        }

        public LarvaePresence EggOilGlobulePresence
        {
            get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.OilGlobulePresenceID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.OilGlobulePresenceID) return;

                Annotation.OilGlobulePresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.OilGlobulePresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("EggOilGlobulePresence");
            }
        }

        public EggEmbryoSize EggEmbryoSize
        {
            get { return EggEmbryoSizes.FirstOrDefault(x => x.ID == Annotation?.EmbryoSizeID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.EmbryoSizeID) return;

                Annotation.EmbryoSizeID = EggEmbryoSizes.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.EmbryoSize = EggEmbryoSizes.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("EggEmbryoSize");
            }
        }

        public EggYolkSegmentation EggYolkSegmentation
        {
            get { return EggYolkSegmentations.FirstOrDefault(x => x.ID == Annotation?.YolkSegmentationID); }
            set
            {
                if (Annotation == null)
                {
                    CreateNewAnnotation();
                }
                if (value?.ID == Annotation.YolkSegmentationID) return;

                Annotation.YolkSegmentationID = EggYolkSegmentations.FirstOrDefault(x => x.ID == value?.ID)?.ID;
                Annotation.YolkSegmentation = EggYolkSegmentations.FirstOrDefault(x => x.ID == value?.ID)?.Code;
                Annotation.RequiresSaving = true;
                RaisePropertyChanged("EggYolkSegmentation");
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
                if (value == Annotation.Comments) return;

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
                    LarvaeEggSampleID = LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.ID,
                    UserID = Global.API.CurrentUser.ID,
                    User = Global.API.CurrentUser.AccountName,
                    LarvaeAnnotationParameterResult = new List<LarvaeAnnotationParameterResult>()
                };
            }
        }

        public void SaveAnnotation()
        {
            try
            {
                if(LarvaeViewModel.LarvaeOwnAnnotationViewModel?.Annotation == null 
                   || LarvaeViewModel.LoadingAnalysis
                   || LarvaeViewModel.LarvaeSampleViewModel.ChangingSample 
                   || !Annotation.RequiresSaving) return;
                LarvaeViewModel.ShowWaitSplashScreen();

                DtoLarvaeEggAnnotation dtoLarvaeEggAnnotation = (DtoLarvaeEggAnnotation)Helper.ConvertType(Annotation, typeof(DtoLarvaeEggAnnotation));
                foreach (var lapr in Annotation.LarvaeAnnotationParameterResult)
                {
                    DtoLarvaeEggAnnotationParameterResult dtoLapr = (DtoLarvaeEggAnnotationParameterResult)Helper.ConvertType(lapr, typeof(DtoLarvaeEggAnnotationParameterResult));
                    dtoLapr.AnnotationID = Annotation.ID;
                    dtoLapr.FileID = lapr.LarvaeFileID;
                    dtoLapr.ParameterID = lapr.LarvaeParameterID;
                    foreach (var dot in lapr.Dots)
                    {
                        DtoLarvaeEggDot dtoLarvaeEggDot = (DtoLarvaeEggDot)Helper.ConvertType(dot, typeof(DtoLarvaeEggDot));
                        dtoLapr.Dots.Add(dtoLarvaeEggDot);
                    }
                    foreach (var line in lapr.Lines)
                    {
                        DtoLarvaeEggLine dtoLarvaeEggLine = (DtoLarvaeEggLine)Helper.ConvertType(line, typeof(DtoLarvaeEggLine));
                        dtoLapr.Lines.Add(dtoLarvaeEggLine);
                    }

                    if (lapr.Circle != null)
                    {
                        DtoLarvaeEggCircle dtoLarvaeEggCircle = (DtoLarvaeEggCircle)Helper.ConvertType(lapr.Circle, typeof(DtoLarvaeEggCircle));
                        dtoLapr.Circle = dtoLarvaeEggCircle;
                    }

                    dtoLarvaeEggAnnotation.AnnotationParameterResult.Add(dtoLapr);
                }
                var savennotationResult = Global.API.SaveLarvaeAnnotation(LarvaeViewModel.LarvaeAnalysis.Type, dtoLarvaeEggAnnotation);
                if (!savennotationResult.Succeeded)
                {
                    LarvaeViewModel.CloseSplashScreen();
                    Helper.ShowWinUIMessageBox("Error Saving Larvae annotation\n" + savennotationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        an.Species = LarvaeSpecies
                            .FirstOrDefault(x => x.ID == an.SpeciesID)?.Code;
                        an.Quality = LarvaeQualities
                            .FirstOrDefault(x => x.ID == an.QualityID)?.Code;
                        an.DevelopmentStage = LarvaeDevelopmentStages
                            .FirstOrDefault(x => x.ID == an.DevelopmentStageID)?.Code;
                        an.AnalFinPresence = LarvaePresences
                            .FirstOrDefault(x => x.ID == an.AnalFinPresenceID)?.Code;
                        an.DorsalFinPresence = LarvaePresences
                            .FirstOrDefault(x => x.ID == an.DorsalFinPresenceID)?.Code;
                        an.PelvicFinPresence = LarvaePresences
                            .FirstOrDefault(x => x.ID == an.PelvicFinPresenceID)?.Code;
                        an.EmbryoPresence = LarvaePresences
                            .FirstOrDefault(x => x.ID == an.EmbryoPresenceID)?.Code;
                        an.EmbryoSize = EggEmbryoSizes
                            .FirstOrDefault(x => x.ID == an.EmbryoSizeID)?.Code;
                        an.YolkSegmentation = EggYolkSegmentations
                            .FirstOrDefault(x => x.ID == an.YolkSegmentationID)?.Code;
                        an.OilGlobulePresence = larvaePresences
                            .FirstOrDefault(x => x.ID == an.OilGlobulePresenceID)?.Code;

                        foreach (var lapr in an.LarvaeAnnotationParameterResult)
                        {
                            if (lapr.Parameter == null && lapr.LarvaeParameterID != Guid.Empty)
                            {
                                lapr.Parameter =
                                    LarvaeViewModel.LarvaeAnalysis.LarvaeParameters.FirstOrDefault(x =>
                                        x.ID == lapr.LarvaeParameterID);
                                lapr.File =
                                    LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.Files.FirstOrDefault(x =>
                                        x.ID == lapr.LarvaeFileID);
                            }
                        }

                    }

                    LarvaeViewModel.LarvaeAllAnnotationViewModel.LarvaeAnnotationParameterResult = new ObservableCollection<LarvaeAnnotationParameterResult>(larvaeSample.Annotations.SelectMany(x => x.LarvaeAnnotationParameterResult));
                    LarvaeViewModel.LarvaeAllAnnotationView.AnnotationList.BestFitColumns();

                    //LarvaeViewModel.LarvaeAllAnnotationViewModel.Annotations = larvaeSample.Annotations;
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
            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample && Annotation.RequiresSaving)
            {
                SaveAnnotation();
            }

        }

        public void ToggleApprove()
        {
            if (Annotation == null || (Annotation.QualityID == null || Annotation.QualityID == Guid.Empty))
            {
                Helper.ShowWinUIMessageBox("Error toggling approved state for annotation\n" + "Please make sure to fill in the required fields first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Annotation.IsApproved = !Annotation.IsApproved;
            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
            {
                SaveAnnotation();
            }
        }

        //public void AnnotateRadio_Change(object sender, RoutedEventArgs e)
        //{
        //    var radio = ((RadioButton)e.OriginalSource);
        //    if (radio.Name == "AnnotateRadio" && radio.IsChecked == true)
        //    {
        //        // code to annotate
        //        LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.SelectionMode = MultiSelectMode.Cell;
        //    }
        //    else if (radio.Name == "ShowRadio" && radio.IsChecked == true)
        //    {
        //        // code to show annotations
        //        LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.SelectionMode = MultiSelectMode.MultipleRow;
        //    }

        //    LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();
        //}

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
            //                    if (!Helper.MultiUserColorsDict.ContainsKey((Guid)annotation.LabTechnicianID))
            //                    {
            //                        Helper.MultiUserColorsDict.Add((Guid)annotation.LabTechnicianID, Helper.MultiUserColors.FirstOrDefault(x => !Helper.MultiUserColorsDict.Select(y => y.Value).Contains(x)));
            //                    }
            //                    annotation.MultiUserColor = Helper.MultiUserColorsDict.FirstOrDefault(x => x.Key == (Guid)annotation.LabTechnicianID).Value;
            //                }
            //            }
            //        }
            //    }



            //    AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            //    AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            //}
            LarvaeViewModel.LarvaeEditorViewModel.UndoRedo?.EmptyStacks();
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();

            //if (SelectedParameters.Any())
            //{
            //    LarvaeViewModel.LarvaeAllAnnotationViewModel.SelectedAnnotations = new List<LarvaeAnnotation>();
            //}

            //AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
        }

        public void Visible_ContentChanged(object sender, CellValueChangedEventArgs e)
        {
            //LarvaeViewModel?.LarvaeEditorViewModel?.RefreshShapes();
            //var tmp = Annotation.LarvaeAnnotationParameterResult.Where(x => x.IsVisible);
            //ObservableCollection<LarvaeAnnotationParameterResult> selectedParam = new ObservableCollection<LarvaeAnnotationParameterResult>();
            //ObservableCollection<LarvaeAnnotationParameterResult> notSelectedParam = new ObservableCollection<LarvaeAnnotationParameterResult>();
            //foreach (var p in LarvaeViewModel.LarvaeAllAnnotationViewModel.LarvaeAnnotationParameterResult.Where(x => x.Annotation.UserID == Global.API.CurrentUser.ID))
            //{


            //    var selected = tmp.FirstOrDefault(x => x.ID == p.ID);

            //    if (selected != null) selectedParam.Add(p);
            //    else
            //    {
            //        notSelectedParam.Add(p);
            //    }

            //}

            //foreach (var p in notSelectedParam)
            //{
            //    LarvaeViewModel.LarvaeAllAnnotationViewModel.SelectedLarvaeAnnotationParameterResults.Remove(p);

            //}
            //foreach (var p in selectedParam)
            //{
            //    if (!LarvaeViewModel.LarvaeAllAnnotationViewModel.SelectedLarvaeAnnotationParameterResults.Any(x =>
            //        x.ID == p.ID))
            //    {
            //        LarvaeViewModel.LarvaeAllAnnotationViewModel.SelectedLarvaeAnnotationParameterResults.Add(p);
            //    }
            //}
            LarvaeViewModel.LarvaeAllAnnotationView.LarvaeAnnotationGrid.RefreshData();

            //LarvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeOwnAnnotationViewModel.RaisePropertyChanged("Annotation");
            //LarvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();

            LarvaeViewModel.LarvaeEditorViewModel.RefreshShapes();
        }
    }

}
