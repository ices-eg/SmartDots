using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
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
        private string _headerModule;
        private bool _headerBackBtnIsVisible;
        public bool _isListening;
        public bool _isIcesApi;

        public string HeaderInfo
        {
            get { return _headerInfo; }
            set
            {
                _headerInfo = value;
                OnPropertyChanged("HeaderInfo");
            }
        }

        public string HeaderModule
        {
            get { return _headerModule; }
            set
            {
                _headerModule = value;
                OnPropertyChanged("HeaderModule");
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

        public bool IsIcesApi
        {
            get { return _isIcesApi; }
            set
            {
                _isIcesApi = value;
                OnPropertyChanged(nameof(IsIcesApi));
            }
        }

        public ObservableCollection<object> ContentControls { get; } = new ObservableCollection<object>();
        public AgeReadingView AgeReadingControl { get; private set; }
        public MaturityView MaturityControl { get; private set; }
        public LarvaeView LarvaeControl { get; private set; }
        public ServerSelectionView ServerSelectionView { get; private set; }

        public bool HeaderBackBtnIsVisible
        {
            get { return _headerBackBtnIsVisible; }
            set
            {
                _headerBackBtnIsVisible = value;
                OnPropertyChanged(nameof(HeaderBackBtnIsVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            //Title = "Test title";
            //Subtitle = "Test subtitle";
        }

        public void Initialize()
        {
            Global.API = new WebAPI();
            AgeReadingControl = new AgeReadingView(this);
            MaturityControl = new MaturityView(this);
            LarvaeControl = new LarvaeView(this);
            LoadServerScreen();
            if (!Connect(null))
            {
                SetActiveControl(ServerSelectionView);
            }
            AgeReadingControl.AgeReadingViewModel.WaitState = false;
            MaturityControl.MaturityViewModel.WaitState = false;
            LarvaeControl.LarvaeViewModel.WaitState = false;
        }

        public void SetActiveControl(object ctrl)
        {
            ContentControls.Clear();
            //ctrl.HorizontalAlignment = HorizontalAlignment.Stretch;
            //ctrl.VerticalAlignment = VerticalAlignment.Stretch;
            ContentControls.Add(ctrl);

            if (ctrl.GetType() == typeof(ServerSelectionView))
            {
                var isLoggedIn = ((ServerSelectionView) ctrl).IsLoggedIn;
                if (isLoggedIn)
                {
                    HeaderBackBtnIsVisible = true;
                }
                else
                {
                    HeaderBackBtnIsVisible = false;
                }
                
            }
            else
            {
                HeaderBackBtnIsVisible = true;
            }
        }

        public void GoBack(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ContentControls.Contains(AgeReadingControl))
            {
                AgeReadingControl.AgeReadingViewModel.GoBack();
            }
            else if (ContentControls.Contains(MaturityControl))
            {
                MaturityControl.MaturityViewModel.GoBack();
            }
            else if (ContentControls.Contains(LarvaeControl))
            {
                LarvaeControl.LarvaeViewModel.GoBack();
            }
            else
            {
                ServerSelectionView.Disconnect();
            }
        }

        public void LoadServerScreen()
        {
            ServerSelectionView = new ServerSelectionView(this);
            ServerSelectionView.Connected += Connected;
            ServerSelectionView.AnalysisSelected += AnalysisSelected;

        }

        private void AnalysisSelected(object sender, AnalysisEventArgs e)
        {
            if (e.Analysis.Purpose != null && e.Analysis.Purpose.ToString().ToLower().Substring(0, 3).Equals("mat"))
            {
                if (MaturityControl.MaturityViewModel.LoadMaturityAnalysis(Guid.Parse(e.Analysis.ID.ToString())))
                {
                    SetActiveControl(MaturityControl);
                }

            }
            else if (e.Analysis.Purpose != null && e.Analysis.Purpose.ToString().ToLower().Substring(0, 3).Equals("lar"))
            {
                if (LarvaeControl.LarvaeViewModel.LoadLarvaeAnalysis(Guid.Parse(e.Analysis.ID.ToString()), "Larvae"))
                {
                    SetActiveControl(LarvaeControl);
                }

            }
            else if (e.Analysis.Purpose != null && e.Analysis.Purpose.ToString().ToLower().Substring(0, 3).Equals("egg"))
            {
                if (LarvaeControl.LarvaeViewModel.LoadLarvaeAnalysis(Guid.Parse(e.Analysis.ID.ToString()), "Egg"))
                {
                    SetActiveControl(LarvaeControl);
                }

            }
            else if (AgeReadingControl.AgeReadingViewModel.LoadAnalysis(Guid.Parse(e.Analysis.ID.ToString()))) SetActiveControl(AgeReadingControl);

        }

        public void ApplySettings()
        {
            if (Global.API.Settings.EventAlias == null) Global.API.Settings.EventAlias = "Event";
            if (Global.API.Settings.SampleAlias == null) Global.API.Settings.SampleAlias = "Sample";
            AgeReadingControl.AgeReadingViewModel.SampleAlias = Global.API.Settings.SampleAlias;
            AgeReadingControl.AgeReadingViewModel.AgeReadingFileViewModel.SampleNumberAlias = Global.API.Settings.SampleAlias + " number";
            AgeReadingControl.AgeReadingViewModel.AgeReadingFileViewModel.CanAttachDetachSampleVisibility = Global.API.Settings.CanAttachDetachSample ? Visibility.Visible : Visibility.Collapsed;
            //SmartDotsControl.AgeReadingViewModel.AgeReadingFileViewModel.CanBrowseFolderVisibility = WebAPI.Settings.CanBrowseFolder ? Visibility.Visible : Visibility.Collapsed;
            ServerSelectionView.ButtonFolder.Visibility = Global.API.Settings.CanBrowseFolder ? Visibility.Visible : Visibility.Collapsed;
            ServerSelectionView.ButtonFinished.Visibility = Global.API.Settings.CanMarkEventAsCompleted ? Visibility.Visible : Visibility.Collapsed;
            AgeReadingControl.AgeReadingViewModel.AgeReadingFileViewModel.UseSampleStatus = Global.API.Settings.UseSampleStatus;
            AgeReadingControl.AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApproveAnnotation = Global.API.Settings.CanApproveAnnotation;
            AgeReadingControl.AgeReadingViewModel.EditAnnotationDialogViewModel.CanApproveAnnotation = Global.API.Settings.CanApproveAnnotation ? Visibility.Visible : Visibility.Collapsed;
            var newWindow = !Global.API.Settings.OpenSocket;
            if (!_isListening && !newWindow)
            {
                AsynchronousSocketListener.AgeReadingViewModel = AgeReadingControl.AgeReadingViewModel;
                Helper.DoAsync(AsynchronousSocketListener.StartListening);
                _isListening = true;
            }
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            if (e.ConnectionInfo.ConnectionSucceeded)
                HeaderLogoIsVisible = true;
        }

        //public void StartDotting()
        //{
        //    try
        //    {
        //        //var user = SmartDots.Helpers.WebAPI.CurrentUser;
        //        //TODO: check permission(?)

        //        var smartDotsControl = new AgeReadingView(this);
        //        this.ContentControls.Add(smartDotsControl);
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleError(ex);
        //    }
        //}

        public void HandleClosing()
        {
            AgeReadingControl?.AgeReadingViewModel.Save();
            MaturityControl?.MaturityViewModel.Save();
            LarvaeControl?.LarvaeViewModel.Save();

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
                IsIcesApi = false;
                //args = "analysis;http://srvsqld1:81/api/smartdots/,1fd67e04-eadf-43bf-b7ee-e397da490972,e0774b18-03ea-4e95-a3c8-b82b6f94524b";
                //args = "analysis;http://localhost:63216/api/smartdots/,7C74D726-3F5F-4283-82E9-760E56D818BF,0F4E57BD-6832-40EC-9B14-E2CC0507EFD1";
                if (args == null && App.Args == null)
                {
                    AgeReadingControl.AgeReadingViewModel.FirstLoad = false;
                    return false;
                }

                args = args?.Replace("\\", @"/");
                //Helper.ShowWinUIMessageBox(args, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                //TODO: clean up & refactor entire method

                AgeReadingControl.AgeReadingViewModel.WaitState = true;
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
                var connectionAttempt = Global.API.EstablishConnection(serverarg);
                if (!connectionAttempt.Succeeded)
                {
                    Helper.ShowWinUIMessageBox(connectionAttempt.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }


                auth.DtoAuthenticationMethod = DtoAuthenticationMethod.Windows;
                auth.Username = userid;

                var authentication = Global.API.Authenticate(auth);
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
                //if (!ServerSelectionView.LoadQualities())
                //{
                //    Helper.ShowWinUIMessageBox("Unable to load qualities", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return false;
                //}

                if (!AgeReadingControl.AgeReadingViewModel.LoadAnalysis(Guid.Parse(analysisid)))
                {
                    Helper.ShowWinUIMessageBox("Unable to load Analysis", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                ServerSelectionView.FieldApi.Text = serverarg;
                ServerSelectionView.SaveChoices();
                SetActiveControl(AgeReadingControl);
                return true;
            }
            catch (Exception e)
            {
                //Helper.ShowWinUIMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, e);
                return false;
            }
        }

        public void ResetLayout()
        {
            if (ContentControls.Contains(AgeReadingControl))
            {
                AgeReadingControl.AgeReadingViewModel.LoadLayout("DefaultLayout.xml");
            }
            else if (ContentControls.Contains(MaturityControl))
            {
                MaturityControl.MaturityViewModel.LoadLayout("DefaultMaturityLayout.xml");
            }
            else if (ContentControls.Contains(LarvaeControl))
            {
                LarvaeControl.LarvaeViewModel.LoadLayout("DefaultLarvaeLayout.xml");
            }
        }

        public void OpenManualsLink()
        {
            //System.Diagnostics.Process.Start("http://ices.dk/publications/library/Pages/default.aspx#k=smartdots%20handbook");
            System.Diagnostics.Process.Start("https://ices-library.figshare.com/search?q=%3Atitle%3A%20smartdots&sortBy=publication_date&sortType=desc&groups=37194");
        }

        public void OpenIcesSharePoint()
        {
            System.Diagnostics.Process.Start("https://www.ices.dk/data/tools/Pages/smartdots.aspx");
        }

        public void OpenIcesTrainingVideos()
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCa4bjXo-eBDfW0cm1oElWeQ");
        }
        public void OpenIcesUserFeedback()
        {
            System.Diagnostics.Process.Start("https://smartdots.ices.dk/Userfeedback");
        }

        public void OpenIcesAreas()
        {
            System.Diagnostics.Process.Start("https://raw.githubusercontent.com/ices-eg/SmartDots/master/SmartDots/Resources/ICES-Ecoregions-hybrid-statistical-areas.png");
        }

        public void OpenGitHub()
        {
            System.Diagnostics.Process.Start("https://github.com/ices-eg/SmartDots#readme");
        }

        
    }
}
