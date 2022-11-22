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
    /// Interaction logic for LarvaeStatusbarView.xaml
    /// </summary>
    public partial class LarvaeStatusbarView : UserControl
    {
        private LarvaeStatusbarViewModel maturityStatusbarViewModel;

        public LarvaeStatusbarViewModel LarvaeStatusbarViewModel
        {
            get { return maturityStatusbarViewModel; }
            set { maturityStatusbarViewModel = value; }
        }

        public LarvaeStatusbarView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityStatusbarViewModel = (LarvaeStatusbarViewModel)base.DataContext;
            maturityStatusbarViewModel.LarvaeViewModel = maturityViewModel;
        }
    }
}
