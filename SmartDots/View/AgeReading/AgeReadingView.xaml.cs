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
    public partial class AgeReadingView : UserControl
    {
        private AgeReadingViewModel ageReadingViewModel;
        private MainWindowViewModel mainWindowViewModel;

        public AgeReadingViewModel AgeReadingViewModel
        {
            get { return ageReadingViewModel; }
            set { ageReadingViewModel = value; }
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return mainWindowViewModel; }
            set { mainWindowViewModel = value; }
        }

        Point locationOfYourControl;

        public AgeReadingView(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            this.mainWindowViewModel = mainWindowViewModel;

            ageReadingViewModel = (AgeReadingViewModel) base.DataContext;
            AgeReadingViewModel.AgeReadingView = this;

            AgeReadingViewModel.LoadLayout("Layout.xml");
            AgeReadingViewModel.MakeGraph();

            ////this order is important
            //Show();
            //WindowState = WindowState.Maximized;

            
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
