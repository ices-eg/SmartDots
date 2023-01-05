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
    public partial class LarvaeOwnAnnotationView : UserControl
    {
        private LarvaeOwnAnnotationViewModel larvaeOwnAnnotationViewModel;

        public LarvaeOwnAnnotationViewModel LarvaeOwnAnnotationViewModel
        {
            get { return larvaeOwnAnnotationViewModel; }
            set { larvaeOwnAnnotationViewModel = value; }
        }

        public LarvaeOwnAnnotationView(LarvaeViewModel maturityViewModel)
        {
            InitializeComponent();
            larvaeOwnAnnotationViewModel = (LarvaeOwnAnnotationViewModel)base.DataContext;
            larvaeOwnAnnotationViewModel.LarvaeViewModel = maturityViewModel;
        }

        private void AnnotationGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            larvaeOwnAnnotationViewModel.UpdateSelectionMode();
        }

        private void AnnotationGrid_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            larvaeOwnAnnotationViewModel.UpdateSelectionMode();
        }
    }
}
