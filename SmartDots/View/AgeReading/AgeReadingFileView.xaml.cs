using System.Windows.Controls;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingFileView.xaml
    /// </summary>
    public partial class AgeReadingFileView : UserControl
    {
        private AgeReadingFileViewModel ageReadingFileViewModel;

        public AgeReadingFileViewModel AgeReadingFileViewModel
        {
            get { return ageReadingFileViewModel; }
            set { ageReadingFileViewModel = value; }
        }

        public AgeReadingFileView(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            ageReadingFileViewModel = (AgeReadingFileViewModel) base.DataContext;
            ageReadingFileViewModel.AgeReadingViewModel = ageReadingViewModel;
        }
    }
}
