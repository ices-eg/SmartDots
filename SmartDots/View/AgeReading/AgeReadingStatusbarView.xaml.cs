using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingStatusbarView.xaml
    /// </summary>
    public partial class AgeReadingStatusbarView : UserControl
    {
        private AgeReadingStatusbarViewModel ageReadingStatusbarViewModel;

        public AgeReadingStatusbarViewModel AgeReadingStatusbarViewModel
        {
            get { return ageReadingStatusbarViewModel; }
            set { ageReadingStatusbarViewModel = value; }
        }

        public AgeReadingStatusbarView(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            ageReadingStatusbarViewModel = (AgeReadingStatusbarViewModel)base.DataContext;
            ageReadingStatusbarViewModel.AgeReadingViewModel = ageReadingViewModel;
        }
    }
}
