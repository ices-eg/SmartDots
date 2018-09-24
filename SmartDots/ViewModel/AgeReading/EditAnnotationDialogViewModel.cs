using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        private bool isApproved;
        private string comment;
        private string detailString;
        private bool canApprove;
        private int height = 420;
        private Visibility canApproveAnnotation;

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

        public void Save()
        {
            //update the annotation in the annotationlist
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ParameterID = AnalysisParameter?.ID;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.QualityID = Quality?.ID;

            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Comment = Comment;

            if (IsApproved)
            {
                //set all outcomes to approved false
                foreach (var outcome in AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes)
                {
                    outcome.IsApproved = false;
                }
                //approve the selected one
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsApproved = IsApproved;
            }
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            //saving uses all data from the annotationlist
            AgeReadingViewModel.SaveAnnotations();

            AgeReadingViewModel.AgeReadingAnnotationViewModel.UpdateList();
            AgeReadingViewModel.EditAnnotationDialog.DialogResult = true;
            CloseDialog(AgeReadingViewModel.EditAnnotationDialog);
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
            
        }
    }
}
