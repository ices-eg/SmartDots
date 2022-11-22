using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for MaturitySampleView.xaml
    /// </summary>
    public partial class MaturitySampleView : UserControl
    {
        private MaturitySampleViewModel maturitySampleViewModel;

        public MaturitySampleViewModel MaturitySampleViewModel
        {
            get { return maturitySampleViewModel; }
            set { maturitySampleViewModel = value; }
        }

        public MaturitySampleView(MaturityViewModel maturityViewModel)
        {
            InitializeComponent();
            maturitySampleViewModel = (MaturitySampleViewModel) base.DataContext;
            maturitySampleViewModel.MaturityViewModel = maturityViewModel;
        }

        private void MaturitySampleGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SampleList.BestFitColumns();

        }

        private void MaturitySampleGrid_EndGrouping(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturitySampleViewModel.RefreshNavigationButtons();
        }

        private void MaturitySampleGrid_EndSorting(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturitySampleViewModel.RefreshNavigationButtons();
        }

        private void MaturitySampleGrid_FilterChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturitySampleViewModel.RefreshNavigationButtons();
        }
    }
}
