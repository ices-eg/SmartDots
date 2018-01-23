using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using SmartDots.Helpers;
using SmartDots.Model.Events;
using SmartDots.Model.Security;
using SmartDots.View;

namespace SmartDots.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _headerLogoIsVisible;
        private string _headerInfo;
        public bool _isListening;

        public string HeaderInfo
        {
            get { return _headerInfo; }
            set
            {
                _headerInfo = value;
                OnPropertyChanged("HeaderInfo");
            }
        }

        //public string Title { get; set; }
        //public string Subtitle { get; set; }

        //public ConnectionInfo CurrentConnection { get; set; }

        public bool HeaderLogoIsVisible
        {
            get { return _headerLogoIsVisible; }
            set
            {
                _headerLogoIsVisible = value;
                OnPropertyChanged(nameof(HeaderLogoIsVisible));
            }
        }

        public ObservableCollection<object> ContentControls { get; } = new ObservableCollection<object>();
        public AgeReadingView SmartDotsControl { get; private set; }
        public ServerSelectionView ServerSelectionView { get; private set; }

        public static string WindowsSecurityConnectionString;
        public static string SecurityConnectionString;
        public static string SmartDotConnectionString;

        public event PropertyChangedEventHandler PropertyChanged;


        public MainWindowViewModel()
        {
            //Title = "Test title";
            //Subtitle = "Test subtitle";
        }

        public void Initialize()
        {
            SmartDotsControl = new AgeReadingView(this);
            LoadServerScreen();
            if (!Connect(null))
            {
                SetActiveControl(ServerSelectionView);
            }
            SmartDotsControl.AgeReadingViewModel.WaitState = false;
        }

        public void SetActiveControl(object ctrl)
        {
            ContentControls.Clear();
            //ctrl.HorizontalAlignment = HorizontalAlignment.Stretch;
            //ctrl.VerticalAlignment = VerticalAlignment.Stretch;
            ContentControls.Add(ctrl);
        }

        public void LoadServerScreen()
        {
            ServerSelectionView = new ServerSelectionView(this);
            ServerSelectionView.Disconnected += Disconnected;
            ServerSelectionView.Connected += Connected;
            ServerSelectionView.AnalysisSelected += AnalysisSelected;

        }

        private void AnalysisSelected(object sender, AnalysisEventArgs e)
        {
            if(SmartDotsControl.AgeReadingViewModel.LoadAnalysis(Guid.Parse(e.Analysis.ID.ToString()))) SetActiveControl(SmartDotsControl);

        }

        public void ApplySettings()
        {
            SmartDotsControl.AgeReadingViewModel.AgeReadingFileViewModel.CanAttachDetachSampleVisibility = WebAPI.Settings.CanAttachDetachSample ? Visibility.Visible : Visibility.Collapsed;
            //SmartDotsControl.AgeReadingViewModel.AgeReadingFileViewModel.CanBrowseFolderVisibility = WebAPI.Settings.CanBrowseFolder ? Visibility.Visible : Visibility.Collapsed;
            ServerSelectionView.ButtonFolder.Visibility = WebAPI.Settings.CanBrowseFolder ? Visibility.Visible : Visibility.Collapsed;
            SmartDotsControl.AgeReadingViewModel.AgeReadingFileViewModel.UseSampleStatus = WebAPI.Settings.UseSampleStatus;
            SmartDotsControl.AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApproveAnnotation = WebAPI.Settings.CanApproveAnnotation;
            SmartDotsControl.AgeReadingViewModel.EditAnnotationDialogViewModel.CanApproveAnnotation = WebAPI.Settings.CanApproveAnnotation ? Visibility.Visible : Visibility.Collapsed;

            var newWindow = !WebAPI.Settings.OpenSocket;
            if (!_isListening && !newWindow)
            {
                AsynchronousSocketListener.AgeReadingViewModel = SmartDotsControl.AgeReadingViewModel;
                Helper.DoAsync(AsynchronousSocketListener.StartListening);
                _isListening = true;
            }
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            if (e.ConnectionInfo.ConnectionSucceeded)
                HeaderLogoIsVisible = true;
        }

        private void Disconnected(object sender, EventArgs eventArgs)
        {
            HeaderLogoIsVisible = false;
        }

        public void StartDotting()
        {
            try
            {
                //var user = SmartDots.Helpers.WebAPI.CurrentUser;
                //TODO: check permission(?)

                var smartDotsControl = new AgeReadingView(this);
                this.ContentControls.Add(smartDotsControl);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public void HandleClosing()
        {
            SmartDotsControl?.AgeReadingViewModel.Save();

            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        public void HandleError(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            while (ex.InnerException != null) sb.AppendLine();
            ShowErrorMessage(sb.ToString());
        }

        public void ShowErrorMessage(Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "An error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
        public void ShowMessage(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, buttons, icon);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Connect(string args = null)
        {
            try
            {
                //args = "analysis;http://srvsqld1:81/api/smartdots/,1fd67e04-eadf-43bf-b7ee-e397da490972,e0774b18-03ea-4e95-a3c8-b82b6f94524b";
                //args = "analysis;http://localhost:63216/api/smartdots/,7C74D726-3F5F-4283-82E9-760E56D818BF,0F4E57BD-6832-40EC-9B14-E2CC0507EFD1";
                if (args == null && App.Args == null)
                {
                    SmartDotsControl.AgeReadingViewModel.FirstLoad = false;
                    return false;
                }

                args = args?.Replace("\\", @"/");
                //Helper.ShowWinUIMessageBox(args, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                //TODO: clean up & refactor entire method

                SmartDotsControl.AgeReadingViewModel.WaitState = true;
                var arguments = new string[2];
                var serverarg = "";
                var userid = "";
                var analysisid = "";
                if (args == null && App.Args != null)
                {
                    arguments = App.Args[0].Split(';')[1].Split(',');
                    serverarg = arguments[0];
                    userid = arguments[1];
                    analysisid = arguments[2];
                }
                else if (args != null)
                {
                    arguments = args.Split(';')[1].Split(',');
                    serverarg = arguments[0];
                    userid = arguments[1];
                    analysisid = arguments[2];
                }

                serverarg = serverarg?.Replace("\\", @"/");
                var auth = new DtoUserAuthentication();
                var connectionAttempt = WebAPI.EstablishConnection(serverarg);
                if (!connectionAttempt.Succeeded)
                {
                    Helper.ShowWinUIMessageBox(connectionAttempt.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }


                auth.DtoAuthenticationMethod = DtoAuthenticationMethod.Windows;
                auth.Username = userid;

                var authentication = WebAPI.Authenticate(auth);
                if (!authentication.Succeeded)
                {
                    Helper.ShowWinUIMessageBox(authentication.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (!ServerSelectionView.LoadSettings())
                {
                    Helper.ShowWinUIMessageBox("Unable to load settings", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (!ServerSelectionView.LoadQualities())
                {
                    Helper.ShowWinUIMessageBox("Unable to load qualities", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (!SmartDotsControl.AgeReadingViewModel.LoadAnalysis(Guid.Parse(analysisid)))
                {
                    Helper.ShowWinUIMessageBox("Unable to load Analysis", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                ServerSelectionView.FieldApi.Text = serverarg;
                ServerSelectionView.SaveChoices();
                SetActiveControl(SmartDotsControl);
                return true;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
                return false;
            }
        }
    }
}
