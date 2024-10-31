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
                Lb1.Content = kb[1].Item2;
                Lb2.Content = kb[2].Item2;
                Lb3.Content = kb[3].Item2;
                Lb4.Content = kb[4].Item2;
                Lb5.Content = kb[5].Item2;
                Lb6.Content = kb[6].Item2;
                Lb7.Content = kb[7].Item2;
                Lb8.Content = kb[8].Item2;
                Lb9.Content = kb[9].Item2;
                Lb10.Content = kb[10].Item2;
                Lb11.Content = kb[11].Item2;
                Lb12.Content = kb[12].Item2;
                Lb13.Content = kb[13].Item2;
                Lb14.Content = kb[14].Item2;
                Lb15.Content = kb[15].Item2;
                Lb16.Content = kb[16].Item2;
                Lb17.Content = kb[17].Item2;
                Lb18.Content = kb[18].Item2;

                Tb1.Text = kb[1].Item3;
                Tb2.Text = kb[2].Item3;
                Tb3.Text = kb[3].Item3;
                Tb4.Text = kb[4].Item3;
                Tb5.Text = kb[5].Item3;
                Tb6.Text = kb[6].Item3;
                Tb7.Text = kb[7].Item3;
                Tb8.Text = kb[8].Item3;
                Tb9.Text = kb[9].Item3;
                Tb10.Text = kb[10].Item3;
                Tb11.Text = kb[11].Item3;
                Tb12.Text = kb[12].Item3;
                Tb13.Text = kb[13].Item3;
                Tb14.Text = kb[14].Item3;
                Tb15.Text = kb[15].Item3;
                Tb16.Text = kb[16].Item3;
                Tb17.Text = kb[17].Item3;
                Tb18.Text = kb[18].Item3;

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
            evm.ShortcutActions[1] = new Tuple<Action, string, string>(evm.ShortcutActions[1].Item1, evm.ShortcutActions[1].Item2, Tb1.Text);
            evm.ShortcutActions[2] = new Tuple<Action, string, string>(evm.ShortcutActions[2].Item1, evm.ShortcutActions[2].Item2, Tb2.Text);
            evm.ShortcutActions[3] = new Tuple<Action, string, string>(evm.ShortcutActions[3].Item1, evm.ShortcutActions[3].Item2, Tb3.Text);
            evm.ShortcutActions[4] = new Tuple<Action, string, string>(evm.ShortcutActions[4].Item1, evm.ShortcutActions[4].Item2, Tb4.Text);
            evm.ShortcutActions[5] = new Tuple<Action, string, string>(evm.ShortcutActions[5].Item1, evm.ShortcutActions[5].Item2, Tb5.Text);
            evm.ShortcutActions[6] = new Tuple<Action, string, string>(evm.ShortcutActions[6].Item1, evm.ShortcutActions[6].Item2, Tb6.Text);
            evm.ShortcutActions[7] = new Tuple<Action, string, string>(evm.ShortcutActions[7].Item1, evm.ShortcutActions[7].Item2, Tb7.Text);
            evm.ShortcutActions[8] = new Tuple<Action, string, string>(evm.ShortcutActions[8].Item1, evm.ShortcutActions[8].Item2, Tb8.Text);
            evm.ShortcutActions[9] = new Tuple<Action, string, string>(evm.ShortcutActions[9].Item1, evm.ShortcutActions[9].Item2, Tb9.Text);
            evm.ShortcutActions[10] = new Tuple<Action, string, string>(evm.ShortcutActions[10].Item1, evm.ShortcutActions[10].Item2, Tb10.Text);
            evm.ShortcutActions[11] = new Tuple<Action, string, string>(evm.ShortcutActions[11].Item1, evm.ShortcutActions[11].Item2, Tb11.Text);
            evm.ShortcutActions[12] = new Tuple<Action, string, string>(evm.ShortcutActions[12].Item1, evm.ShortcutActions[12].Item2, Tb12.Text);
            evm.ShortcutActions[13] = new Tuple<Action, string, string>(evm.ShortcutActions[13].Item1, evm.ShortcutActions[13].Item2, Tb13.Text);
            evm.ShortcutActions[14] = new Tuple<Action, string, string>(evm.ShortcutActions[14].Item1, evm.ShortcutActions[14].Item2, Tb14.Text);
            evm.ShortcutActions[15] = new Tuple<Action, string, string>(evm.ShortcutActions[15].Item1, evm.ShortcutActions[15].Item2, Tb15.Text);
            evm.ShortcutActions[16] = new Tuple<Action, string, string>(evm.ShortcutActions[16].Item1, evm.ShortcutActions[16].Item2, Tb16.Text);
            evm.ShortcutActions[17] = new Tuple<Action, string, string>(evm.ShortcutActions[17].Item1, evm.ShortcutActions[17].Item2, Tb17.Text);
            evm.ShortcutActions[18] = new Tuple<Action, string, string>(evm.ShortcutActions[18].Item1, evm.ShortcutActions[18].Item2, Tb18.Text);
            evm.ShortcutActions[19] = new Tuple<Action, string, string>(evm.ShortcutActions[19].Item1, evm.ShortcutActions[19].Item2, Tb19.Text);
            evm.ShortcutActions[20] = new Tuple<Action, string, string>(evm.ShortcutActions[20].Item1, evm.ShortcutActions[20].Item2, Tb20.Text);
            evm.ShortcutActions[21] = new Tuple<Action, string, string>(evm.ShortcutActions[21].Item1, evm.ShortcutActions[21].Item2, Tb21.Text);
            evm.ShortcutActions[22] = new Tuple<Action, string, string>(evm.ShortcutActions[22].Item1, evm.ShortcutActions[22].Item2, Tb22.Text);
            evm.ShortcutActions[23] = new Tuple<Action, string, string>(evm.ShortcutActions[23].Item1, evm.ShortcutActions[23].Item2, Tb23.Text);
            evm.ShortcutActions[24] = new Tuple<Action, string, string>(evm.ShortcutActions[24].Item1, evm.ShortcutActions[24].Item2, Tb24.Text);

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
