using System.Collections.Generic;
using System.Windows;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.Model.Smartdots;

namespace SmartDots.ViewModel
{
    public class AttachSampleDialogViewModel : AgeReadingBaseViewModel
    {
        private AnalysisSample sample;
        private List<AnalysisSample> samples;
        private bool isValid;

        public AnalysisSample Sample
        {
            get { return sample; }
            set
            {
                sample = value;
                RaisePropertyChanged("Sample");

                IsValid = sample != null;
            }
        }

        public List<AnalysisSample> Samples
        {
            get { return samples; }
            set
            {
                samples = value;
                RaisePropertyChanged("Samples");
            }
        }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                RaisePropertyChanged("IsValid");
            }
        }

        public void Attach()
        {
            File file = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile;
            file.SampleID = Sample.ID;
            file.SampleNumber = Sample.No.ToString();

            var dbfile = (DtoFile)Helper.ConvertType(file, typeof(DtoFile));
            var updateFileResult = WebAPI.UpdateFile(dbfile);
            if (!updateFileResult.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error saving File in Web API\n" + updateFileResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fileResult = WebAPI.GetFile(dbfile.ID, false, true);
            if (!fileResult.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading File from the Web API\n" + fileResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tempfile = (File)Helper.ConvertType(fileResult.Result, typeof(File));
            tempfile.Sample = (Sample) Helper.ConvertType(fileResult.Result.Sample, typeof(Sample));

            file.SampleID = tempfile.SampleID;
            file.Sample = tempfile.Sample;
            file.SampleNumber = tempfile.SampleNumber;
            file.IsReadOnly = tempfile.IsReadOnly;
            file.Scale = tempfile.Scale;
            var dynFile = AgeReadingViewModel.AgeReadingFileViewModel.CreateDynamicFile(file);
            file.FetchProps((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row);
            AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row = dynFile;
            AgeReadingViewModel.AgeReadingSampleViewModel.SetSample();

            AgeReadingViewModel.AgeReadingFileViewModel.UpdateList();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
            CloseDialog(AgeReadingViewModel.AttachSampleDialog);
        }

        public void Cancel()
        {
            CloseDialog(AgeReadingViewModel.AttachSampleDialog);
        }
    }
}
