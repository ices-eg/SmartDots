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
    public class MaturityBaseViewModel : INotifyPropertyChanged
    {
        public MaturityViewModel maturityViewModel;

        public MaturityViewModel MaturityViewModel
        {
            get { return maturityViewModel; }
            set { maturityViewModel = value; }
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
            MaturityViewModel.WaitState = false;
        }

        public void ShowDialog(WinUIDialogWindow dialog)
        {
            dialog.Owner = Window.GetWindow(MaturityViewModel.MaturityView);
            MaturityViewModel.WaitState = true;
            RaisePropertyChanged(null); //triggers propertychanged on all properties
            dialog.ShowDialog();
        }

        //public void KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Down && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Down", MessageBoxButton.YesNo).ShowDialog();
        //        //MaturitySampleViewModel.Next(null, null);
        //    }

        //    if (e.Key == Key.Up && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Up", MessageBoxButton.YesNo).ShowDialog();
        //        //MaturitySampleViewModel.Previous(null, null);
        //    }

        //    if (e.Key == Key.Right && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Right", MessageBoxButton.YesNo).ShowDialog();
        //        //MaturityFileViewModel.Next(null, null);
        //    }

        //    if (e.Key == Key.Left && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //    {
        //        new WinUIDialogWindow("Left", MessageBoxButton.YesNo).ShowDialog();
        //        //MaturityFileViewModel.Previous(null, null);
        //    }
        //}

    }
}
