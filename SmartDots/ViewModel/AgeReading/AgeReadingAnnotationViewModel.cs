using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DevExpress.CodeParser;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.View;

namespace SmartDots.ViewModel
{
    public class AgeReadingAnnotationViewModel : AgeReadingBaseViewModel
    {
        private ObservableCollection<Annotation> annotations = new ObservableCollection<Annotation>();
        private List<Annotation> selectedAnnotations = new List<Annotation>();
        private Annotation workingAnnotation;
        private List<Quality> qualities;
        private List<AnalysisParameter> parameters;
        private List<VisibilityState> visibilityStates;
        private List<FormatCondition> conditionalRowStyles;
        private bool canApproveAnnotation;
        private bool showNucleusColumn;
        private bool showEdgeColumn;
        private string newAnnotationTooltip;
        private string editAnnotationTooltip;
        private string deleteAnnotationTooltip;
        private string pinAnnotationTooltip;
        private string approveAnnotationTooltip;

        public ObservableCollection<Annotation> Outcomes
        {
            get { return annotations; }
            set
            {
                if (value.Any())
                    value = new ObservableCollection<Annotation>(value.OrderByDescending(x => x.IsFixed).ThenBy(x => x.DateCreation));
                annotations = value;
                SelectedAnnotations = new List<Annotation>();
                WorkingAnnotation = null;
                foreach (var annotation in value)
                {
                    annotation.Quality = Qualities.FirstOrDefault(x => x.ID == annotation.QualityID);
                    annotation.CalculateAge();
                }
                if(AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row != null)
                {
                    AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = value.Count(x => !x.IsFixed);
                    ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).AnnotationCount = value.Count(x => !x.IsFixed);
                }
                RefreshActions();
                AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            }
        }

        public List<Annotation> SelectedAnnotations
        {
            get {
                foreach (var annotation in selectedAnnotations.Where(x => string.IsNullOrEmpty(x.MultiUserColor)))
                {
                    annotation.MultiUserColor = Helper.MultiUserDotColors.FirstOrDefault(x => !annotations.Select(y => y.MultiUserColor).Contains(x));
                }
                return selectedAnnotations; }
            set
            {
                
                selectedAnnotations = value;

                RefreshActions();
                AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();

                AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
            }
        }

        public Annotation WorkingAnnotation
        {
            get { return workingAnnotation; }
            set
            {
                if(WorkingAnnotation != value)
                {
                    workingAnnotation = value;

                    RefreshActions();
                    AgeReadingViewModel.UpdateGraphs();
                }
                
            }
        }

        public List<Quality> Qualities
        {
            get { return qualities; }
            set
            {
                qualities = value;
                RaisePropertyChanged("Qualities");
            }
        }

        public List<AnalysisParameter> Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                RaisePropertyChanged("Parameters");
            }
        }

        public List<VisibilityState> VisibilityStates
        {
            get { return visibilityStates; }
            set
            {
                visibilityStates = value;
                RaisePropertyChanged("VisibilityStates");
            }
        }

        public bool ShowNucleusColumn
        {
            get { return showNucleusColumn; }
            set
            {
                showNucleusColumn = value;
                RaisePropertyChanged("ShowNucleusColumn");
            }
        }

        public bool ShowEdgeColumn
        {
            get { return showEdgeColumn; }
            set
            {
                showEdgeColumn = value;
                RaisePropertyChanged("ShowEdgeColumn");
            }
        }

        public string NewAnnotationTooltip
        {
            get { return newAnnotationTooltip; }
            set
            {
                newAnnotationTooltip = value;
                RaisePropertyChanged("NewAnnotationTooltip");
            }
        }

        public string EditAnnotationTooltip
        {
            get { return editAnnotationTooltip; }
            set
            {
                editAnnotationTooltip = value;
                RaisePropertyChanged("EditAnnotationTooltip");
            }
        }

        public string DeleteAnnotationTooltip
        {
            get { return deleteAnnotationTooltip; }
            set
            {
                deleteAnnotationTooltip = value;
                RaisePropertyChanged("DeleteAnnotationTooltip");
            }
        }

        public string PinAnnotationTooltip
        {
            get { return pinAnnotationTooltip; }
            set
            {
                pinAnnotationTooltip = value;
                RaisePropertyChanged("PinAnnotationTooltip");
            }
        }

        public string ApproveAnnotationTooltip
        {
            get { return approveAnnotationTooltip; }
            set
            {
                approveAnnotationTooltip = value;
                RaisePropertyChanged("ApproveAnnotationTooltip");
            }
        }

        public bool CanCreate
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    NewAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    NewAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    NewAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingSampleViewModel.Sample == null && !WebAPI.Settings.AnnotateWithoutSample)
                {
                    NewAnnotationTooltip = "Cannot create an Annotation when no Sample is linked";
                    return false;
                }
                if (SelectedAnnotations.Any())
                {
                    NewAnnotationTooltip = "In order to create an Annotation, make sure that no Annotations are selected";
                    return false;
                }
                if (WorkingAnnotation != null)
                {
                    NewAnnotationTooltip = "Still working on an Annotation, make sure that no Annotations are selected";
                }

                NewAnnotationTooltip = "Create new Annotation";
                return true;
            }
        }

        public bool CanEdit
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    EditAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    EditAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    EditAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (WorkingAnnotation == null)
                {
                    EditAnnotationTooltip = "In order to edit an Annotation, make sure that 1 is selected";
                    return false;
                }
                if (WorkingAnnotation.IsReadOnly)
                {
                    EditAnnotationTooltip = "The selected Annotation is read-only";
                    return false;
                }
                if (WorkingAnnotation.IsFixed)
                {
                    EditAnnotationTooltip = "The selected Annotation is a fixed reading line";
                    return false;
                }

                EditAnnotationTooltip = "Edit Annotation";
                return true;
            }
        }

        public bool CanDelete
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    DeleteAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    DeleteAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    DeleteAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (!SelectedAnnotations.Any())
                {
                    DeleteAnnotationTooltip = "No Annotations selected";
                    return false;
                }
                if (SelectedAnnotations.Any(x => x.IsFixed))
                {
                    DeleteAnnotationTooltip = "The selection contains a fixed reading line";
                    return false;
                }
                if (SelectedAnnotations.Any(x => x.IsReadOnly))
                {
                    DeleteAnnotationTooltip = "The selection contains read-only Annotation(s)";
                    return false;
                }
                DeleteAnnotationTooltip = "Delete Annotation(s)";
                return true;
            }
        }

        public bool CanTogglePin
        {
            get
            {
                if (WorkingAnnotation != null && !workingAnnotation.IsFixed)
                {
                    if (CanPin)
                    {
                        PinAnnotationTooltip = "Make fixed reading line";
                        return true;
                    }
                }
                else
                {
                    if (CanUnpin)
                    {
                        PinAnnotationTooltip = "Undo fixed reading line";
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanPin
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    PinAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.Analysis != null && !AgeReadingViewModel.Analysis.UserCanPin)
                {
                    PinAnnotationTooltip = "No permission to make fixed reading lines";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    PinAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    PinAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (WorkingAnnotation == null)
                {
                    PinAnnotationTooltip = "It is only possible to create a fixed reading line when there is 1 Annotation selected";
                    return false;
                };
                if (!WorkingAnnotation.CombinedLines.Any() || !WorkingAnnotation.CombinedLines[0].Lines.Any())
                {
                    PinAnnotationTooltip = "In order to create a fixed reading line, you have to create a line first";
                    return false;
                }
                if (WorkingAnnotation.CombinedLines[0].Dots.Any())
                {
                    PinAnnotationTooltip = "In order to create a fixed reading line, you have to remove all dots first";
                    return false;
                }
                if (WorkingAnnotation.IsFixed)
                {
                    PinAnnotationTooltip = "This is already a fixed reading line";
                    return false;
                }
                if (WorkingAnnotation.IsFixed)
                {
                    PinAnnotationTooltip = "An approved Annotation cannot be made into a fixed reading line";
                    return false;
                }
                PinAnnotationTooltip = "Make fixed reading line";
                return true;
            }
        }

        //public Visibility PinVisibility
        //{
        //    get {
        //        return CanPin ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        //public Visibility UnPinVisibility
        //{
        //    get {
        //        return CanUnpin ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        public bool CanUnpin
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    PinAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.Analysis != null && !AgeReadingViewModel.Analysis.UserCanPin)
                {
                    PinAnnotationTooltip = "No permission to make fixed reading lines";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    PinAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    PinAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (WorkingAnnotation == null)
                {
                    PinAnnotationTooltip = "It is only possible to create a fixed reading line when there is 1 Annotation selected";
                    return false;
                };
                if (!WorkingAnnotation.IsFixed)
                {
                    PinAnnotationTooltip = "This is not a fixed reading line";
                    return false;
                }
                PinAnnotationTooltip = "Undo fixed reading line";
                return true;
            }

        }

        public bool CanToggleApprove
        {
            get
            {
                if (WorkingAnnotation != null && !workingAnnotation.IsApproved)
                {
                    if (CanApprove)
                    {
                        ApproveAnnotationTooltip = "Approve Annotation";
                        return true;
                    }
                }
                else
                {
                    if (CanDisApprove)
                    {
                        ApproveAnnotationTooltip = "Unapprove Annotation";
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanApprove
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    ApproveAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    ApproveAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    ApproveAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile?.CanApprove == false)
                {
                    ApproveAnnotationTooltip = "No permission to change approvals";
                    return false;
                }
                if (WorkingAnnotation == null)
                {
                    ApproveAnnotationTooltip = "It is only possible to approve when there is 1 Annotation selected";
                    return false;
                };
                if (WorkingAnnotation.IsReadOnly)
                {
                    ApproveAnnotationTooltip = "The selected Annotation is read-only";
                    return false;
                };
                if (WorkingAnnotation.IsApproved)
                {
                    ApproveAnnotationTooltip = "The selected Annotation is already approved";
                    return false;
                }
                if (WorkingAnnotation.IsFixed)
                {
                    ApproveAnnotationTooltip = "Cannot approve a fixed reading line";
                    return false;
                }
                if (WorkingAnnotation?.QualityID != Qualities.FirstOrDefault(x => x.Code.ToUpper() == "AQ1").ID && WebAPI.Settings.RequireAq1ForApproval)
                {
                    ApproveAnnotationTooltip = "A quality code AQ1 is needed to approve the Annotation";
                    return false;
                };
                if (WorkingAnnotation?.ParameterID == null && WebAPI.Settings.RequireParamForApproval)
                {
                    ApproveAnnotationTooltip = "A parameter is needed to approve the Annotation";
                    return false;
                };
                
                ApproveAnnotationTooltip = "Approve Annotation";
                return true;
            }

        }

        public bool CanDisApprove
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection))
                {
                    ApproveAnnotationTooltip = "No active connection";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile == null)
                {
                    ApproveAnnotationTooltip = "No file selected";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly)
                {
                    ApproveAnnotationTooltip = "The selected file is read-only";
                    return false;
                }
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.CanApprove == false)
                {
                    ApproveAnnotationTooltip = "No permission to change approvals";
                    return false;
                }
                if (WorkingAnnotation == null)
                {
                    ApproveAnnotationTooltip = "It is only possible to unapprove when there is 1 Annotation selected";
                    return false;
                };
                if (WorkingAnnotation.IsReadOnly)
                {
                    ApproveAnnotationTooltip = "The selected Annotation is read-only";
                    return false;
                };
                if (!WorkingAnnotation.IsApproved)
                {
                    ApproveAnnotationTooltip = "The selected Annotation is not approved";
                    return false;
                }

                ApproveAnnotationTooltip = "Unapprove Annotation";
                return true;
            }

        }
        //public Visibility ApproveVisibility
        //{
        //    get
        //    {
        //        return CanApprove ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        //public Visibility DisApproveVisibility
        //{
        //    get
        //    {
        //        return CanDisApprove ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}
        public bool CanApproveAnnotation
        {
            get { return canApproveAnnotation; }
            set
            {
                canApproveAnnotation = value;
                if (canApproveAnnotation)
                {
                    AgeReadingViewModel.AgeReadingAnnotationView.btnApproveAnnotation.Visibility = Visibility.Visible;
                    //AgeReadingViewModel.AgeReadingAnnotationView.btnDisApproveAnnotation.Visibility = Visibility.Visible;
                }
                else
                {
                    AgeReadingViewModel.AgeReadingAnnotationView.btnApproveAnnotation.Visibility = Visibility.Collapsed;
                    //AgeReadingViewModel.AgeReadingAnnotationView.btnDisApproveAnnotation.Visibility = Visibility.Collapsed;
                }
                RaisePropertyChanged("CanApproveAnnotation");
            }
        }

        public bool AllowEditing => AgeReadingViewModel?.AgeReadingFileViewModel.SelectedFile != null && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly;

        public Dictionary<string, SolidColorBrush> AQColors { get; set; } = new Dictionary<string, SolidColorBrush>();

        public List<FormatCondition> ConditionalRowStyles
        {
            get { return conditionalRowStyles; }
            set
            {
                conditionalRowStyles = value;
                RaisePropertyChanged("ConditionalRowStyles");
            }
        }

        public string ImageNew { get; set; } = "../../Resources/New.png";
        public string ImageEdit { get; set; } = "../../Resources/Edit.png";
        public string ImageDelete { get; set; } = "../../Resources/delete-32.png";
        public string ImageApprove { get; set; } = "../../Resources/ok-32.png";
        public string ImagePin { get; set; } = "../../Resources/pin-32.png";
        public string ApproveIcon { get; set; } = "../../Resources/ok-16.png";
        public string PinIcon { get; set; } = "../../Resources/pin-16.png";


        public void ShowEditAnnotation()
        {
            EditAnnotation();
        }

        public bool EditAnnotation()
        {
            AgeReadingViewModel.EditAnnotationDialog = new EditAnnotationDialog(AgeReadingViewModel);
            AgeReadingViewModel.EditAnnotationDialogViewModel = AgeReadingViewModel.EditAnnotationDialog.EditAnnotationDialogViewModel;
            AgeReadingViewModel.EditAnnotationDialogViewModel.Annotation = WorkingAnnotation;
            ShowDialog(AgeReadingViewModel.EditAnnotationDialog);
            return AgeReadingViewModel.EditAnnotationDialog.DialogResult.Value;
        }

        public AgeReadingAnnotationViewModel()
        {
            selectedAnnotations = new List<Annotation>();
            Parameters = new List<AnalysisParameter>();
            VisibilityStates = new List<VisibilityState>();
            VisibilityStates.Add(new VisibilityState() { Value = "Opaque", Visibility = "Opaque" });
            VisibilityStates.Add(new VisibilityState() { Value = "Translucent", Visibility = "Translucent" });
            VisibilityStates.Add(new VisibilityState() { Value = null, Visibility = "NA" });
            ConditionalRowStyles = new List<FormatCondition>();
        }

        public void MapAQColors(List<Quality> qualities)
        {
            try
            {
                AQColors = new Dictionary<string, SolidColorBrush>();

                Qualities = qualities;

                foreach (var quality in Qualities)
                {
                    SolidColorBrush temp = (SolidColorBrush)new BrushConverter().ConvertFrom(quality.Color);
                    SolidColorBrush newBrush = new SolidColorBrush(Color.FromArgb(128, temp.Color.R, temp.Color.G, temp.Color.B));

                    AQColors.Add(quality.Code, newBrush);
                }
                List<FormatCondition> styles = new List<FormatCondition>();
                foreach (var quality in Qualities)
                {
                    styles.Add(new FormatCondition()
                    {
                        Expression = "[QualityGuid] == '" + quality.ID + "'",
                        ApplyToRow = true,
                        Format = new Format() { Background = AQColors[quality.Code] },
                    });
                }
                //styles.Add(new FormatCondition()
                //{
                //    Expression = "[IsFixed] = 1",
                //    //ApplyToRow = true,
                //    FieldName = "IsFixed",
                //    Format = new Format()
                //    {
                //        //Background = new SolidColorBrush(Color.FromRgb(170,255,255)),
                //        Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Resources/pin-16.png", UriKind.RelativeOrAbsolute)).Stream),
                //        IconVerticalAlignment = VerticalAlignment.Center,
                //    }
                //});
                //styles.Add(new FormatCondition()
                //{
                //    Expression = "[IsApproved] = 1",
                //    //ApplyToRow = true,
                //    FieldName = "IsApproved",
                //    Format = new Format()
                //    {
                //        //Background = new SolidColorBrush(Color.FromRgb(170,255,255)),
                //        Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Resources/ok-16.png", UriKind.RelativeOrAbsolute)).Stream),
                //        IconVerticalAlignment = VerticalAlignment.Center,
                //    }
                //});
                ConditionalRowStyles = styles;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error Mapping AQ colors", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
        }

        public void NewAnnotation()
        {
            try
            {
                Annotation an = new Annotation()
                {
                    LabTechnicianID = WebAPI.CurrentUser?.ID,
                    LabTechnician = WebAPI.CurrentUser.AccountName,
                    DateCreation = DateTime.Now,
                    FileID = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID,
                    Qualities = Qualities,
                    SampleID = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile?.SampleID,
                    ID = Guid.NewGuid(),
                };
                var pinnedAnnotation = AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.FirstOrDefault(x => x.IsFixed);
                if (pinnedAnnotation == null)
                {
                    AgeReadingViewModel.AgeReadingEditorViewModel.Mode = EditorModeEnum.DrawLine;

                }
                else
                {
                    an.CombinedLines = new List<CombinedLine>();
                    an.CombinedLines.Add(new CombinedLine() { ID = Guid.NewGuid(), Dots = Helper.DeepCopy(pinnedAnnotation.CombinedLines[0].Dots), Lines = Helper.DeepCopy(pinnedAnnotation.CombinedLines[0].Lines), IsFixed = true });
                    foreach (var dot in an.CombinedLines[0].Dots)
                    {
                        dot.ID = Guid.NewGuid();
                        dot.AnnotationID = an.ID;
                        dot.ParentCombinedLine = an.CombinedLines[0];
                    }
                    foreach (var line in an.CombinedLines[0].Lines)
                    {
                        line.ID = Guid.NewGuid();
                        line.AnnotationID = an.ID;
                        line.ParentCombinedLine = an.CombinedLines[0];
                    }
                    an.IsChanged = true;
                    AgeReadingViewModel.AgeReadingEditorViewModel.Mode = EditorModeEnum.DrawDot;
                }

                if (Parameters.Count == 1) an.ParameterID = Parameters[0].ID;
                var dtoAnnotation = (DtoAnnotation)Helper.ConvertType(an, typeof(DtoAnnotation));
                dtoAnnotation.SampleId = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile?.SampleID;
                dtoAnnotation.AnalysisId = AgeReadingViewModel.Analysis.ID;
                var webapiresult = WebAPI.AddAnnotation(dtoAnnotation);
                if (!webapiresult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox(webapiresult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Outcomes.Add(an);

                int rowhandle = AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.FindRowByValue("DateCreation", an.DateCreation);
                AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.DataController.Selection.Clear();
                AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.DataController.Selection.SetSelected(rowhandle, true);
                AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.Focusable = false;
                WorkingAnnotation = an;

                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = Outcomes.Count(x => !x.IsFixed);
                ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).AnnotationCount = Outcomes.Count(x => !x.IsFixed);
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
                AgeReadingViewModel.AgeReadingFileViewModel.Refresh();

            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error creating new Annotation", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
        }

        public void DeleteAnnotation()
        {
            try
            {
                //get selected annotations from the grid and remove them (set gc-record = true)
                if (SelectedAnnotations.Any())
                {
                    if (SelectedAnnotations.Any(x => x.IsFixed))
                    {
                        string action = "Please deselect the Pinned Annotation or ask an Administrator to unpin it.";
                        if (AgeReadingViewModel.Analysis.UserCanPin) action = "Please deselect the Pinned Annotation or unpin it.";
                        Helper.ShowWinUIMessageBox("Can not delete a fixed Annotation! " + action, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var deleteAnnotationsResult = WebAPI.DeleteAnnotations(SelectedAnnotations.Select(x => x.ID).ToList());
                    if (!deleteAnnotationsResult.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox("Error deleting Annotations\n" + deleteAnnotationsResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                WorkingAnnotation = null;

                foreach (var outcome in SelectedAnnotations.ToList())
                {
                    Outcomes.Remove(outcome);
                }

                var selectedFile = AgeReadingViewModel.AgeReadingFileViewModel.Files.FirstOrDefault(x => x == AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile);

                if (!Outcomes.Any(x => !x.IsFixed))
                {
                    var dbfile = WebAPI.GetFile(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID, false, true);
                    if (!dbfile.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox("Error loading File from Web API\n" + dbfile.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var file = (File)Helper.ConvertType(dbfile.Result, typeof(File));
                    file.Sample = (Sample)Helper.ConvertType(dbfile.Result.Sample, typeof(Sample));
                    if (selectedFile != null)
                    {
                        selectedFile.Sample = file.Sample;
                        //selectedFile.AnnotationCount = file.AnnotationCount; this is not needed here
                        selectedFile.IsReadOnly = file.IsReadOnly;
                        selectedFile.Scale = file.Scale;

                    }

                    selectedFile.AnnotationCount = Outcomes.Count(x => !x.IsFixed);
                    selectedFile.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
                    ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).AnnotationCount = Outcomes.Count(x => !x.IsFixed);
                    var dynFile = AgeReadingViewModel.AgeReadingFileViewModel.CreateDynamicFile(selectedFile);
                    AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row = dynFile;
                }

                
                //AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                AgeReadingViewModel.AgeReadingFileViewModel.Refresh();
                AgeReadingViewModel.AgeReadingFileViewModel.UpdateList();
                UpdateList();

            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error deleting annotations", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
        }

        public void SetAge(int? age)
        {
            WorkingAnnotation?.SetAge(age);
        }

        public void UpdateList()
        {
            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
        }

        public void RefreshActions()
        {
            RaisePropertyChanged("Outcomes");
            RaisePropertyChanged("SelectedAnnotations");
            RaisePropertyChanged("CanCreate");
            RaisePropertyChanged("CanEdit");
            RaisePropertyChanged("CanDelete");
            RaisePropertyChanged("CanPin");
            RaisePropertyChanged("CanUnpin");
            RaisePropertyChanged("CanTogglePin");
            RaisePropertyChanged("CanApprove");
            RaisePropertyChanged("CanDisApprove");
            //RaisePropertyChanged("PinVisibility");
            //RaisePropertyChanged("UnPinVisibility");
            //RaisePropertyChanged("ApproveVisibility");
            //RaisePropertyChanged("DisApproveVisibility");
            RaisePropertyChanged("CanToggleApprove");
            RaisePropertyChanged("AllowEditing");
        }

        public void GridControl_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = null;
            if (SelectedAnnotations.Count == 1)
            {
                WorkingAnnotation = SelectedAnnotations[0];

                foreach (var combinedline in WorkingAnnotation.CombinedLines)
                {
                    combinedline.RecalculatePoints();
                    combinedline.CalculateDotIndices();
                }
                try
                {
                    AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = WorkingAnnotation?.CombinedLines.FirstOrDefault();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else
            {
                WorkingAnnotation = null;
                AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            }
            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
        }

        public void AnnotationList_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            var an = Outcomes.FirstOrDefault(x => x.ID == ((Annotation)e.Row).ID);
            if (e.Column.FieldName == "ParameterID")
            {
                an.ParameterID = (Guid?)e.Value;
                an.IsChanged = true;
                //AgeReadingViewModel.SaveAnnotations();
            }
            else if (e.Column.FieldName == "QualityID")
            {
                var aq3 = Qualities.FirstOrDefault(x => x.Code.ToLower().Trim().Equals("aq3"));
                if (aq3 != null && (Guid)e.Value == aq3.ID)
                {
                    WinUIMessageBoxService messageBox = new WinUIMessageBoxService();
                    var result = messageBox.ShowMessage("Changing the readability quality to AQ3 will remove all dots for this annotation, are you sure you wish to continue?", "Warning", MessageButton.YesNo, MessageIcon.Warning, MessageResult.No);

                    if (result == MessageResult.No)
                    {
                        e.Handled = false;
                        an.QualityID = (Guid?)e.OldValue;

                        AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
                        AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.CancelRowEdit();
                        //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.Focus();
                        return;
                    }
                }

                an.QualityID = (Guid?)e.Value;
                an.IsChanged = true;
                var quality = Qualities.FirstOrDefault(x => x.ID == an.QualityID);
                if (quality == null) return;
                an.Quality = quality;
                an.CalculateAge();
                ageReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
                //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            }
            else if (e.Column.FieldName == "Nucleus")
            {
                an.Nucleus = e.Value?.ToString();
                an.IsChanged = true;
                //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            }
            else if (e.Column.FieldName == "Edge")
            {
                an.Edge = e.Value?.ToString();
                an.IsChanged = true;
                //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            }
        }


        public void AnnotationList_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            RefreshActions();
            e.Handled = true;
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            //disable selecting all through keyboard
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.A)
            {
                e.Handled = true;
            }
        }

        //public void AnnotationList_ShowingEditor(object sender, ShowingEditorEventArgs e)
        //{
        //    if (WorkingAnnotation == null)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }
        //    var an = Outcomes.FirstOrDefault(x => x.ID == ((Annotation)e.Row).ID);
        //    if (an != null && an.IsApproved)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }


        //if (e.Column.FieldName == "IsApproved")
        //{
        //    var newValue = !(bool)e.Value;
        //    var an = Outcomes.FirstOrDefault(x => x.ID == ((Annotation)e.Row).ID);
        //    if (an == null)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }
        //    if (an.QualityID != Qualities.FirstOrDefault(x => x.Code == "AQ1").ID && WebAPI.Settings.RequireAq1ForApproval && newValue)
        //    {
        //        new WinUIMessageBoxService().Show("Can not approve an Annotation without an AQ1 value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        e.Cancel = true;
        //        return;
        //    }
        //    if (newValue)
        //    {
        //        foreach (var outcome in Outcomes)
        //        {
        //            if (outcome.IsApproved) outcome.IsChanged = true;
        //            outcome.IsApproved = false;
        //        }

        //        an.IsApproved = true;
        //        an.IsChanged = true;
        //    }
        //    else
        //    {
        //        an.IsApproved = false;
        //        an.IsChanged = true;
        //    }
        //    AgeReadingViewModel.SaveAnnotations();

        //    //Helper.DoAsync(SetSampleStatus);

        //    //hack
        //    e.Column.AllowFocus = false;
        //    e.Column.AllowFocus = true;
        //    if (newValue)
        //    {
        //        AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.AllowEditing = false;
        //    }
        //}
        //}
        public void TogglePinAnnotation()
        {
            if (CanPin) PinAnnotation();
            else
            {
                UnpinAnnotation();
            }
        }

        public void PinAnnotation()
        {
            //if (!WorkingAnnotation.CombinedLines.Any() || !WorkingAnnotation.CombinedLines[0].Lines.Any())
            //{
            //    Helper.ShowWinUIMessageBox("Annotation does not contain any Lines!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            foreach (var annotation in Outcomes)
            {
                if (annotation.IsFixed)
                {
                    annotation.IsChanged = true;
                    annotation.IsFixed = false;

                }
            }
            WorkingAnnotation.IsFixed = true;
            WorkingAnnotation.Quality = null;
            WorkingAnnotation.QualityID = null;
            WorkingAnnotation.IsApproved = false;

            WorkingAnnotation.IsChanged = true;

            AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = Outcomes.Count(x => !x.IsFixed);
            ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).AnnotationCount = Outcomes.Count(x => !x.IsFixed);
            AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
            AgeReadingViewModel.AgeReadingFileViewModel.Refresh();

            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.InvalidateVisual();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.UpdateLayout();
            AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            RefreshActions();

            RaisePropertyChanged("Outcomes");
        }

        public void UnpinAnnotation()
        {
            WorkingAnnotation.IsFixed = false;
            WorkingAnnotation.IsChanged = true;
            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            RefreshActions();
            AgeReadingViewModel.AgeReadingAnnotationView.Focus();
            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.InvalidateVisual();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.UpdateLayout();
            RaisePropertyChanged("Outcomes");
        }

        public void ToggleApprove()
        {
            if (CanApprove) Approve();
            else
            {
                DisApprove();
            }
        }

        public void Approve()
        {
            foreach (var outcome in Outcomes)
            {
                if (outcome.IsApproved && !WebAPI.Settings.AllowMultipleApprovements)
                {
                    outcome.IsChanged = true;
                    outcome.IsApproved = false;
                }
            }

            WorkingAnnotation.IsApproved = true;
            WorkingAnnotation.IsChanged = true;

            AgeReadingViewModel.SaveAnnotations();
            //AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly = true;
            AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            RefreshActions();
            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
        }

        public void DisApprove()
        {
            WorkingAnnotation.IsApproved = false;
            WorkingAnnotation.IsChanged = true;
            AgeReadingViewModel.SaveAnnotations();
            //AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly = false;
            RefreshActions();
        }

        public void AnnotationList_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
