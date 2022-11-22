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

        //public void KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Down && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Down", MessageBoxButton.YesNo).ShowDialog();
        //        //LarvaeSampleViewModel.Next(null, null);
        //    }

        //    if (e.Key == Key.Up && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Up", MessageBoxButton.YesNo).ShowDialog();
        //        //LarvaeSampleViewModel.Previous(null, null);
        //    }

        //    if (e.Key == Key.Right && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Right", MessageBoxButton.YesNo).ShowDialog();
        //        //LarvaeFileViewModel.Next(null, null);
        //    }

        //    if (e.Key == Key.Left && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Left", MessageBoxButton.YesNo).ShowDialog();
        //        //LarvaeFileViewModel.Previous(null, null);
        //    }
        //}

    }
}
