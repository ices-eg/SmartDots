using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for MaturityEditorView.xaml
    /// </summary>
    public partial class MaturityEditorView : UserControl
    {
        private MaturityEditorViewModel maturityEditorViewModel;

        public MaturityEditorViewModel MaturityEditorViewModel
        {
            get { return maturityEditorViewModel; }
            set { maturityEditorViewModel = value; }
        }

        public MaturityEditorView(MaturityViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityEditorViewModel = (MaturityEditorViewModel)base.DataContext;
            maturityEditorViewModel.MaturityViewModel = maturityViewModel;

            //DeleteBtn.Checked += DeleteBtn_Checked;

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
            //maturityEditorViewModel.MaturityViewModel.MaturityEditorView.ScrollViewer.Width = maturityEditorViewModel
            //    .MaturityViewModel.MaturityEditorView.MaturityDockLayoutManager.ActualWidth;
            //maturityEditorViewModel.MaturityViewModel.MaturityEditorView.ScrollViewer.Height = maturityEditorViewModel
            //    .MaturityViewModel.MaturityEditorView.MaturityDockLayoutManager.ActualHeight;
            maturityEditorViewModel.ScrollViewer_SizeChanged(null, null);
        }

        //private void PopupMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!maturityEditorViewModel.IsContextmenuVisible)
        //    {
        //        e.Cancel = true;
        //    }
        //}
    }
}
