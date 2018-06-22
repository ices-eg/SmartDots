using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge.Imaging;
using AForge.Imaging.Filters;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Helpers;
using SmartDots.Model;
using Tesseract;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Line = System.Windows.Shapes.Line;
using Point = System.Windows.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SmartDots.ViewModel
{
    public class AgeReadingEditorViewModel : AgeReadingBaseViewModel
    {
        private object lineColor;
        private object dotColor;
        private int lineWidth;
        private int dotWidth;
        private List<Line> originalLineShapes = new List<Line>();
        private List<Dot> originalDots = new List<Dot>();
        private ObservableCollection<Line> lineShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> scaleShapes = new ObservableCollection<Line>();
        private ObservableCollection<Shape> dotShapes = new ObservableCollection<Shape>();
        private CombinedLine activeCombinedLine;
        private CombinedLine tempCombinedLine;
        private EditorModeEnum mode;
        private Image tracker;
        private BitmapImage originalImage;
        private bool shapeChangeFlag;
        private BitmapImage otolithImage;
        private int originalWidth;
        private int originalHeight;
        private int width;
        private int height;
        private double brightness;
        private double contrast;
        private decimal pixelLength;
        private bool canUndo;
        private bool canRedo;
        private bool isScaleDrawn;
        private UndoRedo undoRedo;
        private string dotType = "Seawater";
        private string dotShape = "Dot";

        public object LineColor
        {
            get { return lineColor; }
            set
            {
                lineColor = value;
                RaisePropertyChanged("LineColor");
            }
        }

        public object DotColor
        {
            get { return dotColor; }
            set
            {
                dotColor = value;
                RaisePropertyChanged("DotColor");
            }
        }

        public object LineWidth
        {
            get { return lineWidth; }
            set
            {
                lineWidth = int.Parse(value.ToString());
                RaisePropertyChanged("LineWidth");
            }
        }

        public object DotWidth
        {
            get { return dotWidth; }
            set
            {
                dotWidth = int.Parse(value.ToString());
                RaisePropertyChanged("DotWidth");
            }
        }

        public string DotType
        {
            get { return dotType; }
            set
            {
                dotType = value;
                RaisePropertyChanged("DotType");
            }
        }

        public string DotShape
        {
            get { return dotShape; }
            set
            {
                dotShape = value;
                RaisePropertyChanged("DotShape");
            }
        }

        public ObservableCollection<Line> LineShapes
        {
            get { return lineShapes; }
            set
            {
                lineShapes = value;
                RaisePropertyChanged("LineShapes");
            }
        }

        public ObservableCollection<Line> ScaleShapes
        {
            get { return scaleShapes; }
            set
            {
                scaleShapes = value;
                RaisePropertyChanged("ScaleShapes");
            }
        }

        public List<Line> OriginalLineShapes
        {
            get { return originalLineShapes; }
            set
            {
                originalLineShapes = value;
                RaisePropertyChanged("LineShapes");
            }
        }

        public CombinedLine ActiveCombinedLine
        {
            get { return activeCombinedLine; }
            set
            {
                activeCombinedLine = value;

                activeCombinedLine?.CalculateDotIndices();
                AgeReadingViewModel.AgeReadingView.BrightnessGraph.graphViewer.SetCombinedLine(value);
                AgeReadingViewModel.AgeReadingView.RednessGraph.graphViewer.SetCombinedLine(value);
                AgeReadingViewModel.AgeReadingView.GrowthGraph.graphViewer.SetCombinedLine(value);
                RaisePropertyChanged("ActiveCombinedLine");
            }
        }

        public EditorModeEnum Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                if (ageReadingViewModel != null)
                {
                    AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = false;
                    AgeReadingViewModel.AgeReadingEditorView.DotButton.IsPressed = false;
                    AgeReadingViewModel.AgeReadingEditorView.DeleteBtn.IsChecked = false;

                    switch (value)
                    {
                        case EditorModeEnum.DrawLine:
                            AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = true;
                            break;
                        case EditorModeEnum.MakingLine:
                            AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = true;
                            break;
                        case EditorModeEnum.DrawDot:
                            AgeReadingViewModel.AgeReadingEditorView.DotButton.IsPressed = true;
                            break;
                        case EditorModeEnum.DragImage:
                            //AgeReadingViewModel.AgeReadingEditorView.MoveBtn.IsChecked = true;
                            break;
                        case EditorModeEnum.SelectLine:
                            //AgeReadingViewModel.AgeReadingEditorView.SelectLineBtn.IsChecked = true;
                            break;
                        case EditorModeEnum.Delete:
                            AgeReadingViewModel.AgeReadingEditorView.DeleteBtn.IsChecked = true;
                            break;
                        case EditorModeEnum.None:
                            AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = false;
                            AgeReadingViewModel.AgeReadingEditorView.DotButton.IsPressed = false;
                            AgeReadingViewModel.AgeReadingEditorView.DeleteBtn.IsChecked = false;
                            break;
                    }
                }
                RaisePropertyChanged("Mode");
            }
        }

        public Image Tracker
        {
            get { return tracker; }
            set
            {
                tracker = value;
                RaisePropertyChanged("Tracker");
            }
        }

        public BitmapImage OtolithImage
        {
            get { return otolithImage; }
            set
            {
                otolithImage = value;
                OriginalWidth = otolithImage.PixelWidth;
                OriginalHeight = otolithImage.PixelHeight;
                AgeReadingViewModel.AgeReadingStatusbarView.lblImgSize.Content = $"{OriginalWidth} x {OriginalHeight}";

                RaisePropertyChanged("OtolithImage");
                RaisePropertyChanged("CanMeasure");
            }
        }

        public int OriginalWidth
        {
            get { return originalWidth; }
            set
            {
                originalWidth = value;
                RaisePropertyChanged("OriginalWidth");
            }
        }

        public int OriginalHeight
        {
            get { return originalHeight; }
            set
            {
                originalHeight = value;
                RaisePropertyChanged("OriginalHeight");
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.HorizontalScrollBarVisibility = width >
                                                                                                      AgeReadingViewModel
                                                                                                          .AgeReadingEditorView
                                                                                                          .ScrollViewer
                                                                                                          .Width
                    ? ScrollBarVisibility.Hidden
                    : ScrollBarVisibility.Auto;
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                AgeReadingViewModel.AgeReadingEditorView.ScrollViewer.VerticalScrollBarVisibility = height >
                                                                                                    AgeReadingViewModel
                                                                                                        .AgeReadingEditorView
                                                                                                        .ScrollViewer
                                                                                                        .Height
                    ? ScrollBarVisibility.Hidden
                    : ScrollBarVisibility.Auto;
                RaisePropertyChanged("Height");
            }
        }

        public CombinedLine TempCombinedLine
        {
            get { return tempCombinedLine; }
            set { tempCombinedLine = value; }
        }

        public List<Dot> OriginalDots
        {
            get { return originalDots; }
            set { originalDots = value; }
        }

        public ObservableCollection<Shape> DotShapes
        {
            get { return dotShapes; }
            set
            {
                dotShapes = value;
                RaisePropertyChanged("DotShapes");
            }
        }

        public List<decimal> LineBrightness
        {
            get
            {
                List<decimal> values = new List<decimal>();
                if (ActiveCombinedLine != null)
                {
                    values = ActiveCombinedLine.GetLineBrightness(BitmapConverter.BitmapImage2Bitmap(OtolithImage));
                }
                return values;
            }
        }

        public List<decimal> LineRedness
        {
            get
            {
                List<decimal> values = new List<decimal>();
                if (ActiveCombinedLine != null)
                {
                    values = ActiveCombinedLine.GetLineRedness(BitmapConverter.BitmapImage2Bitmap(OtolithImage));
                }
                return values;
            }
        }

        public List<decimal> LineGrowth
        {
            get
            {
                List<decimal> values = new List<decimal>();
                if (ActiveCombinedLine != null)
                {
                    values = ActiveCombinedLine.CalculateLineGrowth();
                }
                return values;
            }
        }

        public double Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                if (AgeReadingViewModel != null && !AgeReadingViewModel.AgeReadingFileViewModel.ChangingFile)
                {
                    AdjustImage();
                    RaisePropertyChanged("Brightness");
                    RaisePropertyChanged("OtolithImage");
                }
            }
        }

        public double Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                if (AgeReadingViewModel != null && !AgeReadingViewModel.AgeReadingFileViewModel.ChangingFile)
                {
                    AdjustImage();
                    RaisePropertyChanged("Contrast");
                    RaisePropertyChanged("OtolithImage");
                }
            }
        }

        public BitmapImage OriginalImage
        {
            get { return originalImage; }
            set
            {
                originalImage = value;
                originalImage.Freeze();
                if (WebAPI.Settings.AutoMeasureScale && (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale == null || AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale == 0.0m))
                {
                    AutoMeasureScale();
                }
            }
        }

        public decimal PixelLength
        {
            get { return pixelLength; }
            set { pixelLength = value; }
        }

        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                canUndo = value;
                RaisePropertyChanged("CanUndo");
            }
        }

        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                canRedo = value;
                RaisePropertyChanged("CanRedo");
            }
        }

        public bool CanMeasure
        {
            get { return OtolithImage != null; }
        }

        public UndoRedo UndoRedo
        {
            get { return undoRedo; }
            set { undoRedo = value; }
        }

        public bool ShapeChangeFlag
        {
            get { return shapeChangeFlag; }
            set { shapeChangeFlag = value; }
        }

        public bool CanDrawLine
        {
            get
            {
                return AgeReadingViewModel?.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count < 2
                       && ActiveCombinedLine == null
                       && !AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any(x => x.IsFixed)
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        WebAPI.Settings.AnnotateWithoutSample);
            }
        }

        public bool CanDrawDot
        {
            get
            {
                return AgeReadingViewModel?.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count == 1
                       && ActiveCombinedLine != null
                       && Mode != EditorModeEnum.MakingLine
                       && !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        WebAPI.Settings.AnnotateWithoutSample);
            }
        }

        public bool CanDelete
        {
            get
            {
                return AgeReadingViewModel?.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count == 1
                       && ActiveCombinedLine != null
                       && Mode != EditorModeEnum.MakingLine
                       && !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        WebAPI.Settings.AnnotateWithoutSample);
            }
        }

        public bool IsScaleDrawn
        {
            get { return isScaleDrawn; }
            set
            {
                isScaleDrawn = value;
                if (isScaleDrawn)
                {
                    AgeReadingViewModel.AgeReadingEditorView.ScaleDrawn.Visibility = Visibility.Visible;
                    AgeReadingViewModel.AgeReadingEditorView.ScaleNotDrawn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AgeReadingViewModel.AgeReadingEditorView.ScaleDrawn.Visibility = Visibility.Collapsed;
                    AgeReadingViewModel.AgeReadingEditorView.ScaleNotDrawn.Visibility = Visibility.Visible;
                }
                RaisePropertyChanged("IsScaleDrawn");
            }
        }

        public System.Drawing.Rectangle ScaleRectangle { get; private set; }
        public bool IsMeasuring { get; private set; }
        public List<Guid> MeasuredFileIDs { get; set; } = new List<Guid>();

        public AgeReadingEditorViewModel()
        {
            Mode = EditorModeEnum.DrawLine;
            Brightness = 0;
            Contrast = 0;
            LineColor = (Color) ColorConverter.ConvertFromString("#FFFF00FF");
            LineWidth = 2;
            DotColor = (Color) ColorConverter.ConvertFromString("#FF00FF00");
            DotWidth = 10;
            UndoRedo = new UndoRedo(this);
            LoadUserPreferences();
        }

        public void CalculateAge()
        {
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation == null) return;
            int age = 0;
            foreach (
                CombinedLine combinedLine in
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines)
            {
                if (combinedLine.Dots.Count > age)
                {
                    age = combinedLine.Dots.Count;
                }
            }
            AgeReadingViewModel.AgeReadingAnnotationViewModel.SetAge(age);
        }

        public async void GetPixelLength()
        {
            try
            {
                var oldvalue = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale;
                AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = false;

                
                PixelLength = await Task.Run( () => CalculatePixelLength(OriginalImage, 128));
                if(PixelLength == 0) PixelLength = await Task.Run(() => CalculatePixelLength(OriginalImage, 20));
                if (PixelLength == 0) PixelLength = await Task.Run( () => CalculatePixelLengthOld(OriginalImage));

                    string scaleText = await System.Threading.Tasks.Task.Run(() => FindScaleText());
                scaleText = Regex.Replace(scaleText, " ", "");
                if (scaleText.StartsWith(".")) scaleText = scaleText.Substring(1, scaleText.Length - 1);
                if (scaleText.StartsWith(",")) scaleText = scaleText.Substring(1, scaleText.Length - 1);


                double factor = 1;
                int resultIndex = 0;
                if (scaleText.Contains("mm"))
                {
                    resultIndex = scaleText.IndexOf("mm");
                }
                else if (scaleText.Contains("cm"))
                {
                    factor = 10;
                    resultIndex = scaleText.IndexOf("cm");
                }
                else if (scaleText.Contains("µm"))
                {
                    factor = 0.001;
                    resultIndex = scaleText.IndexOf("µm");
                }
                else if (scaleText.Contains("um"))
                {
                    factor = 0.001;
                    resultIndex = scaleText.IndexOf("um");
                }
                else if (scaleText.Contains("m"))
                {
                    factor = 1;
                    resultIndex = scaleText.IndexOf("m");
                }

                scaleText = scaleText.Substring(0, resultIndex);
                decimal number = 0;

                scaleText = scaleText.Replace(',', '.');
                scaleText = new String(scaleText.Where(Char.IsDigit).ToArray());
                number = decimal.Parse(scaleText, CultureInfo.InvariantCulture);
                PixelLength = (decimal) (PixelLength/number/(decimal) factor);
                var newvalue = PixelLength;
                if (oldvalue != newvalue)
                {
                    AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale = PixelLength;
                    AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                    AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
                    DtoFile dtofile =
                        (DtoFile)
                        Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
                    var updateFileResult = WebAPI.UpdateFile(dtofile);
                    if (!updateFileResult.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox("Error saving File to Web API\n" + updateFileResult.ErrorMessage, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = false;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox(
                    "Could not determine the scale automatically. Please measure the scale manually", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information, e);

                PixelLength = 0;
            }



            AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
        }

        public PixelColor[,] GetPixels(BitmapSource source)
        {
            
            //Application.Current.Dispatcher.Invoke((Action)delegate {

                if (source.Format != PixelFormats.Bgra32)
                    source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

                int width = source.PixelWidth;
                int height = source.PixelHeight;
                PixelColor[,] result = new PixelColor[width, height];

                BitmapSourceHelper.CopyPixels(source, result, width * 4, 0);
                

            //});
            return result;
        }

        public string FindScaleText()
        {
            var ocrtext = string.Empty;
            try
            {
                using (var engine = new TesseractEngine("tessdata", "eng", EngineMode.TesseractOnly))
                {

                    engine.SetVariable("textord_min_xheight", 15);
                    engine.SetVariable("textord_max_noise_size", 10);
                    engine.SetVariable("tessedit_char_whitelist", "0123456789uµmc.,");
                    //engine.SetVariable("classify_font_name", "Arial");
                    //engine.SetVariable("x_ht_acceptance_tolerance", 3);

                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {

                        //Invert filter = new Invert();
                        Bitmap bitmapimage =
                            AForge.Imaging.Image.Clone(BitmapConverter.PreProcessForScaleDetection(OriginalImage, 128),
                                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        //filter.ApplyInPlace(bitmapimage);
                        using (var img = PixConverter.ToPix(bitmapimage))
                        {
                            using (
                                var page = engine.Process(img,
                                    new Tesseract.Rect(ScaleRectangle.X, ScaleRectangle.Y, ScaleRectangle.Width,
                                        ScaleRectangle.Height), PageSegMode.SingleLine))
                            {
                                ocrtext = page.GetText();
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                //
            }

            return ocrtext;
        }

        public decimal CalculatePixelLengthOld(BitmapImage image)
        {
            var pixels = GetPixels(image);
            decimal milimeterPixels = 0;
            int x = 0, y = 0;
            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    if (pixels[j, i].Red >= 250 && pixels[j, i].Blue >= 250 && pixels[j, i].Green >= 250)
                    {
                        try
                        {
                            for (int k = 1; k < image.PixelHeight - j; k++)
                            {
                                if (j + k < image.PixelWidth &&
                                    (pixels[j + k, i].Red >= 250 || pixels[j + k, i].Blue >= 250 ||
                                     pixels[j + k, i].Green >= 250))
                                {
                                    if (k > milimeterPixels)
                                    {
                                        milimeterPixels = k;
                                        x = j;
                                        y = i;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
            }
            try
            {
                if (milimeterPixels == 0) return 0;
                var rect = new System.Drawing.Rectangle();
                rect.X = x;
                if (y < 70) rect.Y = 0;
                else
                {
                    rect.Y = y - 70;
                }
                rect.Width = (int) milimeterPixels;
                rect.Height = 68;
                ScaleRectangle = rect;
                return milimeterPixels;

            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public decimal CalculatePixelLength(BitmapImage image, int threshold)
        {
            decimal milimeterPixels = 0;
            var newImage = BitmapConverter.PreProcessForScaleDetection(image,threshold);
            
            
            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter();
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = 40;
            bc.MinHeight = 0;
            bc.MaxHeight = 70;
            bc.MaxWidth = newImage.Width / 3;
            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;
            // process binary image
            bc.ProcessImage(newImage);
            //newImage.Save("step5.jpg");
            List<Blob> blobs = bc.GetObjectsInformation().ToList();
            blobs = blobs.Where(x => (x.Rectangle.Width / x.Rectangle.Height) > 4).OrderByDescending(x => x.Rectangle.Width / x.Rectangle.Height).ToList();
            // extract the biggest blob
            if (blobs.Count > 0)
            {
                milimeterPixels = blobs[0].Rectangle.Width;
                try
                {
                    if (milimeterPixels == 0) return 0;
                    var rect = new System.Drawing.Rectangle();
                    rect.X = blobs[0].Rectangle.X;
                    if (blobs[0].Rectangle.Y < 70) rect.Y = 0;
                    else
                    {
                        rect.Y = blobs[0].Rectangle.Y - 70;
                    }
                    if (blobs[0].Rectangle.Y + 70 > newImage.Height)
                    {

                        rect.Height = 70 + newImage.Height - blobs[0].Rectangle.Y;
                    }
                    else
                    {
                        rect.Height = 140;
                    }
                    rect.Width = (int)milimeterPixels;
                    if (rect.Y + rect.Height > newImage.Height) return 0;
                    if (rect.X + rect.Width > newImage.Width) return 0;
                    ScaleRectangle = rect;
                    return milimeterPixels;
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public struct PixelColor
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        public void SaveUserPreferences()
        {
            try
            {
                Properties.Settings.Default.DotColor = DotColor.ToString();
                Properties.Settings.Default.DotWidth = (int) DotWidth;
                Properties.Settings.Default.DotShape = DotShape;
                Properties.Settings.Default.DotType = DotType;
                Properties.Settings.Default.LineColor = LineColor.ToString();
                Properties.Settings.Default.LineWidth = (int) LineWidth;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void UpdateButtons()
        {
            RaisePropertyChanged("CanDrawLine");
            RaisePropertyChanged("CanDrawDot");
            RaisePropertyChanged("CanDelete");
            AgeReadingViewModel.AgeReadingEditorView.LineButton.UpdateButtonBackground(
                AgeReadingViewModel.AgeReadingEditorView.LineButton);
            AgeReadingViewModel.AgeReadingEditorView.DotButton.UpdateButtonBackground(
                AgeReadingViewModel.AgeReadingEditorView.DotButton);
        }

        public void LoadUserPreferences()
        {
            try
            {
                DotColor = (Color) ColorConverter.ConvertFromString(Properties.Settings.Default.DotColor);
                DotWidth = Properties.Settings.Default.DotWidth;
                DotShape = Properties.Settings.Default.DotShape;
                DotType = Properties.Settings.Default.DotType;
                LineColor = (Color) ColorConverter.ConvertFromString(Properties.Settings.Default.LineColor);
                LineWidth = Properties.Settings.Default.LineWidth;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void RestoreUserPreferences()
        {
            try
            {
                DotColor = (Color) ColorConverter.ConvertFromString("#FF00FF00");
                DotWidth = 8;
                DotShape = "Dot";
                DotType = "Seawater";
                LineColor = (Color) ColorConverter.ConvertFromString("#FFFF00FF");
                LineWidth = 2;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error restoring user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void RefreshShapes(bool updategraphs = true)
        {
            LineShapes = new ObservableCollection<Line>();
            DotShapes = new ObservableCollection<Shape>();
            float zoomfactor = AgeReadingViewModel == null
                ? 0
                : AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
            ObservableCollection<Line> lines = new ObservableCollection<Line>();
            ObservableCollection<Shape> dots = new ObservableCollection<Shape>();

            foreach (var annotation in AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations)
            {
                foreach (CombinedLine cl in annotation.CombinedLines)
                {
                    foreach (Model.Line l in cl.Lines)
                    {
                        Line line = new Line()
                        {
                            X1 = (float) l.X1*zoomfactor,
                            Y1 = (float) l.Y1*zoomfactor,
                            X2 = (float) l.X2*zoomfactor,
                            Y2 = (float) l.Y2*zoomfactor,
                            Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom(l.Color)),
                            StrokeThickness = (float) l.Width*zoomfactor,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round

                        };
                        lines.Add(line);
                    }

                    foreach (Dot d in cl.Dots)
                    {
                        if (d.DotShape == "Cross")
                        {
                            Shape c = new Cross()
                            {
                                Fill = (SolidColorBrush) (new BrushConverter().ConvertFrom(d.Color)),
                                Width = (float) d.Width*zoomfactor,
                                X = (float) d.X*zoomfactor,
                                Y = (float) d.Y*zoomfactor,
                            };
                            var line1 = ((Cross) c).Line1;
                            line1.StrokeStartLineCap = PenLineCap.Round;
                            line1.StrokeEndLineCap = PenLineCap.Round;

                            var line2 = ((Cross) c).Line2;
                            line2.StrokeStartLineCap = PenLineCap.Round;
                            line2.StrokeEndLineCap = PenLineCap.Round;

                            lines.Add(line1);
                            lines.Add(line2);
                        }
                        else
                        {
                            Shape l = new Ellipse()
                            {
                                Fill = (SolidColorBrush) (new BrushConverter().ConvertFrom(d.Color)),
                                Width = (float) d.Width*zoomfactor,
                                Height = (float) d.Width*zoomfactor,
                                SnapsToDevicePixels = false

                            };
                            var left = (float) d.X*zoomfactor - (float) d.Width/2*zoomfactor;
                            var top = (float) d.Y*zoomfactor - (float) d.Width/2*zoomfactor;
                            Canvas.SetLeft(l, left);
                            Canvas.SetTop(l, top);
                            dots.Add(l);
                        }
                        if (d.DotType == "Freshwater")
                        {
                            var radius = (float) d.Width*1.8;
                            Shape l = new Ellipse()
                            {
                                Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom(d.Color)),
                                Width = ((radius)*zoomfactor),
                                Height = ((radius)*zoomfactor),
                                StrokeThickness = (float) d.Width/8*zoomfactor,
                                StrokeDashArray = new DoubleCollection() {3, 3}

                            };
                            var left = (float) d.X*zoomfactor - (radius)/2*zoomfactor;
                            var top = (float) d.Y*zoomfactor - (radius)/2*zoomfactor;
                            Canvas.SetLeft(l, left);
                            Canvas.SetTop(l, top);
                            dots.Add(l);
                        }
                    }
                    if (Mode != EditorModeEnum.MakingLine && cl.Lines.Any())
                    {
                        var lastLine = cl.Lines.OrderBy(x => x.LineIndex).Last();
                        var n = Math.Atan2(lastLine.Y1 - lastLine.Y2, lastLine.X2 - lastLine.X1)*180/Math.PI;
                        Point point1 = new Point(lastLine.X2, lastLine.Y2);
                        var cos = Math.Cos((n + 30)*(Math.PI/180.0))*10;
                        var sin = Math.Sin((n + 30)*(Math.PI/180.0))*10;
                        Point point2 = new Point(lastLine.X2 - cos, lastLine.Y2 + sin);
                        cos = Math.Cos((n - 30)*(Math.PI/180.0))*10;
                        sin = Math.Sin((n - 30)*(Math.PI/180.0))*10;
                        Point point3 = new Point(lastLine.X2 - cos, lastLine.Y2 + sin);
                        var line1 = new Line()
                        {
                            X1 = (float) point1.X*zoomfactor,
                            Y1 = (float) point1.Y*zoomfactor,
                            X2 = (float) point2.X*zoomfactor,
                            Y2 = (float) point2.Y*zoomfactor,
                            Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom(lastLine.Color)),
                            StrokeThickness = (lastLine.Width*zoomfactor),
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        var line2 = new Line()
                        {
                            X1 = (float) point1.X*zoomfactor,
                            Y1 = (float) point1.Y*zoomfactor,
                            X2 = (float) point3.X*zoomfactor,
                            Y2 = (float) point3.Y*zoomfactor,
                            Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom(lastLine.Color)),
                            StrokeThickness = (lastLine.Width*zoomfactor),
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        lines.Add(line1);
                        lines.Add(line2);
                    }

                }
            }

            LineShapes = lines;
            DotShapes = dots;
            ScaleShapes = ScaleShapes;

            if (updategraphs) AgeReadingViewModel.UpdateGraphs();
            CalculateAge();
        }

        public Tuple<LinePoint, double> GetClosestLinePointWithDistance(Point p)
        {
            float zoomfactor = AgeReadingViewModel == null
                ? 0
                : AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;

            Tuple<LinePoint, double> closestPoint = null;
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation?.CombinedLines[0].Points.Count > 0)
            {
                closestPoint =
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0]
                        .GetClosestPointWithDistance(new Point(p.X/zoomfactor, p.Y/zoomfactor));
            }

            Tuple<LinePoint, double> point = null;
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Count > 1)
            {
                foreach (
                    CombinedLine l in AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines)
                {
                    if (l.Points.Count > 0)
                        point = l.GetClosestPointWithDistance(new Point(p.X/zoomfactor, p.Y/zoomfactor));

                    if (closestPoint != null && (point != null && point.Item2 < closestPoint.Item2))
                    {
                        closestPoint = point;
                    }
                }
            }
            return closestPoint;
        }

        public void AddLine(Model.Line l)
        {
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last().AddLine(l);
            ActiveCombinedLine =
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last();

            RefreshShapes();
            AgeReadingViewModel.UpdateGraphs();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            ShapeChangeFlag = true;
        }

        public void RemoveLine(Model.Line l)
        {
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last().RemoveLine(l);
            ActiveCombinedLine =
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last();
            RefreshShapes();
            AgeReadingViewModel.UpdateGraphs();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            ShapeChangeFlag = true;
        }

        public void AddDot(CombinedLine parentCombinedLine, Dot d)
        {
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.First(
                cl => cl.Equals(parentCombinedLine)).AddDot(d);
            RefreshShapes();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            ShapeChangeFlag = true;
        }

        public void RemoveDot(CombinedLine parentCombinedLine, Dot d)
        {
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.First(
                cl => cl.Equals(parentCombinedLine)).RemoveDot(d);
            RefreshShapes();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            ShapeChangeFlag = true;
        }

        public void AdjustImage()
        {
            try
            {
                if (OriginalImage != null)
                {
                    OtolithImage = SetBrightness(SetContrast(OriginalImage));
                    AgeReadingViewModel.UpdateGraphs();
                }
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error adjusting image", "Error", MessageBoxButton.OK, MessageBoxImage.Error,
                    e);
            }
        }

        public BitmapImage SetBrightness(BitmapImage img)
        {
            unsafe
            {
                double brightness = Brightness;
                Bitmap bmap = BitmapConverter.BitmapImage2Bitmap(img);
                BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
                    ImageLockMode.ReadWrite, bmap.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* PtrFirstPixel = (byte*) bitmapData.Scan0;

                Parallel.For((long) 0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y*bitmapData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        double oldBlue = currentLine[x] + brightness;
                        if (oldBlue < 0) oldBlue = 1;
                        if (oldBlue > 255) oldBlue = 255;

                        double oldGreen = currentLine[x + 1] + brightness;
                        if (oldGreen < 0) oldGreen = 1;
                        if (oldGreen > 255) oldGreen = 255;

                        double oldRed = currentLine[x + 2] + brightness;
                        if (oldRed < 0) oldRed = 1;
                        if (oldRed > 255) oldRed = 255;

                        currentLine[x] = (byte) oldBlue;
                        currentLine[x + 1] = (byte) oldGreen;
                        currentLine[x + 2] = (byte) oldRed;
                    }
                });
                bmap.UnlockBits(bitmapData);
                return BitmapConverter.Bitmap2BitmapImage(bmap);
            }
        }

        public BitmapImage SetContrast(BitmapImage img)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing.
            stopwatch.Start();

            unsafe
            {
                double contrast = Contrast;
                Bitmap bmap = BitmapConverter.BitmapImage2Bitmap(img);
                contrast = (100.0 + contrast)/100.0;
                contrast *= contrast;
                BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
                    ImageLockMode.ReadWrite, bmap.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* PtrFirstPixel = (byte*) bitmapData.Scan0;

                Parallel.For((long) 0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y*bitmapData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        double oldBlue = currentLine[x]/255.0;
                        oldBlue -= 0.5;
                        oldBlue *= contrast;
                        oldBlue += 0.5;
                        oldBlue *= 255;
                        if (oldBlue < 0) oldBlue = 0;
                        if (oldBlue > 255) oldBlue = 255;

                        double oldGreen = currentLine[x + 1]/255.0;
                        oldGreen -= 0.5;
                        oldGreen *= contrast;
                        oldGreen += 0.5;
                        oldGreen *= 255;
                        if (oldGreen < 0) oldGreen = 0;
                        if (oldGreen > 255) oldGreen = 255;

                        double oldRed = currentLine[x + 2]/255.0;
                        oldRed -= 0.5;
                        oldRed *= contrast;
                        oldRed += 0.5;
                        oldRed *= 255;
                        if (oldRed < 0) oldRed = 0;
                        if (oldRed > 255) oldRed = 255;

                        currentLine[x] = (byte) oldBlue;
                        currentLine[x + 1] = (byte) oldGreen;
                        currentLine[x + 2] = (byte) oldRed;
                    }
                });
                bmap.UnlockBits(bitmapData);

                // Stop timing.
                stopwatch.Stop();

                // Write result.
                Console.WriteLine("Time elapsed for SetContrast: " + stopwatch.Elapsed);
                return BitmapConverter.Bitmap2BitmapImage(bmap);
            }
        }

        
        public void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AgeReadingViewModel.AgeReadingStatusbarViewModel.IsFittingImage)
            {
                AgeReadingViewModel.AgeReadingStatusbarViewModel.FitImage();
            }
            else
            {
                RefreshShapes();
            }
        }

        public void ParentCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //here comes the code for drawing, dotting, deleting

            if (e.ChangedButton == MouseButton.Left)
            {
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly &&
                    Mode != EditorModeEnum.DrawScale)
                {
                    new WinUIMessageBoxService().Show("The selected file is ReadOnly!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsReadOnly)
                {
                    new WinUIMessageBoxService().Show("The selected annotation is ReadOnly!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations.Count > 1)
                {
                    new WinUIMessageBoxService().Show(
                        "Can not draw any shapes when multiple annotations are selected!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                switch (Mode)
                {
                    case EditorModeEnum.DrawLine:
                        DrawLine(e);
                        break;
                    case EditorModeEnum.MakingLine:
                        MakeLine(e);
                        break;
                    case EditorModeEnum.DrawDot:
                        DrawDot(e);
                        break;
                    case EditorModeEnum.Delete:
                        DeleteLineOrDot(e);
                        break;
                    case EditorModeEnum.DrawScale:
                        DrawScale(e);
                        break;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                switch (Mode)
                {
                    case EditorModeEnum.MakingLine:
                        EndLine(e);
                        break;
                }
            }
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
            UpdateButtons();
        }

        private void DrawScale(MouseButtonEventArgs e)
        {
            if (!ScaleShapes.Any())
            {
                float zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
                var position = e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas);
                Line line = new Line()
                {
                    X1 = position.X,
                    Y1 = position.Y,
                    X2 = position.X,
                    Y2 = position.Y,
                    Stroke = (SolidColorBrush) (new BrushConverter().ConvertFrom(LineColor.ToString())),
                    StrokeThickness = ((int) LineWidth*zoomfactor),
                };
                ScaleShapes.Add(line);
            }
            else
            {
                IsScaleDrawn = true;
                Mode = EditorModeEnum.None;
            }
            RefreshShapes();
        }

        private void DrawLine(MouseButtonEventArgs e)
        {
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation == null)
            {
                if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.CanCreate) return;
                AgeReadingViewModel.AgeReadingAnnotationViewModel.NewAnnotation();
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation == null) return;
            }
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any()) return;

            AgeReadingViewModel.EnableUI(false);
            CombinedLine temp = new CombinedLine();
            float zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
            Model.Line l = new Model.Line()
            {
                ID = Guid.NewGuid(),
                Color = LineColor.ToString(),
                X1 = (int) ((int) e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X/zoomfactor),
                X2 = (int) ((int) e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X/zoomfactor),
                Y1 = (int) ((int) e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y/zoomfactor),
                Y2 = (int) ((int) e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y/zoomfactor),
                Width = (int) LineWidth,
                ParentCombinedLine = temp,
                AnnotationID = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ID,
                LineIndex = 0
            };
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Add(temp);
            Mode = EditorModeEnum.MakingLine;
            AddLine(l);
        }

        private void MakeLine(MouseButtonEventArgs e)
        {
            try
            {
                if (Mode == EditorModeEnum.MakingLine)
                {
                    float zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;

                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                            .Lines.Last()
                            .X2
                        = (int) (e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X/zoomfactor);
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                            .Lines.Last()
                            .Y2
                        = (int) (e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y/zoomfactor);

                    Model.Line l =
                        AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                            .Lines.Last();

                    Model.Line line = new Model.Line()
                    {
                        ID = Guid.NewGuid(),
                        Color = LineColor.ToString(),
                        X1 = l.X2,
                        X2 = l.X2,
                        Y1 = l.Y2,
                        Y2 = l.Y2,
                        Width = (int) LineWidth,
                        ParentCombinedLine =
                            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last(),
                        AnnotationID = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ID,
                        LineIndex = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation
                                        .CombinedLines.Last().Lines.Last().LineIndex + 1
                    };
                    UndoRedo.InsertInUnDoRedoForAddLine(l,
                        AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last(), this);

                    AddLine(line);
                }
            }
            catch (Exception)
            {
                //
            }

        }

        private void EndLine(MouseButtonEventArgs e)
        {
            try
            {
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                {
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                        .Lines.RemoveAt(
                            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                                .Lines.Count -
                            1);
                    if (
                        !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                            .Lines.Any())
                    {
                        AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(
                            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last());
                        ActiveCombinedLine = null;
                        Mode = EditorModeEnum.DrawLine;
                    }
                    else
                    {
                        Mode = EditorModeEnum.DrawDot;
                    }

                    RefreshShapes();
                    AgeReadingViewModel.UpdateGraphs();
                    UpdateButtons();
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();

                    AgeReadingViewModel.EnableUI(true);
                }
            }
            catch (Exception)
            {
                Mode = EditorModeEnum.DrawLine;
                AgeReadingViewModel.EnableUI(true);

            }
            AgeReadingViewModel.EnableUI(true);
            UpdateButtons();


        }

        private void DrawDot(MouseButtonEventArgs e)
        {
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation == null) return;
            if (!CanDrawDot) return;
            if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any()) return;
            Tuple<LinePoint, double> closestPoint =
                GetClosestLinePointWithDistance(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
            if (closestPoint != null)
            {
                if (closestPoint.Item2 <= 50)
                {
                    if (
                        !closestPoint.Item1.ParentCombinedLine.ContainsDotAt(new Point(closestPoint.Item1.X,
                            closestPoint.Item1.Y)))
                    {
                        Dot d = new Dot()
                        {
                            ID = Guid.NewGuid(),
                            X = closestPoint.Item1.X,
                            Y = closestPoint.Item1.Y,
                            Width = (int) DotWidth,
                            Color = DotColor.ToString(),
                            ParentCombinedLine = closestPoint.Item1.ParentCombinedLine,
                            AnnotationID = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ID,
                            DotShape = DotShape,
                            DotType = DotType
                        };
                        AddDot(closestPoint.Item1.ParentCombinedLine, d);
                        UndoRedo.InsertInUnDoRedoForAddDot(d, d.ParentCombinedLine, this);
                        //ParentControl.UpdateAnnotations();
                    }
                }
            }
        }

        private void DeleteLineOrDot(MouseButtonEventArgs e)
        {
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation == null) return;
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Count == 0) return;
            if (e.ChangedButton == MouseButton.Left &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count == 1 &&
                AgeReadingViewModel.AgeReadingEditorView.Cursor == Cursors.Hand)
            {

                Tuple<Dot, double> closestDot = new Tuple<Dot, double>(new Dot(), 100);

                var allDots = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots;
                closestDot = CalculateClosestDot(allDots,
                    e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                bool dotdeleted = false;
                if (allDots.Count > 0 && closestDot.Item2 <= 20)
                {
                    if (AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any(x => x.IsFixed) &&
                        AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.FirstOrDefault(x => x.IsFixed)
                            .Dots.Any(x => x.Location == closestDot.Item1.Location && x.IsFixed))
                    {
                        Helper.ShowWinUIMessageBox("Cannot delete a pinned Dot!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    AgeReadingViewModel.AgeReadingAnnotationViewModel?.WorkingAnnotation.CombinedLines[0].Dots.Remove(
                        closestDot.Item1);
                    closestDot.Item1.ParentCombinedLine =
                        AgeReadingViewModel.AgeReadingAnnotationViewModel?.WorkingAnnotation.CombinedLines[0];
                    UndoRedo.InsertInUnDoRedoForDeleteDot(closestDot.Item1, this);
                    ActiveCombinedLine = closestDot.Item1.ParentCombinedLine;
                    dotdeleted = true;
                }
                // Anders verwijder de lijn
                else if (!dotdeleted)
                {
                    if (AgeReadingViewModel.AgeReadingAnnotationViewModel.Outcomes.Any(x => x.IsFixed) && ActiveCombinedLine.IsFixed)
                    {
                        Helper.ShowWinUIMessageBox("Cannot delete a pinned Line!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(
                        ActiveCombinedLine);
                    UndoRedo.InsertInUnDoRedoForDelete(ActiveCombinedLine, this);
                    ActiveCombinedLine = null;
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                    Mode = EditorModeEnum.DrawLine;
                }
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
                RefreshShapes();
            }
        }

        private Tuple<Dot, double> CalculateClosestDot(List<Dot> allDots, Point p)
        {
            if (!allDots.Any()) return null;
            Dot dot = allDots[0];
            var smallestDistance =
                Math.Abs(Math.Pow(p.X/AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - allDots[0].X, 2) +
                         Math.Pow(p.Y/AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - allDots[0].Y, 2));
            foreach (Dot d in allDots)
            {
                var distance =
                    Math.Abs(Math.Pow(p.X/AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - d.X, 2) +
                             Math.Pow(p.Y/AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - d.Y, 2));

                if (distance < smallestDistance)
                {
                    dot = d;
                    smallestDistance = distance;
                }
            }
            smallestDistance = Math.Sqrt(smallestDistance);
            return new Tuple<Dot, double>(dot, smallestDistance);
        }

        private double distance(LinePoint source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        }

        public void ParentCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                {
                    Tuple<Dot, double> closestDot = new Tuple<Dot, double>(new Dot(), 100);

                    //Dichtste punt wordt berekend
                    var allDots = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots;
                    closestDot = CalculateClosestDot(allDots,
                        e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                    if (allDots.Count > 0 && closestDot.Item2 <= 20)
                    {
                        AgeReadingViewModel.AgeReadingStatusbarViewModel.Info =
                            $"Age {closestDot.Item1.DotIndex} ({closestDot.Item1.DotType})";
                    }
                    else
                    {
                        AgeReadingViewModel.AgeReadingStatusbarViewModel.Info = "";
                    }
                }

                if (Mode == EditorModeEnum.MakingLine && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                {
                    float zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;

                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last().Lines.Last().X2
                        = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X / zoomfactor);
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last().Lines.Last().Y2
                        = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y / zoomfactor);
                    LineShapes.Last().X2 = (int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X;
                    LineShapes.Last().Y2 = (int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y;
                }
                else if (Mode == EditorModeEnum.DrawDot &&
                         AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                         AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                {






                    Tuple<LinePoint, double> closestPoint =
                        GetClosestLinePointWithDistance(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                    if (closestPoint != null)
                    {
                        AgeReadingViewModel.AgeReadingEditorView.Tracker.SetLeft(closestPoint.Item1.X *
                                                                                 AgeReadingViewModel
                                                                                     .AgeReadingStatusbarViewModel
                                                                                     .ZoomFactor - 16);
                        AgeReadingViewModel.AgeReadingEditorView.Tracker.SetTop(closestPoint.Item1.Y *
                                                                                AgeReadingViewModel
                                                                                    .AgeReadingStatusbarViewModel.ZoomFactor -
                                                                                16);

                        if (closestPoint.Item2 <= 50)
                        {
                            AgeReadingViewModel.AgeReadingEditorView.Tracker.Visibility = Visibility.Visible;
                            if (!ActiveCombinedLine.Equals(closestPoint.Item1.ParentCombinedLine))
                            {
                                ActiveCombinedLine = closestPoint.Item1.ParentCombinedLine;
                                AgeReadingViewModel.UpdateGraphs();
                            }

                            AgeReadingViewModel.AgeReadingView.BrightnessGraph.m_MouseLocationTracker.X =
                                closestPoint.Item1.Index;
                            AgeReadingViewModel.AgeReadingView.BrightnessGraph.m_MouseLocationTracker.Hidden = false;

                            AgeReadingViewModel.AgeReadingView.RednessGraph.m_MouseLocationTracker.X =
                                closestPoint.Item1.Index;
                            AgeReadingViewModel.AgeReadingView.RednessGraph.m_MouseLocationTracker.Hidden = false;

                        }
                        else
                        {
                            AgeReadingViewModel.AgeReadingEditorView.Tracker.Visibility = Visibility.Hidden;
                            AgeReadingViewModel.AgeReadingView.BrightnessGraph.m_MouseLocationTracker.Hidden = true;
                        }
                    }
                }
                else if ((Mode == EditorModeEnum.Delete || Mode == EditorModeEnum.SelectLine) &&
                         AgeReadingViewModel.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count == 1)
                {
                    if (AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations[0].CombinedLines.Count > 0)
                    {
                        Tuple<LinePoint, double> linepointwithdistance =
                            GetClosestLinePointWithDistance(
                                e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                        if (linepointwithdistance.Item2 <= 30)
                        {
                            AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                        }
                    }
                    else
                    {
                        AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                    }
                }
                else if (Mode == EditorModeEnum.DrawScale && ScaleShapes.Any())
                {
                    var position = e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas);
                    var line = ScaleShapes.Last();
                    line.X2 = position.X;
                    var zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
                    AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text =
                        Math.Sqrt(
                            (int)
                            Math.Abs(Math.Pow((line.X2 - line.X1) / zoomfactor, 2) +
                                     Math.Pow((line.Y2 - line.Y1) / zoomfactor, 2))).ToString("N0");
                }
            }
            catch (Exception)
            {
                //
            }
            
        }

        public void DrawLineBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.DrawLine;
        }

        public void DrawDotBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.DrawDot;
        }

        public void SelectLineBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.SelectLine;
        }

        public void MoveBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.DragImage;
        }

        public void DeleteBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.Delete;
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            //AgeReadingEditorViewModel.Guess();
        }

        public void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.Undo();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
            UpdateButtons();
        }

        public void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.Redo();
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
            UpdateButtons();
        }

        public void AutoMeasureScale(bool buttonPressed = false)
        {
            if (!buttonPressed)
            {
                if(MeasuredFileIDs.Contains(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID)) return;
                else
                {
                    MeasuredFileIDs.Add(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID);
                }
            }

            if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFileLocation != null)
            {
                try
                {

                        GetPixelLength();

                    
                }
                catch (Exception e)
                {
                    Helper.ShowWinUIMessageBox("Could not determine the scale automatically. Please measure the scale manually", "Info", MessageBoxButton.OK, MessageBoxImage.Information, e);
                }
            }
        }

        public void ManualMeasureScale()
        {
            Mode = EditorModeEnum.DrawScale;
            AgeReadingViewModel.EnableUI(false);
            AgeReadingViewModel.AgeReadingEditorView.MeasureScalePanel.Visibility = Visibility.Visible;
        }

        public void DeleteMeasureScale()
        {
            AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale = null;
            AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
            var dtofile = (DtoFile)Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
            var deleteResult = WebAPI.UpdateFile(dtofile);
            if (!deleteResult.Succeeded)
                Helper.ShowWinUIMessageBox(deleteResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ClearScaleLine()
        {
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            Mode = EditorModeEnum.DrawScale;
        }

        public void AcceptScale()
        {
            double pixels;
            double.TryParse(AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out pixels);
            double milimeters;
            double.TryParse(AgeReadingViewModel.AgeReadingEditorView.ScaleMilimeters.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out milimeters);
            
            PixelLength = (decimal) (pixels/milimeters);
            var oldvalue = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale;
            var newvalue = PixelLength;
            if (oldvalue != newvalue)
            {
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale = PixelLength;
                AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
                DtoFile dtofile = (DtoFile)Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
                var updateFileResult = WebAPI.UpdateFile(dtofile);
                if (!updateFileResult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error saving File to Web API\n" + updateFileResult.ErrorMessage, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            AgeReadingViewModel.AgeReadingEditorView.MeasureScalePanel.Visibility = Visibility.Collapsed;
            Mode = EditorModeEnum.None;
            AgeReadingViewModel.EnableUI(true);
        }

        public void CancelScale()
        {
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            AgeReadingViewModel.AgeReadingEditorView.MeasureScalePanel.Visibility = Visibility.Collapsed;
            Mode = EditorModeEnum.None;
            AgeReadingViewModel.EnableUI(true);
        }
    }
}
