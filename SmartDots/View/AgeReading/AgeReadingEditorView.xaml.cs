using System.Windows;
using System.Windows.Controls;
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
    }
}
