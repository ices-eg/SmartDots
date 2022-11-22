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
    public partial class LarvaeView : UserControl
    {
        private LarvaeViewModel maturityViewModel;
        private MainWindowViewModel mainWindowViewModel;

        public LarvaeViewModel LarvaeViewModel
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

        public LarvaeView(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            this.mainWindowViewModel = mainWindowViewModel;

            maturityViewModel = (LarvaeViewModel) base.DataContext;
            LarvaeViewModel.LarvaeView = this;

            LarvaeViewModel.LoadLayout("LarvaeLayout.xml");


            ////this order is important
            //Show();
            //WindowState = WindowState.Maximized;


        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.ToggleApprove();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            
            LarvaeViewModel.LarvaeSampleView.SampleList.MovePrevRow();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            LarvaeViewModel.LarvaeSampleView.SampleList.MoveNextRow();
        }

        private void FilePrevious_Click(object sender, RoutedEventArgs e)
        {
            LarvaeViewModel.LarvaeFileView.FileList.MovePrevCell();
        }

        private void FileNext_Click(object sender, RoutedEventArgs e)
        {
            FileNext.UpdateLayout();
            LarvaeViewModel.LarvaeFileView.FileList.MoveNextCell();
        }

        private void FileNext_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

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
