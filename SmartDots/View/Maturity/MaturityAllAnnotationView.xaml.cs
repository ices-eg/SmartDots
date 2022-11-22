using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for MaturityAllAnnotationView.xaml
    /// </summary>
    public partial class MaturityAllAnnotationView : UserControl
    {
        private MaturityAllAnnotationViewModel maturityAllAnnotationViewModel;

        public MaturityAllAnnotationViewModel MaturityAllAnnotationViewModel
        {
            get { return maturityAllAnnotationViewModel; }
            set { maturityAllAnnotationViewModel = value; }
        }

        public MaturityAllAnnotationView(MaturityViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityAllAnnotationViewModel = (MaturityAllAnnotationViewModel) base.DataContext;
            maturityAllAnnotationViewModel.MaturityViewModel = maturityViewModel;
        }

        private void MaturityAnnotationGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AnnotationList.BestFitColumns();
        }
    }
}
