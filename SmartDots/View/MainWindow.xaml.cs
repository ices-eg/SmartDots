using System;
using System.Windows;
using System.Windows.Navigation;
using SmartDots.Helpers;
using SmartDots.ViewModel;

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

        

        public void Manual_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ices.dk/publications/library/Pages/default.aspx#k=smartdots%20handbook");
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

    }
}
