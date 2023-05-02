using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;

namespace SmartDots.ViewModel
{
    public class LarvaeBaseViewModel : INotifyPropertyChanged
    {
        public LarvaeViewModel larvaeViewModel;

        public LarvaeViewModel LarvaeViewModel
        {
            get { return larvaeViewModel; }
            set { larvaeViewModel = value; }
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
            dialog.Close();
            LarvaeViewModel.WaitState = false;
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            dialog.Owner = Window.GetWindow(LarvaeViewModel.LarvaeView);
            LarvaeViewModel.WaitState = true;
            RaisePropertyChanged(null); //triggers propertychanged on all properties
            dialog.ShowDialog();
        }
    }
}
