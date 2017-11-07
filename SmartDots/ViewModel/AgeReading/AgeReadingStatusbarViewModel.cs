using System.Windows.Input;
using SmartDots.Helpers;

namespace SmartDots.ViewModel
{
    public class AgeReadingStatusbarViewModel : AgeReadingBaseViewModel
    {
        private float zoomPercentage = 0;
        private string zoomPercentageString;
        private ICommand zoomCommand;
        private bool isFittingImage = true;
        private string info;


        public object ZoomPercentage
        {
            get { return zoomPercentage; }
            set
            {
                if (!float.IsPositiveInfinity(float.Parse(value.ToString())))
                {
                    zoomPercentage = float.Parse(value.ToString());
                    ZoomPercentageString = zoomPercentage + "%";
                    AgeReadingViewModel.AgeReadingEditorViewModel.Height = (int)(AgeReadingViewModel.AgeReadingEditorViewModel.OriginalHeight * (double)zoomPercentage / 100);
                    AgeReadingViewModel.AgeReadingEditorViewModel.Width = (int)(AgeReadingViewModel.AgeReadingEditorViewModel.OriginalWidth * (double)zoomPercentage / 100);


                    //todo get visual center point to scroll to
                    AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ScrollToHorizontalOffset((AgeReadingViewModel.AgeReadingEditorViewModel.Width / 2) - (AgeReadingViewModel.AgeReadingEditorView.ActualWidth - 20) / 2);
                    AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ScrollToVerticalOffset((AgeReadingViewModel.AgeReadingEditorViewModel.Height / 2) - (AgeReadingViewModel.AgeReadingEditorView.ActualHeight - 20) / 2); //20 is the width of the scrollbar

                    RaisePropertyChanged("ZoomPercentage");
                    AgeReadingViewModel.AgeReadingEditorViewModel.RefreshShapes(false);
                }
            }
        }

        public string ZoomPercentageString
        {
            get { return zoomPercentageString; }
            set
            {
                zoomPercentageString = value;
                RaisePropertyChanged("ZoomPercentageString");
            }
        }

        public float ZoomFactor
        {
            get { return zoomPercentage / 100; }
        }

        public ICommand ZoomCommand
        {
            get { return zoomCommand ?? (zoomCommand = new Command(p => true, p => this.Zoom(int.Parse((string)p)))); }
        }

        public bool IsFittingImage
        {
            get { return isFittingImage; }
            set
            {
                isFittingImage = value;
                if (isFittingImage)
                {
                    FitImage();
                }
                RaisePropertyChanged("IsFittingImage");
            }
        }

        public string Info
        {
            get { return info; }
            set
            {
                info = value;
                RaisePropertyChanged("Info");
            }
        }


        private void Zoom(int zoomFactor)
        {
            IsFittingImage = false;
            ZoomPercentage = zoomFactor;
        }

        public void FitImage()
        {
            if (AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ActualWidth /
                AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ActualHeight <
                (float)AgeReadingViewModel.AgeReadingEditorViewModel.OriginalWidth /
                (float)AgeReadingViewModel.AgeReadingEditorViewModel.OriginalHeight)
            {
                ZoomPercentage = AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ActualWidth / AgeReadingViewModel.AgeReadingEditorViewModel.OriginalWidth * 100;
            }
            else
            {
                ZoomPercentage = AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.ActualHeight / AgeReadingViewModel.AgeReadingEditorViewModel.OriginalHeight * 100;
            }

        }

        //todo this does not seem to work
        public void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsFittingImage = false;
        }
        //todo same as above
        public void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsFittingImage = false;
        }
    }
}
