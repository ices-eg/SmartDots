using System;
using System.Windows;
using System.Windows.Navigation;
using DevExpress.Xpf;
using SmartDots.Helpers;
using SmartDots.ViewModel;
using AboutWindow = SmartDots.View.UserControls.AboutWindow;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; set; }

        private BroadCasterClient broadCasterClient;


        public MainWindow()
        {
            InitializeComponent();

            ViewModel = (MainWindowViewModel)DataContext;

            SizeChanged += MainWindow_SizeChanged;
            Loaded += MainWindow_Loaded;

            ResetLayout.ItemClick += (sender, args) => ViewModel.ResetLayout();
            IcesSharePoint.ItemClick += (sender, args) => ViewModel.OpenIcesSharePoint();
            IcesUserHandbooks.ItemClick += (sender, args) => ViewModel.OpenManualsLink();
            IcesTrainingVideos.ItemClick += (sender, args) => ViewModel.OpenIcesTrainingVideos();
            GitHub.ItemClick += (sender, args) => ViewModel.OpenGitHub();
            About.ItemClick += (sender, args) => OpenAbout();
            
            try
            {
                //WaitState = true
                //var arguments = new string[2];
                //string server = "";
                //Guid? analysisid = null;
                //if (App.Args != null)
                //{
                //    arguments = App.Args[0].Split(';')[1].Split(',');

                //        server = arguments[0];
                //    analysisid = Guid.Parse(arguments[1]);
                //}

                //if (AgeReadingViewModel.Connect(server))
                //{
                //    if (AgeReadingViewModel.Authenticate())
                //    {
                //        AgeReadingViewModel.LoadAnalysis(analysisid);
                //    }
                //}
            }
            catch (Exception e)
            {
                // ignored
            }
            try
            {
                //broadCasterClient = new BroadCasterClient();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void OpenAbout()
        {
            AboutWindow w = new AboutWindow(Helper.Version.ToString().Replace(",","."));
            w.ShowDialog();
            //Window win2 = new Window();
            //win2.
            //win2.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateControls();
            ViewModel.Initialize();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            ShowInTaskbar = true;
            switch (WindowState)
            {
                case WindowState.Maximized:
                    BorderThickness = new Thickness(6);
                    Maximize.Visibility = Visibility.Collapsed;
                    Restore.Visibility = Visibility.Visible;
                    return;

                default:
                    BorderThickness = new Thickness(2);
                    Maximize.Visibility = Visibility.Visible;
                    Restore.Visibility = Visibility.Collapsed;
                    return;
            }
        }


        public void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        public void Restore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        public void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HandleClosing();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private void HelpButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HelpContextMenu.ShowPopup(HelpButton);
        }

        private void SettingsButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SettingsContextMenu.ShowPopup(SettingsButton);
        }
    }
}
