using System;
using System.Windows.Input;

namespace SmartDots.ViewModel
{
    public class SelectAnalysisDialogViewModel : AgeReadingBaseViewModel
    {
        private int number;
        private bool isValid;

        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                RaisePropertyChanged("Number");
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

        public void Select()
        {
            //var analysis = WebAPI.GetAnalysis(null, Number);
            //if (analysis != null)
            //{
            //    AgeReadingViewModel.Analysis = analysis;
            //    AgeReadingViewModel.SelectAnalysisDialog.DialogResult = true;
            //    CloseDialog(AgeReadingViewModel.SelectAnalysisDialog);
            //}
            //else
            //{
            //    Helper.ShowWinUIMessageBox("Analysis not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        public void Cancel()
        {
            AgeReadingViewModel.SelectAnalysisDialog.DialogResult = false;
            CloseDialog(AgeReadingViewModel.SelectAnalysisDialog);
        }

        public void Validate(DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            int num;
            bool res = int.TryParse(e.NewValue.ToString(), out num);
            if (!String.IsNullOrWhiteSpace(e.NewValue.ToString()) && res) IsValid = true;
        }

        public void AnalysisChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            Validate(e);
        }

        public void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && IsValid)
            {
                Select();
            }
        }
    }
}
