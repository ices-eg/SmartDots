using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors;
using SmartDots.Helpers;
using SmartDots.Model;

namespace SmartDots.ViewModel
{
    public class EditAnnotationDialogViewModel : AgeReadingBaseViewModel
    {
        private Annotation annotation;
        private AnalysisParameter analysisParameter;
        private List<AnalysisParameter> parameters = new List<AnalysisParameter>();
        private Quality quality;
        private List<Quality> qualities = new List<Quality>();
        private List<VisibilityState> visibilityStates;
        private bool isApproved;
        private VisibilityState edge;
        private VisibilityState nucleus;
        private string comment;
        private string detailString;
        private bool canApprove;
        private bool showNucleusColumn;
        private bool showEdgeColumn;
        private int height = 420;
        private Visibility canApproveAnnotation;
        private Visibility aq3WarningVisibility;
        

        public EditAnnotationDialogViewModel()
        {
            VisibilityStates = new List<VisibilityState>();
            VisibilityStates.Add(new VisibilityState() { Value = "Opaque", Visibility = "Opaque" });
            VisibilityStates.Add(new VisibilityState() { Value = "Translucent", Visibility = "Translucent" });
            VisibilityStates.Add(new VisibilityState() { Value = null, Visibility = "NA" });
            Aq3WarningVisibility = Visibility.Collapsed;
        }

        public Annotation Annotation
        {
            get { return annotation; }
            set
            {
                try
                {
                    annotation = value;
                    Parameters = AgeReadingViewModel.AgeReadingAnnotationViewModel.Parameters;
                    Qualities = AgeReadingViewModel.AgeReadingAnnotationViewModel.Qualities;
                    AnalysisParameter = Parameters.FirstOrDefault(x => x.ID == annotation.ParameterID);
                    Quality = Qualities.FirstOrDefault(x => x.ID == annotation.QualityID);
                    IsApproved = annotation.IsApproved;
                    Edge = VisibilityStates.FirstOrDefault(x => x.Value == annotation.Edge);
                    Nucleus = VisibilityStates.FirstOrDefault(x => x.Value == annotation.Nucleus);
                    Comment = annotation.Comment;
                    DetailString = $"{AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.DisplayName} - {Annotation.LabTechnician} - {Annotation.DateCreation.ToString()}";
                }
                catch (Exception e)
                {
                    Helper.ShowWinUIMessageBox("Error loading annotation data", "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
                }
            }
        }

        public AnalysisParameter AnalysisParameter
        {
            get { return analysisParameter; }
            set
            {
                analysisParameter = value;
                if (analysisParameter == null && WebAPI.Settings.RequireParamForApproval)
                {
                    IsApproved = false;
                }
                RaisePropertyChanged("AnalysisParameter");
                RaisePropertyChanged("CanApprove");
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

        public Quality Quality
        {
            get { return quality; }
            set
            {
                quality = value;
                if (quality == null && WebAPI.Settings.RequireAqForApproval)
                {
                    IsApproved = false;
                }
                if (quality?.Code != "AQ1" && WebAPI.Settings.RequireAq1ForApproval)
                {
                    IsApproved = false;
                }
                
                RaisePropertyChanged("Quality");
                RaisePropertyChanged("CanApprove");
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

        public bool ShowNucleusColumn
        {
            get { return showNucleusColumn; }
            set
            {
                showNucleusColumn = value;
                CalculateHeight();
                RaisePropertyChanged("ShowNucleusColumn");
                RaisePropertyChanged("NucleusColumnVisibility");
            }
        }

        public Visibility NucleusColumnVisibility
        {
            get { return showNucleusColumn == true ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool ShowEdgeColumn
        {
            get { return showEdgeColumn; }
            set
            {
                showEdgeColumn = value;
                CalculateHeight();
                RaisePropertyChanged("ShowEdgeColumn");
                RaisePropertyChanged("EdgeColumnVisibility");
            }
        }

        public Visibility EdgeColumnVisibility
        {
            get { return showEdgeColumn == true ? Visibility.Visible : Visibility.Collapsed; }
        }

        public VisibilityState Edge
        {
            get { return edge; }
            set
            {
                edge = value;
                RaisePropertyChanged("Edge");
            }
        }

        public VisibilityState Nucleus
        {
            get { return nucleus; }
            set
            {
                nucleus = value;
                RaisePropertyChanged("Nucleus");
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

        public bool IsApproved
        {
            get { return isApproved; }
            set
            {
                isApproved = value;
                RaisePropertyChanged("IsApproved");
            }
        }

        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        public bool CanApprove
        {
            get
            {
                if (String.IsNullOrWhiteSpace(WebAPI.Connection)) return false;
                if (AgeReadingViewModel == null) return false;
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return false;
                if (Annotation == null) return false;
                if (Quality == null && WebAPI.Settings.RequireAqForApproval) return false;
                if (Quality != null && Quality.ID != Qualities.FirstOrDefault(x => x.Code == "AQ1").ID && WebAPI.Settings.RequireAq1ForApproval) return false;
                if (AnalysisParameter?.ID == null && WebAPI.Settings.RequireParamForApproval) return false;
                
                return true;

            }
        }

        public string DetailString
        {
            get { return detailString; }
            set
            {
                detailString = value;
                RaisePropertyChanged("DetailString");
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                RaisePropertyChanged("Height");
            }
        }

        public Visibility CanApproveAnnotation
        {
            get { return canApproveAnnotation; }
            set
            {
                canApproveAnnotation = value;
                RaisePropertyChanged("CanApproveAnnotation");
            }
        }

        public Visibility Aq3WarningVisibility
        {
            get { return aq3WarningVisibility; }
            set
            {
                aq3WarningVisibility = value;
                RaisePropertyChanged("Aq3WarningVisibility");
            }
        }

        

        public void CalculateHeight()
        {
            int h = 420;
            if (showNucleusColumn) h += 64;
            if (showEdgeColumn) h += 64;
            if (Aq3WarningVisibility == Visibility.Visible) h += 34;
            Height = h;
        }

        public void QualityList_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            var aq3 = Qualities.FirstOrDefault(x => x.Code.ToLower().Trim().Equals("aq3"));
            if (aq3 != null && (Guid)((Quality)e.NewValue).ID == aq3.ID)
            {
                Aq3WarningVisibility = Visibility.Visible;
            }
            else
            {
                Aq3WarningVisibility = Visibility.Collapsed;
            }
            CalculateHeight();
        }

        public void Save()
        {
            //update the annotation in the annotationlist
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ParameterID = AnalysisParameter?.ID;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.QualityID = Quality?.ID;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Quality = AgeReadingViewModel.AgeReadingAnnotationViewModel.Qualities.FirstOrDefault(x => x.ID == Quality?.ID);

            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Edge = Edge.Value;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Nucleus = Nucleus.Value;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Comment = Comment;

            if (IsApproved)
            {
                //set all outcomes to approved false
                foreach (var outcome in AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes)
                {
                    if (outcome.IsApproved && !WebAPI.Settings.AllowMultipleApprovements)
                    {
                        outcome.IsApproved = false;
                        outcome.IsChanged = true;
                    }
                }
                //approve the selected one
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsApproved = IsApproved;
            }
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CalculateAge();
            //saving uses all data from the annotationlist
            AgeReadingViewModel.SaveAnnotations();

            AgeReadingViewModel.AgeReadingAnnotationViewModel.UpdateList();
            AgeReadingViewModel.EditAnnotationDialog.DialogResult = true;
            CloseDialog(AgeReadingViewModel.EditAnnotationDialog);
            AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes();
        }

        public void Cancel()
        {
            AgeReadingViewModel.EditAnnotationDialog.DialogResult = false;
            CloseDialog(AgeReadingViewModel.EditAnnotationDialog);
        }

        public void ApplySettings()
        {
            if (!WebAPI.Settings.CanApproveAnnotation)
            {
                CanApproveAnnotation = Visibility.Collapsed;
            }

            ShowEdgeColumn = AgeReadingViewModel.Analysis.ShowEdgeColumn;
            ShowNucleusColumn = AgeReadingViewModel.Analysis.ShowNucleusColumn;
        }
    }
}
