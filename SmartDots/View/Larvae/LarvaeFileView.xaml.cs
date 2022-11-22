using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for LarvaeSampleView.xaml
    /// </summary>
    public partial class LarvaeFileView : UserControl
    {
        private LarvaeFileViewModel maturityFileViewModel;

        public LarvaeFileViewModel LarvaeFileViewModel
        {
            get { return maturityFileViewModel; }
            set { maturityFileViewModel = value; }
        }

        public LarvaeFileView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityFileViewModel = (LarvaeFileViewModel) base.DataContext;
            maturityFileViewModel.LarvaeViewModel = maturityViewModel;
        }

        private void LarvaeFileGrid_EndGrouping(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeFileViewModel.RefreshNavigationButtons();
        }

        private void LarvaeFileGrid_EndSorting(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeFileViewModel.RefreshNavigationButtons();
        }

        private void LarvaeFileGrid_FilterChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            LarvaeFileViewModel.RefreshNavigationButtons();
        }
    }
}
