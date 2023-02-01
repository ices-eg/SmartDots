using System.Windows;
using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for LarvaeEditorView.xaml
    /// </summary>
    public partial class LarvaeEditorView : UserControl
    {
        private LarvaeEditorViewModel maturityEditorViewModel;

        public LarvaeEditorViewModel LarvaeEditorViewModel
        {
            get { return maturityEditorViewModel; }
            set { maturityEditorViewModel = value; }
        }

        public LarvaeEditorView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityEditorViewModel = (LarvaeEditorViewModel)base.DataContext;
            maturityEditorViewModel.LarvaeViewModel = maturityViewModel;

            LineButton.ToggleStateChanged += LineButton_ToggleStateChanged;
            LineButton.ContextButtonClick += (sender, args) => LineMenu.ShowPopup(LineButton);

            DotButton.ToggleStateChanged += DotButton_ToggleStateChanged;
            DotButton.ContextButtonClick += (sender, args) => DotMenu.ShowPopup(DotButton);

            CircleButton.ToggleStateChanged += CircleButton_ToggleStateChanged;
            CircleButton.ContextButtonClick += (sender, args) => CircleMenu.ShowPopup(CircleButton);

            ScaleButton.PrimaryButtonClick += (sender, args) => maturityEditorViewModel.AutoMeasureScale(true);
            ScaleButton.ContextButtonClick += (sender, args) => ScaleContextMenu.ShowPopup(ScaleButton);
            ContextMeasureAutomatic.ItemClick += (sender, args) => maturityEditorViewModel.AutoMeasureScale(true);
            ContextMeasureManual.ItemClick += (sender, args) => maturityEditorViewModel.ManualMeasureScale();
            ContextMeasureDelete.ItemClick += (sender, args) => maturityEditorViewModel.DeleteMeasureScale();

            MeasureButton.PrimaryButtonClick += (sender, args) => maturityEditorViewModel.MeasureTool();
            MeasureButton.ContextButtonClick += (sender, args) => MeasureContextMenu.ShowPopup(MeasureButton);
        }

        private void ScrollViewer_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //maturityEditorViewModel.LarvaeViewModel.LarvaeEditorView.ScrollViewer.Width = maturityEditorViewModel
            //    .LarvaeViewModel.LarvaeEditorView.LarvaeDockLayoutManager.ActualWidth;
            //maturityEditorViewModel.LarvaeViewModel.LarvaeEditorView.ScrollViewer.Height = maturityEditorViewModel
            //    .LarvaeViewModel.LarvaeEditorView.LarvaeDockLayoutManager.ActualHeight;
            maturityEditorViewModel.ScrollViewer_SizeChanged(null, null);
        }

        //private void PopupMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!maturityEditorViewModel.IsContextmenuVisible)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        private void LineButton_ToggleStateChanged(object sender, System.EventArgs e)
        {
            LarvaeEditorViewModel.DrawLineBtn_Checked(sender, new RoutedEventArgs());
        }

        private void DotButton_ToggleStateChanged(object sender, System.EventArgs e)
        {
            LarvaeEditorViewModel.DrawDotBtn_Checked(sender, new RoutedEventArgs());
        }

        private void CircleButton_ToggleStateChanged(object sender, System.EventArgs e)
        {
            LarvaeEditorViewModel.DrawCircleBtn_Checked(sender, new RoutedEventArgs());
        }
    }
}
