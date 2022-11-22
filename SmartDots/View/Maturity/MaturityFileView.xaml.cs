using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for MaturitySampleView.xaml
    /// </summary>
    public partial class MaturityFileView : UserControl
    {
        private MaturityFileViewModel maturityFileViewModel;

        public MaturityFileViewModel MaturityFileViewModel
        {
            get { return maturityFileViewModel; }
            set { maturityFileViewModel = value; }
        }

        public MaturityFileView(MaturityViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityFileViewModel = (MaturityFileViewModel) base.DataContext;
            maturityFileViewModel.MaturityViewModel = maturityViewModel;
        }

        private void MaturityFileGrid_EndGrouping(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturityFileViewModel.RefreshNavigationButtons();
        }

        private void MaturityFileGrid_EndSorting(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturityFileViewModel.RefreshNavigationButtons();
        }

        private void MaturityFileGrid_FilterChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            MaturityFileViewModel.RefreshNavigationButtons();
        }
    }
}
