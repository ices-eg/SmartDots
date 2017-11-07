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
using System.Windows.Forms;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for SelectServerDialog.xaml
    /// </summary>
    public partial class SelectAnalysisDialog : WinUIDialogWindow
    {
        private SelectAnalysisDialogViewModel selectAnalysisDialogViewModel;

        public SelectAnalysisDialogViewModel SelectAnalysisDialogViewModel
        {
            get { return selectAnalysisDialogViewModel; }
            set { selectAnalysisDialogViewModel = value; }
        }

        public SelectAnalysisDialog(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            selectAnalysisDialogViewModel = (SelectAnalysisDialogViewModel)base.DataContext;
            selectAnalysisDialogViewModel.AgeReadingViewModel = ageReadingViewModel;
            NumberAnalysis.Focus();
        }
    }
}
