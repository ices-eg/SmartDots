using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingAnnotationView.xaml
    /// </summary>
    public partial class AgeReadingAnnotationView : UserControl
    {
        private AgeReadingAnnotationViewModel ageReadingAnnotationViewModel;

        public AgeReadingAnnotationViewModel AgeReadingAnnotationViewModel
        {
            get { return ageReadingAnnotationViewModel; }
            set { ageReadingAnnotationViewModel = value; }
        }

        public AgeReadingAnnotationView(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            ageReadingAnnotationViewModel = (AgeReadingAnnotationViewModel) base.DataContext;
            ageReadingAnnotationViewModel.AgeReadingViewModel = ageReadingViewModel;
        }

        
    }
}
