using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Core;
using SmartDots.Helpers;

namespace SmartDots
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args;

        public App()
        {
            typeof(DXSplashScreen).GetProperty("MainThreadDelay", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, -1, null);
            ApplicationThemeHelper.UseLegacyDefaultTheme = true;
            ApplicationThemeHelper.UpdateApplicationThemeName();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Helper.ShowWinUIMessageBox(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error, ((Exception)e.ExceptionObject));
            MessageBox.Show(((Exception)e.ExceptionObject).StackTrace);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                string[] activationData = AppDomain.CurrentDomain.SetupInformation?.ActivationArguments?.ActivationData;
                if (activationData != null && !activationData[0].StartsWith("file"))
                {
                    Args = activationData;
                }
                else
                {
                    Args = null;
                }
            }
            catch (Exception)
            {

                Args = null;
            }

            base.OnStartup(e);
        }
    }
}
