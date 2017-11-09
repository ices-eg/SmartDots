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
        private List<FormatCondition> conditionalRowStyles;
        private bool canApproveAnnotation;

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
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = value.Count;
                RefreshActions();
                AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            }
        }

        public List<Annotation> SelectedAnnotations
        {
            get { return selectedAnnotations; }
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
                workingAnnotation = value;
                //AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = null;

                RefreshActions();
                AgeReadingViewModel.UpdateGraphs();
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

        public bool CanCreate
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (AgeReadingViewModel.AgeReadingSampleViewModel.Sample == null && !WebAPI.Settings.AnnotateWithoutSample) return false;
                if (SelectedAnnotations.Any()) return false;
                return WorkingAnnotation == null;
            }
        }

        public bool CanEdit
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                return WorkingAnnotation != null && !WorkingAnnotation.IsReadOnly && !WorkingAnnotation.IsFixed;
            }
        }

        public bool CanDelete
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (!SelectedAnnotations.Any()) return false;
                if (SelectedAnnotations.Any(x => x.IsFixed)) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                return !SelectedAnnotations.Any(x => x.IsReadOnly) /*&& !SelectedAnnotations.Any(x => x.IsFixed)*/;
            }
        }

        public bool CanTogglePin
        {
            get { return CanPin || CanUnpin; }
        }

        public bool CanPin
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (!AgeReadingViewModel.Analysis.UserCanPin) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (WorkingAnnotation == null) return false;
                if (!WorkingAnnotation.CombinedLines.Any() || !WorkingAnnotation.CombinedLines[0].Lines.Any()) return false;
                if (WorkingAnnotation.CombinedLines[0].Dots.Any()) return false;
                return !WorkingAnnotation.IsFixed;
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
            get {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (!AgeReadingViewModel.Analysis.UserCanPin) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (WorkingAnnotation == null) return false;
                return WorkingAnnotation.IsFixed;
            }
            
        }

        public bool CanToggleApprove
        {
            get { return CanApprove || CanDisApprove; }
        }

        public bool CanApprove
        {
            get {
                //if(WorkOffline) return false; //todo yann
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (WorkingAnnotation == null) return false;
                if (WorkingAnnotation.QualityID != null && WorkingAnnotation.QualityID != Qualities.FirstOrDefault(x => x.Code == "AQ1").ID && WebAPI.Settings.RequireAq1ForApproval) return false;
                if (WorkingAnnotation.ParameterID == null && WebAPI.Settings.RequireParamForApproval) return false;
                return !WorkingAnnotation.IsApproved;
            }
            
        }

        public bool CanDisApprove
        {
            get {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (WorkingAnnotation == null) return false;
                return WorkingAnnotation.IsApproved;
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
                    an.CombinedLines.Add(new CombinedLine() { ID = Guid.NewGuid(), Dots = Helper.DeepCopy(pinnedAnnotation.CombinedLines[0].Dots), Lines = Helper.DeepCopy(pinnedAnnotation.CombinedLines[0].Lines), IsFixed = true});
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

                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = Outcomes.Count;
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.FetchProps();
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
                    if (!WebAPI.DeleteAnnotations(SelectedAnnotations.Select(x => x.ID).ToList()).Succeeded)
                    {
                        Helper.ShowWinUIMessageBox("Error deleting Annotations", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                WorkingAnnotation = null;

                foreach (var outcome in SelectedAnnotations.ToList())
                {
                    Outcomes.Remove(outcome);
                }
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.AnnotationCount = Outcomes.Count;

                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.FetchProps();
                //AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                AgeReadingViewModel.AgeReadingFileViewModel.Refresh();
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error deleting annotations", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
            }
        }

        public void SetAge(int age)
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
                    AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine = WorkingAnnotation?.CombinedLines[0];
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
                an.QualityID = (Guid?)e.Value;
                an.IsChanged = true;
                //AgeReadingViewModel.SaveAnnotations();
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
        //    if (e.Column.FieldName == "IsApproved")
        //    {
        //        var newValue = !(bool)e.Value;
        //        var an = Outcomes.FirstOrDefault(x => x.ID == ((Annotation)e.Row).ID);
        //        if (an == null)
        //        {
        //            e.Cancel = true;
        //            return;
        //        }
        //        if (an.QualityID != Qualities.FirstOrDefault(x => x.Code == "AQ1").ID && WebAPI.Settings.RequireAq1ForApproval && newValue)
        //        {
        //            new WinUIMessageBoxService().Show("Can not approve an Annotation without an AQ1 value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            e.Cancel = true;
        //            return;
        //        }
        //        if (newValue)
        //        {
        //            foreach (var outcome in Outcomes)
        //            {
        //                if (outcome.IsApproved) outcome.IsChanged = true;
        //                outcome.IsApproved = false;
        //            }

        //            an.IsApproved = true;
        //            an.IsChanged = true;
        //        }
        //        else
        //        {
        //            an.IsApproved = false;
        //            an.IsChanged = true;
        //        }
        //        AgeReadingViewModel.SaveAnnotations();

        //        //Helper.DoAsync(SetSampleStatus);

        //        //hack
        //        e.Column.AllowFocus = false;
        //        e.Column.AllowFocus = true;
        //        if (newValue)
        //        {
        //            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.AllowEditing = false;
        //        }
        //    }
        //}
        public void TogglePinAnnotation()
        {
            if(CanPin) PinAnnotation();
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

            WorkingAnnotation.IsChanged = true;

            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
            RefreshActions();
            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.RefreshData();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationGrid.InvalidateVisual();
            //AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.UpdateLayout();
            AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();

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
            if(CanApprove) Approve();
            else
            {
                DisApprove();
            }
        }

        public void Approve()
        {
            foreach (var outcome in Outcomes)
            {
                if (outcome.IsApproved) outcome.IsChanged = true;
                outcome.IsApproved = false;
            }

            WorkingAnnotation.IsApproved = true;
            WorkingAnnotation.IsChanged = true;

            AgeReadingViewModel.SaveAnnotations();
            AgeReadingViewModel.AgeReadingAnnotationView.AnnotationList.AllowEditing = false;
            AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.EmptyStacks();
            RefreshActions();
            AgeReadingViewModel.AgeReadingEditorViewModel.UpdateButtons();
        }

        public void DisApprove()
        {
            WorkingAnnotation.IsApproved = false;
            WorkingAnnotation.IsChanged = true;
            AgeReadingViewModel.SaveAnnotations();
            RefreshActions();
        }

        public void AnnotationList_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
