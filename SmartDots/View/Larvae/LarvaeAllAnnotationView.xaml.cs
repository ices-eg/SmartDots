using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for LarvaeAllAnnotationView.xaml
    /// </summary>
    public partial class LarvaeAllAnnotationView : UserControl
    {
        private LarvaeAllAnnotationViewModel maturityAllAnnotationViewModel;

        public LarvaeAllAnnotationViewModel LarvaeAllAnnotationViewModel
        {
            get { return maturityAllAnnotationViewModel; }
            set { maturityAllAnnotationViewModel = value; }
        }

        public LarvaeAllAnnotationView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityAllAnnotationViewModel = (LarvaeAllAnnotationViewModel) base.DataContext;
            maturityAllAnnotationViewModel.LarvaeViewModel = maturityViewModel;
        }

        private void LarvaeAnnotationGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AnnotationList.BestFitColumns();
        }
    }
}
