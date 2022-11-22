using System;
using System.Windows;
using System.Windows.Media;

namespace SmartDots.View.UserControls
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public string Version { get; set; }
        

        public AboutWindow(string version)
        {
            Version = $"Version {version}";
            InitializeComponent();
            VersionText.Text = Version;

        }

        
    }
}
