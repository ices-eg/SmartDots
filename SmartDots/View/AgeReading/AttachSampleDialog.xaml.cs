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
using SmartDots.Model;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AttachSampleDialog.xaml
    /// </summary>
    public partial class AttachSampleDialog : WinUIDialogWindow
    {
        private AttachSampleDialogViewModel attachSampleDialogViewModel;

        public AttachSampleDialogViewModel AttachSampleDialogViewModel
        {
            get { return attachSampleDialogViewModel; }
            set { attachSampleDialogViewModel = value; }
        }

        public AttachSampleDialog(AgeReadingViewModel ageReadingViewModel)
        {
            InitializeComponent();
            attachSampleDialogViewModel = (AttachSampleDialogViewModel)base.DataContext;
            attachSampleDialogViewModel.AgeReadingViewModel = ageReadingViewModel;
        }
    }
}
