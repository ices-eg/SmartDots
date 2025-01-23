using SmartDots.ViewModel.AgeReading;
using SmartDots.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SmartDots.Helpers;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AgeReadingKeyMappingView : System.Windows.Window
    {

        private AgeReadingKeyMappingViewModel ageReadingKeyMappingViewModel;

        public AgeReadingKeyMappingViewModel AgeReadingKeyMappingViewModel
        {
            get { return ageReadingKeyMappingViewModel; }
            set { ageReadingKeyMappingViewModel = value; }
        }

        public AgeReadingKeyMappingView(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            ageReadingKeyMappingViewModel = (AgeReadingKeyMappingViewModel)base.DataContext;
            ageReadingKeyMappingViewModel.AgeReadingViewModel = ageReadingViewModel;
            LoadKeyBindings();
        }

        private void LoadKeyBindings()
        {
            try
            {
                var kb = AgeReadingKeyMappingViewModel.AgeReadingViewModel.AgeReadingEditorViewModel.ShortcutActions;
                for (int i = 1; i <= 24; i++)
                {
                    var label = (Label)this.FindName($"Lb{i}");
                    if (label != null)
                    {
                        label.Content = kb[i].Item2;
                    }

                    var textBox = (System.Windows.Controls.TextBox)this.FindName($"Tb{i}");
                    if (textBox != null)
                    {
                        textBox.Text = kb[i].Item3;
                    }
                }


            }
            catch (Exception e)
            {

                
            }
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var tb = ((System.Windows.Controls.TextBox)sender);
            var val = e.Key.ToString();
            if (val == "Escape")
            {
                tb.Text = "";
                //((TextBox)sender).
            }
            else
            {
                foreach (System.Windows.Controls.TextBox textbox in FindVisualChildren<System.Windows.Controls.TextBox>(this))
                {
                    if (textbox.Tag == tb.Tag) continue;
                    if (textbox.Text == e.Key.ToString())
                    {
                        textbox.Text = "";
                    }

                }
                tb.Text = e.Key.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var evm = AgeReadingKeyMappingViewModel.AgeReadingViewModel.AgeReadingEditorViewModel;
            for (int i = 1; i <= 24; i++)
            {
                var textBox = (System.Windows.Controls.TextBox)this.FindName($"Tb{i}");
                if (textBox != null)
                {
                    evm.ShortcutActions[i] = new Tuple<Action, string, string>(
                        evm.ShortcutActions[i].Item1,
                        evm.ShortcutActions[i].Item2,
                        textBox.Text
                    );
                }
            }

            AgeReadingKeyMappingViewModel.AgeReadingViewModel.SaveUserPreferences();

            this.Close();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
    }
}
