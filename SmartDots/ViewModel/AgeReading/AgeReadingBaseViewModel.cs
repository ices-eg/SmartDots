using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.WindowsUI;

namespace SmartDots.ViewModel
{
    public class AgeReadingBaseViewModel : INotifyPropertyChanged
    {
        public AgeReadingViewModel ageReadingViewModel;

        public AgeReadingViewModel AgeReadingViewModel
        {
            get { return ageReadingViewModel; }
            set { ageReadingViewModel = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CloseDialog(WinUIDialogWindow dialog)
        {
            AgeReadingViewModel.AgeReadingView.WinFormBrightness.Visibility = Visibility.Visible;
            AgeReadingViewModel.AgeReadingView.WinFormRedness.Visibility = Visibility.Visible;
            dialog.Close();
            AgeReadingViewModel.WaitState = false;
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            dialog.Owner = Window.GetWindow(AgeReadingViewModel.AgeReadingView);
            AgeReadingViewModel.WaitState = true;
            AgeReadingViewModel.AgeReadingView.WinFormBrightness.Visibility = Visibility.Hidden;
            AgeReadingViewModel.AgeReadingView.WinFormRedness.Visibility = Visibility.Hidden;
            RaisePropertyChanged(null); //triggers propertychanged on all properties
            dialog.ShowDialog();
        }
    }
}
