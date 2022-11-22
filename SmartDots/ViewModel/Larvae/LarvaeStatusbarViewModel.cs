using System.Windows.Input;
using SmartDots.Helpers;

namespace SmartDots.ViewModel
{
    public class LarvaeStatusbarViewModel : LarvaeBaseViewModel
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
                    LarvaeViewModel.LarvaeEditorViewModel.Height = (int)(LarvaeViewModel.LarvaeEditorViewModel.OriginalHeight * (double)zoomPercentage / 100);
                    LarvaeViewModel.LarvaeEditorViewModel.Width = (int)(LarvaeViewModel.LarvaeEditorViewModel.OriginalWidth * (double)zoomPercentage / 100);


                    //todo get visual center point to scroll to
                    LarvaeViewModel.LarvaeEditorView.ScrollViewer.ScrollToHorizontalOffset((LarvaeViewModel.LarvaeEditorViewModel.Width / 2) - (LarvaeViewModel.LarvaeEditorView.ActualWidth - 20) / 2);
                    LarvaeViewModel.LarvaeEditorView.ScrollViewer.ScrollToVerticalOffset((LarvaeViewModel.LarvaeEditorViewModel.Height / 2) - (LarvaeViewModel.LarvaeEditorView.ActualHeight - 20) / 2); //20 is the width of the scrollbar

                    RaisePropertyChanged("ZoomPercentage");
                    LarvaeViewModel.LarvaeEditorViewModel.RefreshShapes();
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
            if (LarvaeViewModel.LarvaeEditorView.ScrollViewer.ActualWidth /
                LarvaeViewModel.LarvaeEditorView.ScrollViewer.ActualHeight <
                (float)LarvaeViewModel.LarvaeEditorViewModel.OriginalWidth /
                (float)LarvaeViewModel.LarvaeEditorViewModel.OriginalHeight)
            {
                ZoomPercentage = LarvaeViewModel.LarvaeEditorView.ScrollViewer.ActualWidth / LarvaeViewModel.LarvaeEditorViewModel.OriginalWidth * 100;
            }
            else
            {
                ZoomPercentage = LarvaeViewModel.LarvaeEditorView.ScrollViewer.ActualHeight / LarvaeViewModel.LarvaeEditorViewModel.OriginalHeight * 100;
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
