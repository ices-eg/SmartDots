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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Layout.Core.Selection;
using SmartDots.Model;
using SmartDots.ViewModel;
using SmartDots.ViewModel.AgeReading;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingAnnotationView.xaml
    /// </summary>
    public partial class MaturityOwnAnnotationView : UserControl
    {
        private MaturityOwnAnnotationViewModel maturityOwnAnnotationViewModel;

        public MaturityOwnAnnotationViewModel MaturityOwnAnnotationViewModel
        {
            get { return maturityOwnAnnotationViewModel; }
            set { maturityOwnAnnotationViewModel = value; }
        }

        public MaturityOwnAnnotationView(MaturityViewModel maturityViewModel)
        {
            InitializeComponent();
            maturityOwnAnnotationViewModel = (MaturityOwnAnnotationViewModel)base.DataContext;
            maturityOwnAnnotationViewModel.MaturityViewModel = maturityViewModel;
        }

        
    }
}
