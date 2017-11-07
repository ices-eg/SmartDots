using System.Windows.Media;

namespace SmartDots.View.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsButton.xaml
    /// </summary>
    public partial class WindowButton
    {
        public SolidColorBrush BackgroundDefaultColor { get; set; } = new SolidColorBrush(Color.FromRgb(200, 225, 230));
        public SolidColorBrush BackgroundHoverColor { get; set; } = new SolidColorBrush(Color.FromRgb(234, 225, 143));

        public WindowButton()
        {
            InitializeComponent();

            MouseEnter += WindowButton_MouseEnter;
            MouseLeave += WindowButton_MouseLeave;
            Loaded += WindowButton_Loaded;
        }

        private void WindowButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Background = BackgroundDefaultColor;
        }

        private void WindowButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Background = BackgroundDefaultColor;
        }

        private void WindowButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Background = BackgroundHoverColor;
        }
    }
}
