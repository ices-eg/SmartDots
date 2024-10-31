using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.Model.Extension;
using SmartDots.Model.Events;
using SmartDots.Model.Security;
using SmartDots.ViewModel;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace SmartDots.View
{
    /// <summary>
    /// Interaction logic for ServerSelectionView.xaml
    /// </summary>
    public partial class ServerSelectionView
    {
        //public const string AuthNone = "None";
        public const string AuthWindows = "Windows Authentication";
        public const string AuthUser = "User credentials";
        public const string AuthToken = "Login token";

        public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

        public event ConnectionEventHandler Connected;

        public delegate void AnalysisEventHandler(object sender, AnalysisEventArgs e);

        public event AnalysisEventHandler AnalysisSelected;

        public event EventHandler Disconnected;
        private bool _isLoggedIn;
        private GridSortInfo analysisSortInfo;
        private dynamic selectedItem;
        private MainWindowViewModel mainWindowViewModel;

        public ObservableCollection<ComboBoxItem> ApiList { get; set; } = new ObservableCollection<ComboBoxItem>();

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            private set
            {
                var size = new Size(ActualWidth, ActualHeight);
                if (value != _isLoggedIn)
                    if (value) TransitionToAnalyses(size);
                    else TransitionToLogin(size);
                _isLoggedIn = value;
            }
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return mainWindowViewModel; }
            set { mainWindowViewModel = value; }
        }

        public ServerSelectionView(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            this.mainWindowViewModel = mainWindowViewModel;

            FieldAuth.SelectionChanged += Auth_SelectionChanged;
            SizeChanged += ServerSelectionView_SizeChanged;

            VersionLabel.Content = $"v{Helper.Version.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}";

            LoadChoices();
        }

        public void SaveChoices()
        {
            Properties.Settings.Default.LastAuthType = FieldAuth.Text;
            Properties.Settings.Default.LastUser = FieldUser.Text;
            //MEMO: never store/load password

            var api = FieldApi.Text;
            if (!FieldApi.ListItemsText.Contains(api)) FieldApi.AddListItem(api);
            else Properties.Settings.Default.LastApi = api;
            var c = new StringCollection();
            c.AddRange(FieldApi.ListItemsText.ToArray());
            Properties.Settings.Default.AllApis = c;

            Properties.Settings.Default.Save();
        }

        private void LoadChoices()
        {
            FieldAuth.ItemsSource = new HashSet<string> { AuthWindows, AuthUser, AuthToken };
            var lastAuth = Properties.Settings.Default.LastAuthType;
            FieldAuth.SelectedItem = string.IsNullOrWhiteSpace(lastAuth) ? AuthToken : lastAuth;

            if (Properties.Settings.Default.AllApis != null)
            {
                foreach (var c in Properties.Settings.Default.AllApis) FieldApi.AddListItem(c);
                FieldApi.Text = Properties.Settings.Default.LastApi;
            }
            FieldUser.Text = Properties.Settings.Default.LastUser;
        }

        public async void Login()
        {
            SetControlsEnabledState(false);
            SaveChoices();

            var c = new ConnectionInfo();
            c.ApiUrl = FieldApi.Text;
            c.AuthenticationType = FieldAuth.Text;
            c.UserName = FieldUser.Text;
            c.Password = FieldPassword.Password;
            c = await HandleConnect(c);

            SetControlsEnabledState(true);

            if (c.ConnectionSucceeded)
            {
                if (!LoadSettings())
                {
                    LabelConnecting.Content = string.Empty;
                    return;
                }
                //if (!LoadQualities())
                //{
                //    LabelConnecting.Content = string.Empty;
                //    return;
                //}
                IsLoggedIn = true;
                MainWindowViewModel.HeaderBackBtnIsVisible = true;
                MainWindowViewModel.IsIcesApi = Global.API.Connection.ToLower().Contains("ices.dk");
                MainWindowViewModel.MainWindow.ClearCacheEvent.Content = $@"Delete downloaded images for selected {Global.API.Settings.EventAlias}...";
                MainWindowViewModel.MainWindow.ClearCacheEvent.IsVisible = true;
                LabelConnecting.Content = string.Empty;
                Background = Brushes.White;
                FieldPassword.Clear();


                LoadGrid();
            }
            else PrintError(c.ErrorMessage);

            var cea = new ConnectionEventArgs();
            cea.ConnectionInfo = c;
            cea.UserInfo = Global.API.CurrentUser;
            Connected?.Invoke(this, cea);
        }

        public bool LoadSettings()
        {
            var settings = Global.API.GetSettings();
            if (!settings.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error loading SmartDots settings from the Web API\n" + settings.ErrorMessage, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            Global.API.Settings = settings.Result;
            if (Global.API.Settings.MinRequiredVersion > Helper.Version)
            {
                Helper.ShowWinUIMessageBox($"The minimum supported version of SmartDots is {Global.API.Settings.MinRequiredVersion.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}.\nPlease install the latest version.\nhttps://github.com/ices-eg/SmartDots", "Incompatible version", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            MainWindowViewModel.ApplySettings();
            return true;
        }



        private async Task<ConnectionInfo> HandleConnect(ConnectionInfo c)
        {
            var tt = new ThreadTuple();
            tt.BeginAnimation += (o, args) => AnimateConnection("Connecting", tt.CancelToken);
            tt.BeginAction += (o, args) => c.ErrorMessage = DoConnect(c).ErrorMessage;
            tt.OnError += (o, args) => PrintError(((Exception)args.Value).Message);
            await tt.Start();

            return c;
        }

        private WebApiResult<DtoUser> DoConnect(ConnectionInfo c)
        {
            var auth = new DtoUserAuthentication();
            if (c.ApiUrl.ToLower().Trim() == "offline")
            {
                //Global.API = new 
            }
            else
            {
                Global.API = new WebAPI();
            }
            var connectionAttempt = Global.API.EstablishConnection(c.ApiUrl);
            if (!connectionAttempt.Succeeded)
                return new WebApiResult<DtoUser> { ErrorMessage = connectionAttempt.ErrorMessage };

            switch (c.AuthenticationType)
            {
                //case AuthNone:
                //    //TODO
                //    return new WebApiResult<DtoUser>
                //    {
                //        ErrorMessage = "This authorization type has not yet been implemented."
                //    };
                //    break;

                case AuthWindows:
                    auth.DtoAuthenticationMethod = DtoAuthenticationMethod.Windows;
                    auth.Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    break;

                case AuthUser:
                    auth.DtoAuthenticationMethod = DtoAuthenticationMethod.Basic;
                    auth.Username = c.UserName?.Trim();
                    auth.Password = c.Password?.Trim();
                    break;

                case AuthToken:
                    auth.DtoAuthenticationMethod = DtoAuthenticationMethod.Token;
                    auth.Username = c.UserName?.Trim();
                    break;
            }

            var dtoUser = Global.API.Authenticate(auth);
            if (!dtoUser.Succeeded)
            {
                Helper.ShowWinUIMessageBox(dtoUser.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!string.IsNullOrEmpty(dtoUser.WarningMessage))
            {
                Helper.ShowWinUIMessageBox(dtoUser.WarningMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return dtoUser;
        }

        public void LoadGrid()
        {
            Global.CurrentEventType = "";
            var user = Global.API.CurrentUser;
            if (user == null)
            {
                return;
            }

            var itemsResult = Global.API.GetAnalysesDynamic();
            if (!itemsResult.Succeeded || itemsResult.Result == null)
            {
                Helper.ShowWinUIMessageBox("Error loading Analyses from the Web API\n" + itemsResult.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (!itemsResult.Result.Any())
            {
                Analyses.ItemsSource = null;
                return;
            }

            var selecteditem = Analyses.SelectedItem;
            if (selecteditem != null) selectedItem = selecteditem;

            Dictionary<string, string> values = itemsResult.Result[0].ToObject<Dictionary<string, string>>();
            List<string> toReturn = values.Keys.ToList();

            string idKey = toReturn.FirstOrDefault(x => x.ToUpper().Equals("ID"));

            foreach (var item in itemsResult.Result)
            {
                try
                {
                    Guid id;
                    Guid.TryParse(item[idKey].ToString(), out id);
                    if (id == Guid.Empty)
                    {
                        Helper.ShowWinUIMessageBox("Not all Analyses have an ID", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                        return;
                    }
                    item.ID = id;
                }
                catch (Exception e)
                {
                    return;
                }
                item.IsAvailableOffline = false;
            }

            List<GridColumn> columns = new List<GridColumn>();
            columns.AddRange(toReturn.Where(x => !x.ToUpper().Equals("ID")).Select(columnName => new GridColumn() { FieldName = columnName, HeaderTemplate = Resources["ColumnHeaderTemplate"] as DataTemplate, AllowSorting = DefaultBoolean.True, AllowBestFit = DefaultBoolean.True }));
            //columns.Add(new GridColumn() { Header = "Is Available Offline", FieldName = "IsAvailableOffline", UnboundType = UnboundColumnType.Boolean, Style = Resources["AnalysisColumn"] as Style, AllowSorting = DefaultBoolean.True });

            Analyses.ItemsSource = itemsResult.Result;

            Analyses.ColumnsSource = columns;

            if (analysisSortInfo != null)
            {
                Analyses.SortInfo.Clear();
                Analyses.SortInfo.Add(analysisSortInfo);
            }
            if (selectedItem != null)
            {
                var item = ((List<dynamic>)Analyses.ItemsSource).FirstOrDefault(x => x.Number == selectedItem.Number);
                Analyses.SelectedItem = item;
            }

            if (values.Keys.Any(x => x == "Closed"))
            {
                Analyses.FilterString = "[Closed] <> 'Event is public'";
            }

            AnalysesView.BestFitColumns();


            //Analyses.ColumnsSource = typeof(AnalysisRowObject).GetProperties()
            //.Where(x => !x.GetGetMethod()?.IsVirtual & !x.Name.Equals("AnalysisParameters") ?? false)
            //.Select(x => new GridColumn { FieldName = x.Name, Visible = !x.Name.EndsWith("ID") })
            //.ToList();
        }

        private void Finish()
        {
            var user = Global.API.CurrentUser;
            if (user == null)
            {
                return;
            }

            var singleRowSelected = Analyses.DataController?.Selection.Count == 1;
            if (!singleRowSelected) return;

            var selection = Analyses.DataController.Selection;
            var ae = new AnalysisEventArgs();
            var rowIndex = selection.GetSelectedRows()[0];
            ae.Analysis = Analyses.GetRow(rowIndex);
            var apiResult = new WebApiResult<bool>();

            if (ae.Analysis.Purpose != null)
            {
                switch (ae.Analysis.Purpose.Value.ToString().ToLower().Substring(0, 3))
                {
                    // todo error handling

                    case "mat":
                        apiResult = Global.API.ToggleMaturityAnalysisUserProgress((Guid)ae.Analysis.ID);
                        break;
                    case "lar":
                        apiResult = Global.API.ToggleLarvaeAnalysisUserProgress((Guid)ae.Analysis.ID, "Larvae");
                        break;
                    case "egg":
                        apiResult = Global.API.ToggleLarvaeAnalysisUserProgress((Guid)ae.Analysis.ID, "Egg");
                        break;
                    default:
                        apiResult = Global.API.ToggleAnalysisUserProgress((Guid)ae.Analysis.ID);
                        break;
                }
            }
            else
            {
                apiResult = Global.API.ToggleAnalysisUserProgress((Guid)ae.Analysis.ID);
            }
            if (!apiResult.Succeeded)
            {
                Helper.ShowWinUIMessageBox("Error toggling user progress from the Web API\n" + apiResult.ErrorMessage, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            LoadGrid();

        }

        #region Interface

        private void TransitionToLogin(Size screenSize)
        {
            LabelConnecting.Content = string.Empty;
            FieldPassword.Clear();
            Analyses.Visibility = Visibility.Collapsed;
            MainWindowViewModel.HeaderInfo = "";
            AnalysesActions.Visibility = Visibility.Collapsed;

            RedrawLoginScreen(screenSize);
        }
        private void RedrawLoginScreen(Size screenSize)
        {
            Fields.Visibility = Visibility.Visible;
            Analyses.Visibility = Visibility.Collapsed;
            MainWindowViewModel.HeaderInfo = "";
            MainWindowViewModel.HeaderModule = "";
            AnalysesActions.Visibility = Visibility.Collapsed;
            Logo.Visibility = Visibility.Visible;

            var screenWidth = screenSize.Width;
            var screenHeight = screenSize.Height;

            var fieldsWidth = Fields.ActualWidth;
            var fieldsLeft = (screenWidth / 2) - (fieldsWidth / 2);
            var fieldsHeight = Fields.ActualHeight;
            var fieldsTop = (screenHeight / 2) - (fieldsHeight / 2);

            Canvas.SetLeft(Fields, fieldsLeft);
            Canvas.SetTop(Fields, fieldsTop);
        }

        private void TransitionToAnalyses(Size screenSize)
        {
            Fields.Visibility = Visibility.Collapsed;
            Logo.Visibility = Visibility.Collapsed;
            LabelConnecting.Content = "";

            RedrawAnalysisScreen(screenSize);
        }
        private void RedrawAnalysisScreen(Size screenSize)
        {
            Fields.Visibility = Visibility.Collapsed;
            Analyses.Visibility = Visibility.Visible;
            MainWindowViewModel.HeaderInfo = $"  {Global.API.Settings.EventAlias.ToUpper()} OVERVIEW";
            MainWindowViewModel.HeaderModule = "";
            AnalysesActions.Visibility = Visibility.Visible;
            Logo.Visibility = Visibility.Collapsed;

            var screenWidth = screenSize.Width;
            var screenHeight = screenSize.Height;

            var analysesWidth = screenWidth - 20.0;
            var analysesHeight = screenHeight - 100.0;
            var analysesLeft = (screenWidth / 2) - (analysesWidth / 2);
            var analysesTop = 40.0;

            Analyses.Width = analysesWidth;
            Analyses.Height = analysesHeight;
            AnalysesActions.Width = screenWidth;

            Canvas.SetLeft(Analyses, analysesLeft);
            Canvas.SetTop(Analyses, analysesTop);
            Canvas.SetLeft(AnalysesActions, 0);
            Canvas.SetBottom(AnalysesActions, 0);

            Analyses.DataController?.Selection.SetSelected(0, true); //TODO: memorize selected row
        }

        private void SetControlsEnabledState(bool enabled)
        {
            FieldApi.IsEnabled = enabled;
            FieldAuth.IsEnabled = enabled;
            FieldUser.IsEnabled = enabled;
            FieldPassword.IsEnabled = enabled;
            ButtonConnect.IsEnabled = enabled;
        }

        private void UpdateActionButtonStates()
        {
            if (ButtonWorkOnline == null) return;

            var singleRowSelected = Analyses.DataController?.Selection.Count == 1;
            ButtonWorkOnline.IsEnabled = false;
            ButtonFolder.IsEnabled = false;
            ButtonFinished.IsEnabled = false;
            //ButtonPrepareOffline.IsEnabled = Analyses.SelectedCells.All(x => ((AnalysisRowObject)x).IsAvailableOffline); //TODO
            //ButtonWorkOffline.IsEnabled = singleRowSelected;

            if (!singleRowSelected) return;
            ButtonFolder.IsEnabled = true;
            var item = JsonConvert.DeserializeObject<dynamic>(Analyses.SelectedItem.ToString());
            //if (item.Folder != null)
            //{
            ButtonWorkOnline.IsEnabled = true;
            //}
            //if (item.Folder != null)
            //{
            ButtonWorkOnline.IsEnabled = true;
            //}

            if (item.UserProgress != null)
            {
                var finishedAliases = new List<string>();
                finishedAliases.Add("complete");
                finishedAliases.Add("completed");
                finishedAliases.Add("finished");
                finishedAliases.Add("done");
                finishedAliases.Add("ready");
                ButtonFinished.IsEnabled = true;
                if (finishedAliases.Contains(((string)(item.UserProgress.ToString())).ToLower()))
                {
                    ButtonFinished.Label = "Reopen";
                }
                else
                {
                    ButtonFinished.Label = "Finish";
                }
            }
        }

        private void AnimateConnection(string line, CancellationTokenSource cts)
        {
            var i = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                var dots = new string('.', i);
                var s = $"{line}{dots,-4}";
                PrintMessage(s);
                i++;
                if (i > 3) i = 0;

                Task.Delay(300).Wait();
            }
        }

        private void PrintError(string msg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(() =>
            {
                LabelConnecting.Foreground = Brushes.Red;
                LabelConnecting.Content = msg;
            }));
        }
        private void PrintMessage(string msg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(() =>
            {
                LabelConnecting.Foreground = Brushes.Gray;
                LabelConnecting.Content = msg;
            }));
        }

        #endregion

        #region Events

        private void GetToken(object sender, RoutedEventArgs e)
        {
            Global.API.EstablishConnection(FieldApi.Text);
            var webapiresult = Global.API.GetGuestToken();
            if (!webapiresult.Succeeded)
            {
                Helper.ShowWinUIMessageBox(webapiresult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FieldUser.Text = webapiresult.Result;
            Global.API.Reset();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        public void Disconnect()
        {
            MainWindowViewModel.AgeReadingControl.AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes = new ObservableCollection<Model.Annotation>();
            IsLoggedIn = false;
            MainWindowViewModel.HeaderBackBtnIsVisible = false;
            Background = (SolidColorBrush)Application.Current.TryFindResource("BrushSmartFishDarkBlue") ?? Brushes.White;

            MainWindowViewModel.HeaderLogoIsVisible = false;
        }

        private void WorkOnline_Click(object sender, RoutedEventArgs e)
        {
            var selection = Analyses.DataController.Selection;
            if (selection.Count != 1) return;

            var ae = new AnalysisEventArgs();
            var rowIndex = selection.GetSelectedRows()[0];
            ae.Analysis = Analyses.GetRow(rowIndex);
            AnalysisSelected?.Invoke(sender, ae);
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindowViewModel.AgeReadingControl.AgeReadingViewModel.WaitState = true;
                FolderSelectDialog fbd = new FolderSelectDialog();
                if (fbd.ShowDialog(IntPtr.Zero))
                {
                    var folderpath = FolderHelper.GetUniversalPath(fbd.FileName) ?? fbd.FileName;
                    var selection = Analyses.DataController.Selection;
                    if (selection.Count != 1) return;

                    var ae = new AnalysisEventArgs();
                    var rowIndex = selection.GetSelectedRows()[0];
                    ae.Analysis = Analyses.GetRow(rowIndex);
                    var webapiresult = Global.API.UpdateAnalysisFolder((Guid)ae.Analysis.ID, folderpath);
                    if (!webapiresult.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox(webapiresult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    LoadGrid();
                }
            }
            catch (Exception ex)
            {
                Helper.ShowWinUIMessageBox(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void ServerSelectionView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoggedIn) RedrawAnalysisScreen(e.NewSize);
            else RedrawLoginScreen(e.NewSize);
        }

        private void Auth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FieldUser.Clear();
            FieldPassword.Clear();
            switch (e.AddedItems[0]?.ToString())
            {
                case AuthUser:
                    LabelUser.Content = "User";
                    LabelUser.Visibility = Visibility.Visible;
                    LabelPassword.Visibility = Visibility.Visible;
                    FieldUser.Visibility = Visibility.Visible;
                    FieldPassword.Visibility = Visibility.Visible;
                    ButtonToken.Visibility = Visibility.Collapsed;
                    FieldUser.MinWidth = 330;
                    break;
                case AuthToken:
                    LabelUser.Content = "Token";
                    LabelUser.Visibility = Visibility.Visible;
                    LabelPassword.Visibility = Visibility.Collapsed;
                    FieldUser.Visibility = Visibility.Visible;
                    FieldPassword.Visibility = Visibility.Collapsed;
                    ButtonToken.Visibility = Visibility.Visible;
                    FieldUser.MinWidth = 240;
                    break;
                default:
                    LabelUser.Visibility = Visibility.Collapsed;
                    LabelPassword.Visibility = Visibility.Collapsed;
                    FieldUser.Visibility = Visibility.Collapsed;
                    FieldPassword.Visibility = Visibility.Collapsed;
                    ButtonToken.Visibility = Visibility.Collapsed;
                    FieldUser.MinWidth = 330;
                    break;
            }
        }

        private void GridAnalysis_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            UpdateActionButtonStates();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }

        private void ButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            Finish();
        }

        private void Field_KeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Login();
            }
        }

        #endregion

        private void Analyses_StartSorting(object sender, RoutedEventArgs e)
        {
            analysisSortInfo = ((GridControl)sender).SortInfo.FirstOrDefault();
        }
    }
}
