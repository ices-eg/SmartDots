using System.Windows.Input;
using SmartDots.Helpers;

namespace SmartDots.ViewModel
{
    public class MaturityStatusbarViewModel : MaturityBaseViewModel
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
                    MaturityViewModel.MaturityEditorViewModel.Height = (int)(MaturityViewModel.MaturityEditorViewModel.OriginalHeight * (double)zoomPercentage / 100);
                    MaturityViewModel.MaturityEditorViewModel.Width = (int)(MaturityViewModel.MaturityEditorViewModel.OriginalWidth * (double)zoomPercentage / 100);


                    //todo get visual center point to scroll to
                    MaturityViewModel.MaturityEditorView.ScrollViewer.ScrollToHorizontalOffset((MaturityViewModel.MaturityEditorViewModel.Width / 2) - (MaturityViewModel.MaturityEditorView.ActualWidth - 20) / 2);
                    MaturityViewModel.MaturityEditorView.ScrollViewer.ScrollToVerticalOffset((MaturityViewModel.MaturityEditorViewModel.Height / 2) - (MaturityViewModel.MaturityEditorView.ActualHeight - 20) / 2); //20 is the width of the scrollbar

                    RaisePropertyChanged("ZoomPercentage");
                    MaturityViewModel.MaturityEditorViewModel.RefreshShapes();
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
            if (MaturityViewModel.MaturityEditorView.ScrollViewer.ActualWidth /
                MaturityViewModel.MaturityEditorView.ScrollViewer.ActualHeight <
                (float)MaturityViewModel.MaturityEditorViewModel.OriginalWidth /
                (float)MaturityViewModel.MaturityEditorViewModel.OriginalHeight)
            {
                ZoomPercentage = MaturityViewModel.MaturityEditorView.ScrollViewer.ActualWidth / MaturityViewModel.MaturityEditorViewModel.OriginalWidth * 100;
            }
            else
            {
                ZoomPercentage = MaturityViewModel.MaturityEditorView.ScrollViewer.ActualHeight / MaturityViewModel.MaturityEditorViewModel.OriginalHeight * 100;
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
