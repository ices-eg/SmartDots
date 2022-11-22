using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartDots.View.UserControls
{
    public partial class SDFullDropDownButton
    {
        private string _glyph;
        private string _buttonTooltip;
        private bool _isPressed;
        private bool _initialized;

        public static SolidColorBrush EnabledBrush = new SolidColorBrush(Color.FromRgb(41, 60, 69));
        public static SolidColorBrush DisabledBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        public static SolidColorBrush HoverBrush = new SolidColorBrush(Color.FromRgb(240, 80, 38));
        public static SolidColorBrush PressedBrush = new SolidColorBrush(Color.FromRgb(240, 80, 38));
        public static SolidColorBrush MousePressBrush = new SolidColorBrush(Color.FromRgb(255, 183, 112));

        public event EventHandler PrimaryButtonClick;
        public event EventHandler ContextButtonClick;
        public event EventHandler ToggleStateChanged;

        public bool IsToggleButton { get; set; }

        public bool IsPressed
        {
            get { return _isPressed; }
            set
            {
                _isPressed = value;
                UpdateBothButtons();
            }
        }

        public string ButtonToolTip
        {
            get { return _buttonTooltip; }
            set
            {
                _buttonTooltip = value;
                PrimaryButton.ToolTip = value;
            }
        }
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                _glyph = value;
                ButtonImage.Source = new BitmapImage(new Uri(value, UriKind.Relative));
            }
        }

        public SDFullDropDownButton()
        {
            InitializeComponent();

            IsEnabledChanged += SDButton_IsEnabledChanged;

            PrimaryButton.PreviewMouseDown += PrimaryButton_MouseDown;
            PrimaryButton.PreviewMouseUp += PrimaryButton_MouseUp;

            ContextButton.MouseEnter += (sender, args) => UpdateButtonBackground(ContextButton);
            ContextButton.MouseLeave += (sender, args) => UpdateButtonBackground(ContextButton);

            PrimaryButton.MouseEnter += (sender, args) => UpdateButtonBackground(ContextButton);
            PrimaryButton.MouseLeave += (sender, args) => UpdateButtonBackground(ContextButton);


            Loaded += SDButton_Loaded;
        }

        private void SDButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                PrimaryButton.Click += (o, args) => ContextButtonClick?.Invoke(this, new EventArgs());
                ContextButton.Click += (o, args) => ContextButtonClick?.Invoke(this, new EventArgs());
                _initialized = true;
            }

            UpdateBothButtons();
        }


        private void PrimaryButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsToggleButton)
            {
                IsPressed = true;
                ToggleStateChanged?.Invoke(this, new EventArgs());
            }
            else IsPressed = false;

            UpdateBothButtons();
        }

        private void PrimaryButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PrimaryButton.Background = MousePressBrush;
            ContextButton.Background = MousePressBrush;

            PrimaryButton.UpdateLayout();
            ContextButton.UpdateLayout();
            PrimaryButton.InvalidateVisual();
            ContextButton.InvalidateVisual();

            if (!IsToggleButton) IsPressed = true;

            UpdateBothButtons();
        }

        private void SDButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            PrimaryButton.IsEnabled = value;
            ContextButton.IsEnabled = value;

            var brush = value ? EnabledBrush : DisabledBrush;
            PrimaryButton.Background = brush;
            ContextButton.Background = brush;

            PrimaryButton.UpdateLayout();
            ContextButton.UpdateLayout();
            PrimaryButton.InvalidateVisual();
            ContextButton.InvalidateVisual();
            UpdateBorder();
        }

        private void UpdateBothButtons()
        {
            UpdateButtonBackground(PrimaryButton);
            UpdateButtonBackground(ContextButton);
            UpdateBorder();
        }

        public void UpdateButtonBackground(Control btn)
        {
            if (IsPressed && IsToggleButton && IsMouseOver)
            {
                PrimaryButton.Background = HoverBrush;
                ContextButton.Background = HoverBrush;
            }
            else if (IsPressed && IsEnabled)
            {
                PrimaryButton.Background = PressedBrush;
                ContextButton.Background = PressedBrush;
            }
            else if (IsMouseOver)
            {
                PrimaryButton.Background = HoverBrush;
                ContextButton.Background = HoverBrush;
            }
            else if (IsEnabled)
            {
                PrimaryButton.Background = EnabledBrush;
                ContextButton.Background = EnabledBrush;
            }

            if (!(PrimaryButton.IsMouseOver || ContextButton.IsMouseOver) && IsPressed)
            {
                PrimaryButton.Background = HoverBrush;
                ContextButton.Background = HoverBrush;
            }
            else if (!(PrimaryButton.IsMouseOver || ContextButton.IsMouseOver) && !IsPressed)
            {
                PrimaryButton.Background = EnabledBrush;
                ContextButton.Background = EnabledBrush;
            }
            if (!IsEnabled)
            {
                PrimaryButton.Background = DisabledBrush;
                ContextButton.Background = DisabledBrush;
            }

            PrimaryButton.UpdateLayout();
            ContextButton.UpdateLayout();
            PrimaryButton.InvalidateVisual();
            ContextButton.InvalidateVisual();
        }

        private void UpdateBorder()
        {
            ButtonBorder.BorderBrush = IsEnabled ? EnabledBrush : DisabledBrush;
        }
    }
}
