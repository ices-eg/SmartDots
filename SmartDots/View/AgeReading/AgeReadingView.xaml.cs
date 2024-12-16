using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using SmartDots.Helpers;
using SmartDots.ViewModel;
using System.Windows.Input;

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

            GrowthAllMode.EditValue = AgeReadingViewModel.GrowthAllMode;
            GrowthAllScale.EditValue = AgeReadingViewModel.GrowthAllScale;

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

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingFileView.FileList.MovePrevRow();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
        }

        private void GrowthAllMode_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (AgeReadingViewModel == null) return;
            AgeReadingViewModel.GrowthAllMode = e.NewValue.ToString();
            //AgeReadingViewModel.UpdateGraphs(false, false, false, true);
        }

        private void YAxis_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (AgeReadingViewModel == null) return;
            AgeReadingViewModel.GrowthAllScale = e.NewValue.ToString();
            //AgeReadingViewModel.UpdateGraphs(false, false, false, true);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingFileViewModel.DownloadImages();
        }

        private void BrightnessPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.UpdateGraphs(true, false, false, false);
        }

        private void RednessPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.UpdateGraphs(false, true, false, false);
        }

        private void GrowthPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.UpdateGraphs(false, false, false, true);
        }

        private void CsvDownload_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.GrowthCsvDownload();


        }
    }
}
