using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for LarvaeSampleView.xaml
    /// </summary>
    public partial class LarvaeSampleView : UserControl
    {
        private LarvaeSampleViewModel maturitySampleViewModel;

        public LarvaeSampleViewModel LarvaeSampleViewModel
        {
            get { return maturitySampleViewModel; }
            set { maturitySampleViewModel = value; }
        }

        public LarvaeSampleView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            maturitySampleViewModel = (LarvaeSampleViewModel) base.DataContext;
            maturitySampleViewModel.LarvaeViewModel = maturityViewModel;
        }

        private void LarvaeSampleGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SampleList.BestFitColumns();

        }

        private void LarvaeSampleGrid_EndGrouping(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeSampleViewModel.RefreshNavigationButtons();
        }

        private void LarvaeSampleGrid_EndSorting(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeSampleViewModel.RefreshNavigationButtons();
        }

        private void LarvaeSampleGrid_FilterChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeSampleViewModel.RefreshNavigationButtons();
        }
    }
}
