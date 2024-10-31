using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SmartDots.Helpers;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingEditorView.xaml
    /// </summary>
    public partial class AgeReadingEditorView : UserControl
    {
        private AgeReadingEditorViewModel ageReadingEditorViewModel;

        public AgeReadingEditorViewModel AgeReadingEditorViewModel
        {
            get { return ageReadingEditorViewModel; }
            set { ageReadingEditorViewModel = value; }
        }

        public AgeReadingEditorView(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            ageReadingEditorViewModel = (AgeReadingEditorViewModel)base.DataContext;
            ageReadingEditorViewModel.AgeReadingViewModel = ageReadingViewModel;

            LineButton.ToggleStateChanged += LineButton_ToggleStateChanged;
            LineButton.ContextButtonClick += (sender, args) => LineMenu.ShowPopup(LineButton);

            DotButton.ToggleStateChanged += DotButton_ToggleStateChanged;
            DotButton.ContextButtonClick += (sender, args) => DotMenu.ShowPopup(DotButton);

            //DeleteBtn.Checked += DeleteBtn_Checked;

            ScaleButton.PrimaryButtonClick += (sender, args) => ageReadingEditorViewModel.AutoMeasureScale(true);
            ScaleButton.ContextButtonClick += (sender, args) => ScaleContextMenu.ShowPopup(ScaleButton);
            ContextMeasureAutomatic.ItemClick += (sender, args) => ageReadingEditorViewModel.AutoMeasureScale(true);
            ContextMeasureManual.ItemClick += (sender, args) => ageReadingEditorViewModel.ManualMeasureScale();
            ContextMeasureDelete.ItemClick += (sender, args) => ageReadingEditorViewModel.DeleteMeasureScale();

            MeasureButton.PrimaryButtonClick += (sender, args) => ageReadingEditorViewModel.MeasureTool();
            MeasureButton.ContextButtonClick += (sender, args) => MeasureContextMenu.ShowPopup(MeasureButton);
        }

        private void DeleteBtn_Checked(object sender, RoutedEventArgs e)
        {
            LineButton.IsPressed = false;
            DotButton.IsPressed = false;
        }

        private void LineButton_ToggleStateChanged(object sender, System.EventArgs e)
        {
            DotButton.IsPressed = false;
            ageReadingEditorViewModel.DrawLineBtn_Checked(sender, new RoutedEventArgs());
        }

        private void DotButton_ToggleStateChanged(object sender, System.EventArgs e)
        {
            LineButton.IsPressed = false;
            ageReadingEditorViewModel.DrawDotBtn_Checked(sender, new RoutedEventArgs());
        }

        private void PopupMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ageReadingEditorViewModel.IsContextmenuVisible)
            {
                e.Cancel = true;
            }
        }

        private void EditDotContextMenu_Closed(object sender, System.EventArgs e)
        {
            AgeReadingEditorViewModel.IsContextmenuOpen = false;
        }

        private void EditDotContextMenu_Opened(object sender, System.EventArgs e)
        {
            AgeReadingEditorViewModel.IsContextmenuOpen = true;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string message = "";

                // Here I'm assuming every requested action modifies the annotation, so I'm checking if the user can edit the annotation
                var canEdit = AgeReadingEditorViewModel.AgeReadingViewModel.AgeReadingAnnotationViewModel.CanEdit;
                if (!canEdit)
                {
                    message = AgeReadingEditorViewModel.AgeReadingViewModel.AgeReadingAnnotationViewModel.EditAnnotationTooltip;
                    Helper.ShowWinUIMessageBox(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!AgeReadingEditorViewModel.ShortcutActions.Any(x => x.Value.Item3 == e.Key.ToString())) return;
                var shortcutAction = AgeReadingEditorViewModel.ShortcutActions.FirstOrDefault(x => x.Value.Item3 == e.Key.ToString());
                var action = shortcutAction.Value.Item1;

                if (action == null) return;

                action.Invoke();

                Tracker.Visibility = Visibility.Hidden;


                //if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) // Is Alt key pressed
                //{
                //if (Keyboard.IsKeyDown(Key.D1) || Keyboard.IsKeyDown(Key.NumPad1))
                //{
                // do something here
                // Helper.ShowWinUIMessageBox("Key pressed: " + e.Key.ToString(), "Key Pressed", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                //}
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
