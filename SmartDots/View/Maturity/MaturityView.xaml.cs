using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using SmartDots.Helpers;
using SmartDots.ViewModel;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for AgeReadingView.xaml
    /// </summary>
    public partial class MaturityView : UserControl
    {
        private MaturityViewModel maturityViewModel;
        private MainWindowViewModel mainWindowViewModel;

        public MaturityViewModel MaturityViewModel
        {
            get { return maturityViewModel; }
            set { maturityViewModel = value; }
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return mainWindowViewModel; }
            set { mainWindowViewModel = value; }
        }

        Point locationOfYourControl;

        public MaturityView(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            this.mainWindowViewModel = mainWindowViewModel;

            maturityViewModel = (MaturityViewModel) base.DataContext;
            MaturityViewModel.MaturityView = this;

            MaturityViewModel.LoadLayout("MaturityLayout.xml");


            ////this order is important
            //Show();
            //WindowState = WindowState.Maximized;


        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            MaturityViewModel.MaturityOwnAnnotationViewModel.ToggleApprove();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            
            MaturityViewModel.MaturitySampleView.SampleList.MovePrevRow();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MaturityViewModel.MaturitySampleView.SampleList.MoveNextRow();
        }

        private void FilePrevious_Click(object sender, RoutedEventArgs e)
        {
            MaturityViewModel.MaturityFileView.FileList.MovePrevCell();
        }

        private void FileNext_Click(object sender, RoutedEventArgs e)
        {
            FileNext.UpdateLayout();
            MaturityViewModel.MaturityFileView.FileList.MoveNextCell();
        }

        private void FileNext_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            MaturityViewModel.MaturitySampleViewModel.DownloadImages();
        }


        //private void DockLayoutManager_OnDockOperationCompleted(object sender, DockOperationCompletedEventArgs e)
        //{
        //    try
        //    {
        //        ((DockLayoutManager)sender).ActiveDockItem.MinWidth = 100;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
