using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp;
using SmartDots.Helpers;
using SmartDots.Model;
using XamlReader = System.Windows.Markup.XamlReader;

namespace SmartDots.ViewModel
{
    public class LarvaeOwnAnnotationViewModel : LarvaeBaseViewModel
    {
        private LarvaeAnnotation annotation;
        private List<LarvaeAnnotationParameterResult> selectedParameters = new List<LarvaeAnnotationParameterResult>();
        private Dictionary<LarvaeAnnotationProperty, List<DtoLookupItem>> dict = new Dictionary<LarvaeAnnotationProperty, List<DtoLookupItem>>();


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
                    foreach (var param in LarvaeViewModel.LarvaeAnalysis.LarvaeEggParameters)
                    {
                        if (!annotation.LarvaeAnnotationParameterResult.Any(x => x.ParameterID == param.ID))
                        {
                            annotation.LarvaeAnnotationParameterResult.Add(new LarvaeAnnotationParameterResult()
                            {
                                ID = Guid.NewGuid(),
                                ParameterID = param.ID,
                                Parameter = LarvaeViewModel.LarvaeAnalysis.LarvaeEggParameters.FirstOrDefault(x => x.ID == param.ID),
                                AnnotationID = annotation.ID,
                                Lines = new List<LarvaeLine>(),
                                Dots = new List<LarvaeDot>()
                            });
                        }
                        else
                        {
                            LarvaeViewModel.LarvaeEditorViewModel.CalculateResult(annotation.LarvaeAnnotationParameterResult.FirstOrDefault(x => x.ParameterID == param.ID));
                        }
                    }
                }

                FillGrid();

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

        public Dictionary<LarvaeAnnotationProperty, List<DtoLookupItem>> Dict
        {
            get => dict;
            set => dict = value;
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


        //public List<LarvaeSpecies> LarvaeSpecies
        //{
        //    get { return larvaeSpecies; }
        //    set
        //    {
        //        larvaeSpecies = value;
        //        RaisePropertyChanged("LarvaeSpecies");
        //    }
        //}

        //public List<LarvaeDevelopmentStage> LarvaeDevelopmentStages
        //{
        //    get { return larvaeDevelopmentStages; }
        //    set
        //    {
        //        larvaeDevelopmentStages = value;
        //        RaisePropertyChanged("LarvaeDevelopmentStages");
        //    }
        //}

        //public List<LarvaePresence> LarvaePresences
        //{
        //    get { return larvaePresences; }
        //    set
        //    {
        //        larvaePresences = value;
        //        RaisePropertyChanged("LarvaePresences");
        //    }
        //}

        //public List<LarvaeEggQuality> LarvaeQualities
        //{
        //    get { return larvaeQualities; }
        //    set
        //    {
        //        larvaeQualities = value;
        //        RaisePropertyChanged("LarvaeQualities");
        //    }
        //}

        //public List<EggEmbryoSize> EggEmbryoSizes
        //{
        //    get { return eggEmbryoSizes; }
        //    set
        //    {
        //        eggEmbryoSizes = value;
        //        RaisePropertyChanged("EggEmbryoSizes");
        //    }
        //}

        //public List<EggYolkSegmentation> EggYolkSegmentations
        //{
        //    get { return eggYolkSegmentations; }
        //    set
        //    {
        //        eggYolkSegmentations = value;
        //        RaisePropertyChanged("EggYolkSegmentations");
        //    }
        //}

        //public LarvaeSpecies Species
        //{
        //    get { return LarvaeSpecies.FirstOrDefault(x => x.ID == Annotation?.SpeciesID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.SpeciesID) return;

        //        Annotation.SpeciesID = LarvaeSpecies.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.Species = LarvaeSpecies.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("Species");
        //    }
        //}

        //public LarvaeDevelopmentStage LarvaeDevelopmentStage
        //{
        //    get { return LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == Annotation?.DevelopmentStageID); }
        //    set
        //    {

        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.DevelopmentStageID) return;
        //        Annotation.DevelopmentStageID = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.DevelopmentStage = LarvaeDevelopmentStages.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("LarvaeDevelopmentStage");
        //    }
        //}

        //public LarvaePresence AnalFinPresence
        //{
        //    get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.AnalFinPresenceID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.AnalFinPresenceID) return;

        //        Annotation.AnalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.AnalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("AnalFinPresence");
        //    }
        //}

        //public LarvaePresence DorsalFinPresence
        //{
        //    get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.DorsalFinPresenceID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.DorsalFinPresenceID) return;

        //        Annotation.DorsalFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.DorsalFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("DorsalFinPresence");
        //    }
        //}

        //public LarvaePresence PelvicFinPresence
        //{
        //    get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.PelvicFinPresenceID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.PelvicFinPresenceID) return;

        //        Annotation.PelvicFinPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.PelvicFinPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("PelvicFinPresence");
        //    }
        //}

        //public LarvaeEggQuality Quality
        //{
        //    get { return LarvaeQualities.FirstOrDefault(x => x.ID == Annotation?.QualityID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.QualityID) return;

        //        Annotation.QualityID = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.Quality = LarvaeQualities.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("Quality");
        //    }
        //}

        //public LarvaePresence EggEmbryoPresence
        //{
        //    get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.EmbryoPresenceID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.EmbryoPresenceID) return;

        //        Annotation.EmbryoPresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.EmbryoPresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("EggEmbryoPresence");
        //    }
        //}

        //public LarvaePresence EggOilGlobulePresence
        //{
        //    get { return LarvaePresences.FirstOrDefault(x => x.ID == Annotation?.OilGlobulePresenceID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.OilGlobulePresenceID) return;

        //        Annotation.OilGlobulePresenceID = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.OilGlobulePresence = LarvaePresences.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("EggOilGlobulePresence");
        //    }
        //}

        //public EggEmbryoSize EggEmbryoSize
        //{
        //    get { return EggEmbryoSizes.FirstOrDefault(x => x.ID == Annotation?.EmbryoSizeID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.EmbryoSizeID) return;

        //        Annotation.EmbryoSizeID = EggEmbryoSizes.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.EmbryoSize = EggEmbryoSizes.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("EggEmbryoSize");
        //    }
        //}

        //public EggYolkSegmentation EggYolkSegmentation
        //{
        //    get { return EggYolkSegmentations.FirstOrDefault(x => x.ID == Annotation?.YolkSegmentationID); }
        //    set
        //    {
        //        if (Annotation == null)
        //        {
        //            CreateNewAnnotation();
        //        }
        //        if (value?.ID == Annotation.YolkSegmentationID) return;

        //        Annotation.YolkSegmentationID = EggYolkSegmentations.FirstOrDefault(x => x.ID == value?.ID)?.ID;
        //        Annotation.YolkSegmentation = EggYolkSegmentations.FirstOrDefault(x => x.ID == value?.ID)?.Code;
        //        Annotation.RequiresSaving = true;
        //        RaisePropertyChanged("EggYolkSegmentation");
        //    }
        //}

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

        public void FillGrid()
        {
            ClearGrid();
            LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Dispatcher.Invoke(() =>
            {

                int rowHeight = 24;
                int startpos = 0;
                if (LarvaeViewModel.LarvaeAnalysis.LarvaeEggAnnotationProperties != null)
                {
                    var canEditBinding = new Binding(nameof(CanEdit))
                    {
                        Source = this,
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    foreach (LarvaeAnnotationProperty prop in LarvaeViewModel.LarvaeAnalysis.LarvaeEggAnnotationProperties)
                    {
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(rowHeight)
                        });

                        StackPanel p = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, FlowDirection = FlowDirection.LeftToRight, Orientation = Orientation.Horizontal };
                        Label key = new Label() { Content = prop.Label, Padding = new Thickness(0), FontWeight = FontWeights.Bold };
                        p.Children.Add(key);

                        if (prop.IsRequired)
                        {
                            Label required = new Label() { Content = "*", Padding = new Thickness(0), Margin = new Thickness(2, 0, 0, 0), FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)), ToolTip = "Required for approval" };
                            p.Children.Add(required);
                        }

                        LookUpEdit editor = new LookUpEdit()
                        {
                            AllowNullInput = true,
                            AutoPopulateColumns = false,
                            NullValueButtonPlacement = EditorPlacement.Popup,
                            Background = (SolidColorBrush)Application.Current.TryFindResource("BrushSmartFishYellow") ?? Brushes.White,
                            Padding = new Thickness(0, -2, 0, 0),
                            Margin = new Thickness(0, 2, 0, 2),
                            EditValue = annotation.PropertyValues.ContainsKey(prop.Property) ? Dict[prop].FirstOrDefault(x => x.ID == annotation.PropertyValues[prop.Property]) : null,
                            SelectedItem = annotation.PropertyValues.ContainsKey(prop.Property) ? Dict[prop].FirstOrDefault(x => x.ID == annotation.PropertyValues[prop.Property]) : null,
                            ItemsSource = Dict[prop],
                            DisplayMember = "Code",
                            //IsEnabled = CanEdit, // check if binding possible
                            Tag = prop.Property
                        };

                        editor.EditValueChanged += delegate (object sender, EditValueChangedEventArgs args)
                        {
                            string tag = ((LookUpEdit)sender).Tag.ToString();
                            Annotation.PropertyValues[tag] = ((DtoLookupItem)args.NewValue)?.ID;
                            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
                            {
                                Annotation.RequiresSaving = true;
                                SaveAnnotation();
                            }
                        };

                        editor.SetBinding(UIElement.IsEnabledProperty, canEditBinding);

                        //editor.


                        const string xamlTemplate = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><GridControl xmlns='http://schemas.devexpress.com/winfx/2008/xaml/grid' Name=\"PART_GridControl\"><GridControl.View><TableView AutoWidth=\"True\" RowMinHeight=\"32\"/></GridControl.View><GridColumn FieldName=\"Code\" Width=\"20*\"/><GridColumn FieldName=\"Description\" Width=\"80*\" /></GridControl></ControlTemplate>";
                        XamlReaderSettings settings = new XamlReaderSettings();
                        //Assembly wfAssembly = Assembly.LoadFile(@"DevExpress.Xpf.Grid.v17.1.dll");
                        //settings.LocalAssembly = wfAssembly;

                        var controlTemplate = (ControlTemplate)XamlReader.Parse(xamlTemplate);

                        //var controlTemplate = new ControlTemplate(typeof(Button));

                        editor.PopupContentTemplate = controlTemplate;

                        Grid.SetColumn(p, 0);
                        Grid.SetRow(p, startpos);

                        Grid.SetColumn(editor, 1);
                        Grid.SetRow(editor, startpos);
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(p);
                        LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(editor);
                        startpos++;

                        

                    }
                    //<dxe:TextEdit Grid.Row="7" Grid.Column="1" x:Name="LarvaeComments" EditValue="{Binding Comment, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" Background="{StaticResource BrushSmartFishYellow}" Height="68" TextWrapping="Wrap" AcceptsReturn="True" VerticalContentAlignment="Top" IsEnabled="{Binding CanEdit, Mode=OneWay, UpdateSourceTrigger=PropertyChanged}">

                    LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(72)
                    });

                    TextEdit te = new TextEdit()
                    {
                        Background = (SolidColorBrush)Application.Current.TryFindResource("BrushSmartFishYellow") ?? Brushes.White,
                        Height = 68,
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true,
                        VerticalContentAlignment = VerticalAlignment.Top,
                    };

                    var commentBinding = new Binding(nameof(Comment))
                    {
                        Source = this,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    te.SetBinding(UIElement.IsEnabledProperty, canEditBinding);
                    te.SetBinding(BaseEdit.EditValueProperty, commentBinding);

                    Label commentLabel = new Label() { Content = "Comment", Padding = new Thickness(0), FontWeight = FontWeights.Bold };


                    Grid.SetColumn(commentLabel, 0);
                    Grid.SetRow(commentLabel, startpos);

                    Grid.SetColumn(te, 1);
                    Grid.SetRow(te, startpos);
                    LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(commentLabel);
                    LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Add(te);
                }
                //if (Annotation != null)
                //{
                //LarvaeViewModel.MaturityOwnAnnotationView.Grid.RowDefinitions.Add(new RowDefinition()
                //{
                //    Height = new GridLength(rowHeight)
                //});

            });
        }

        public void ClearGrid()
        {
            LarvaeViewModel.LarvaeOwnAnnotationView.Grid.RowDefinitions.Clear();

            LarvaeViewModel.LarvaeOwnAnnotationView.Grid.Children.Clear();

        }

        public bool SaveAnnotation()
        {
            try
            {
                if (LarvaeViewModel.LarvaeOwnAnnotationViewModel?.Annotation == null
                   || LarvaeViewModel.LoadingAnalysis
                   || LarvaeViewModel.LarvaeSampleViewModel.ChangingSample
                   || !Annotation.RequiresSaving) return false;
                LarvaeViewModel.ShowWaitSplashScreen();

                DtoLarvaeEggAnnotation dtoLarvaeEggAnnotation = (DtoLarvaeEggAnnotation)Helper.ConvertType(Annotation, typeof(DtoLarvaeEggAnnotation));
                foreach (var lapr in Annotation.LarvaeAnnotationParameterResult)
                {
                    DtoLarvaeEggAnnotationParameterResult dtoLapr = (DtoLarvaeEggAnnotationParameterResult)Helper.ConvertType(lapr, typeof(DtoLarvaeEggAnnotationParameterResult));
                    dtoLapr.AnnotationID = Annotation.ID;
                    dtoLapr.FileID = lapr.FileID;
                    dtoLapr.ParameterID = lapr.ParameterID;
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
                    Helper.ShowWinUIMessageBox($"Error Saving {LarvaeViewModel.LarvaeAnalysis.Type} annotation\n" + savennotationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                Annotation.RequiresSaving = false;

                if (savennotationResult.Result != null)
                {
                    var larvaeSample = (LarvaeSample)Helper.ConvertType(savennotationResult.Result, typeof(LarvaeSample));

                    var sample = LarvaeViewModel.LarvaeSampleViewModel.LarvaeSamples.FirstOrDefault(x => x.ID == LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.ID);

                    sample.StatusCode = larvaeSample.StatusCode;
                    sample.StatusColor = larvaeSample.StatusColor;
                    sample.StatusRank = larvaeSample.StatusRank;
                    sample.IsReadOnly = larvaeSample.IsReadOnly;
                    sample.UserHasApproved = larvaeSample.UserHasApproved;
                    sample.AllowApproveToggle = larvaeSample.AllowApproveToggle;

                    var dynSample = LarvaeViewModel.LarvaeSampleViewModel.DynamicSamples.FirstOrDefault(x => x.ID == sample.ID);

                    dynSample.StatusRank = sample.StatusRank;
                    dynSample.StatusColor = sample.StatusColor;
                    dynSample.StatusCode = sample.StatusCode;
                    dynSample.Status = sample.Status;
                    dynSample.IsReadOnly = sample.IsReadOnly;
                    sample.UserHasApproved = larvaeSample.UserHasApproved;
                    sample.AllowApproveToggle = larvaeSample.AllowApproveToggle;

                    LarvaeViewModel.LarvaeSampleViewModel.UpdateList();

                    larvaeSample.ConvertDbAnnotations(savennotationResult.Result.Annotations.ToList());
                    foreach (var an in larvaeSample.Annotations)
                    {
                        //an.Species = LarvaeSpecies
                        //    .FirstOrDefault(x => x.ID == an.SpeciesID)?.Code;
                        //an.Quality = LarvaeQualities
                        //    .FirstOrDefault(x => x.ID == an.QualityID)?.Code;
                        //an.DevelopmentStage = LarvaeDevelopmentStages
                        //    .FirstOrDefault(x => x.ID == an.DevelopmentStageID)?.Code;
                        //an.AnalFinPresence = LarvaePresences
                        //    .FirstOrDefault(x => x.ID == an.AnalFinPresenceID)?.Code;
                        //an.DorsalFinPresence = LarvaePresences
                        //    .FirstOrDefault(x => x.ID == an.DorsalFinPresenceID)?.Code;
                        //an.PelvicFinPresence = LarvaePresences
                        //    .FirstOrDefault(x => x.ID == an.PelvicFinPresenceID)?.Code;
                        //an.EmbryoPresence = LarvaePresences
                        //    .FirstOrDefault(x => x.ID == an.EmbryoPresenceID)?.Code;
                        //an.EmbryoSize = EggEmbryoSizes
                        //    .FirstOrDefault(x => x.ID == an.EmbryoSizeID)?.Code;
                        //an.YolkSegmentation = EggYolkSegmentations
                        //    .FirstOrDefault(x => x.ID == an.YolkSegmentationID)?.Code;
                        //an.OilGlobulePresence = larvaePresences
                        //    .FirstOrDefault(x => x.ID == an.OilGlobulePresenceID)?.Code;

                        foreach (var lapr in an.LarvaeAnnotationParameterResult)
                        {
                            if (lapr.Parameter == null && lapr.ParameterID != Guid.Empty)
                            {
                                lapr.Parameter =
                                    LarvaeViewModel.LarvaeAnalysis.LarvaeEggParameters.FirstOrDefault(x =>
                                        x.ID == lapr.ParameterID);
                                lapr.File =
                                    LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.Files.FirstOrDefault(x =>
                                        x.ID == lapr.FileID);
                            }
                        }

                    }

                    LarvaeViewModel.LarvaeAllAnnotationViewModel.Annotations = new ObservableCollection<LarvaeAnnotation>(larvaeSample.Annotations);
                    LarvaeViewModel.LarvaeAllAnnotationView.AnnotationList.BestFitColumns();
                }

                RaisePropertyChanged("CanEdit");
                LarvaeViewModel.RaisePropertyChanged("CanToggleApprove");
                LarvaeViewModel.CloseSplashScreen();
                LarvaeViewModel.Refresh();
                if (LarvaeViewModel.LarvaeSampleViewModel.LarvaeSamples.All(x => x.UserHasApproved))
                {
                    LarvaeViewModel.PromptAnalysisCompleteDialog();
                }

                return true;
            }
            catch (Exception e)
            {
                LarvaeViewModel.CloseSplashScreen();
                Console.WriteLine(e);
                return false;
            }
            


            
        }

        //public void LarvaeLookup_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        //{
        //    if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample && Annotation.RequiresSaving)
        //    {
        //        SaveAnnotation();
        //    }

        //}

        public void ToggleApprove()
        {
            // todo only able to approve when required fields are provided

            //if (false)
            //{
            //    Helper.ShowWinUIMessageBox("Error toggling approved state for annotation\n" + "Please make sure to fill in the required fields first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            

            if (!LarvaeViewModel.LarvaeSampleViewModel.ChangingSample)
            {
                Annotation.IsApproved = !Annotation.IsApproved;
                Annotation.RequiresSaving = true;
                if (!SaveAnnotation())
                {
                    Annotation.IsApproved = !Annotation.IsApproved;
                }
            }
        }

        public void GridControl_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            LarvaeViewModel.LarvaeEditorViewModel.UndoRedo?.EmptyStacks();
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();
        }

        public void Visible_ContentChanged(object sender, CellValueChangedEventArgs e)
        {
            LarvaeViewModel.LarvaeAllAnnotationView.LarvaeAnnotationGrid.RefreshData();
            LarvaeViewModel.LarvaeEditorViewModel.RefreshShapes();
        }
    }

}
