using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for LarvaeAllAnnotationView.xaml
    /// </summary>
    public partial class LarvaeAllAnnotationView : UserControl
    {
        private LarvaeAllAnnotationViewModel larvaeAllAnnotationViewModel;

        public LarvaeAllAnnotationViewModel LarvaeAllAnnotationViewModel
        {
            get { return larvaeAllAnnotationViewModel; }
            set { larvaeAllAnnotationViewModel = value; }
        }

        public LarvaeAllAnnotationView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            larvaeAllAnnotationViewModel = (LarvaeAllAnnotationViewModel) base.DataContext;
            larvaeAllAnnotationViewModel.LarvaeViewModel = maturityViewModel;
        }

        private void LarvaeAnnotationGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AnnotationList.BestFitColumns();
        }

        private void AnnotationGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //var tmp = LarvaeAllAnnotationViewModel.SelectedLarvaeAnnotationParameterResults;
            //foreach (var p in LarvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult)
            //{
            //    var selected = tmp.FirstOrDefault(x => x.ID == p.ID);

            //    p.IsVisible = selected != null;
            //}
            //LarvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeOwnAnnotationViewModel.RaisePropertyChanged("Annotation");
            //LarvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();

            larvaeAllAnnotationViewModel.LarvaeViewModel.LarvaeEditorViewModel.RefreshShapes();
        }

        private void LarvaeAnnotationOverviewGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AnnotationOverviewList.BestFitColumns();
        }
    }
}
