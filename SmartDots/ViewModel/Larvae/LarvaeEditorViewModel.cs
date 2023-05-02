using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
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
using System.Windows.Threading;
using Accord.IO;
using DevExpress.Mvvm;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Helpers;
using SmartDots.Model;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Line = System.Windows.Shapes.Line;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;

namespace SmartDots.ViewModel
{
    public class LarvaeEditorViewModel : LarvaeBaseViewModel
    {
        private object lineColor;
        private object measureColor;
        private int lineWidth;
        private int dotWidth;
        private int circleWidth;
        private int measureLineWidth;
        private int measureFontSize;
        private LarvaeUndoRedo undoRedo;
        private ObservableCollection<Line> lineShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> scaleShapes = new ObservableCollection<Line>();
        private ObservableCollection<Shape> dotShapes = new ObservableCollection<Shape>();
        private ObservableCollection<Shape> circleShapes = new ObservableCollection<Shape>();

        private ObservableCollection<Line> originalMeasureShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> measureShapes = new ObservableCollection<Line>();
        private ObservableCollection<TextBlock> textShapes = new ObservableCollection<TextBlock>();
        private CombinedLine activeCombinedLine;
        private CombinedLine tempCombinedLine;
        private EditorModeEnum mode;
        private Bitmap originalImage;
        private bool shapeChangeFlag;
        private BitmapImage _larvaeImage;
        private int originalWidth;
        private int originalHeight;
        private int width;
        private int height;
        private double brightness;
        private double contrast;
        private decimal pixelLength;
        private bool hideLines;
        private bool isScaleDrawn;
        private bool isMeasureDrawn;
        private string measureUnit = "mm";


        public object LineColor
        {
            get { return lineColor; }
            set
            {
                lineColor = value;
                RaisePropertyChanged("LineColor");
            }
        }

        public object MeasureColor
        {
            get { return measureColor; }
            set
            {
                measureColor = value;
                RaisePropertyChanged("MeasureColor");
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

        public object CircleWidth
        {
            get { return circleWidth; }
            set
            {
                circleWidth = int.Parse(value.ToString());
                RaisePropertyChanged("CircleWidth");
            }
        }

        public object MeasureLineWidth
        {
            get { return measureLineWidth; }
            set
            {
                measureLineWidth = int.Parse(value.ToString());
                RaisePropertyChanged("MeasureLineWidth");
            }
        }

        public object MeasureFontSize
        {
            get { return measureFontSize; }
            set
            {
                measureFontSize = int.Parse(value.ToString());
                RaisePropertyChanged("MeasureFontSize");
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

        public ObservableCollection<Shape> DotShapes
        {
            get { return dotShapes; }
            set
            {
                dotShapes = value;
                RaisePropertyChanged("DotShapes");
            }
        }

        public ObservableCollection<Shape> CircleShapes
        {
            get { return circleShapes; }
            set
            {
                circleShapes = value;
                RaisePropertyChanged("CircleShapes");
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

        public LarvaeUndoRedo UndoRedo
        {
            get { return undoRedo; }
            set { undoRedo = value; }
        }

        public ObservableCollection<Line> OriginalMeasureShapes
        {
            get { return originalMeasureShapes; }
            set { originalMeasureShapes = value; }
        }

        public ObservableCollection<Line> MeasureShapes
        {
            get { return measureShapes; }
            set
            {
                measureShapes = value;
                RaisePropertyChanged("MeasureShapes");
            }
        }

        public ObservableCollection<TextBlock> TextShapes
        {
            get { return textShapes; }
            set
            {
                textShapes = value;
                RaisePropertyChanged("TextShapes");
            }
        }

        public bool CanDrawLine
        {
            get
            {
                return LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Parameter.ShapeType
                           ?.ToLower() == "line";
            }
        }

        public bool CanDrawDot
        {
            get
            {
                return LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Parameter.ShapeType
                           ?.ToLower() == "dot";
            }
        }

        public bool CanDrawCircle
        {
            get
            {
                return LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Parameter.ShapeType
                           ?.ToLower() == "circle";
            }
        }

        public bool CanDelete
        {
            get
            {
                return (LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1)
                       || MeasureShapes.Any();
            }
        }

        public bool CanUndo
        {
            get
            {
                return (LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1
                       && UndoRedo._Undocommands.Count > 0) || (UndoRedo._Undocommands.Count > 0 && (UndoRedo._Undocommands?.Last()?.GetType() == typeof(AddLarvaeMeasureCommand) || UndoRedo._Undocommands?.Last()?.GetType() == typeof(DeleteLarvaeMeasureCommand)));

            }
            set { }
        }

        public bool CanRedo
        {
            get
            {
                return (LarvaeViewModel?.LarvaeSampleViewModel?.SelectedSample != null
                       && LarvaeViewModel.ActiveTab == "AnnotationOwn"
                       && !LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly
                       && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count == 1
                       && UndoRedo._Redocommands.Count > 0) || (UndoRedo._Redocommands.Count > 0 && (UndoRedo._Redocommands?.Last().GetType() == typeof(AddLarvaeMeasureCommand) || UndoRedo._Redocommands?.Last().GetType() == typeof(DeleteLarvaeMeasureCommand)));
            }
        }

        public bool CanSetScale
        {
            get
            {
                return LarvaeImage != null
                && LarvaeViewModel?.LarvaeAnalysis.AllowSetScale != null
                && (bool)(LarvaeViewModel?.LarvaeAnalysis.AllowSetScale);

            }
        }

        public EditorModeEnum Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                if (larvaeViewModel != null)
                {
                    LarvaeViewModel.LarvaeEditorView.LineButton.IsPressed = false;
                    LarvaeViewModel.LarvaeEditorView.DotButton.IsPressed = false;
                    LarvaeViewModel.LarvaeEditorView.CircleButton.IsPressed = false;
                    LarvaeViewModel.LarvaeEditorView.DeleteBtn.IsChecked = false;
                    LarvaeViewModel.LarvaeEditorView.MeasureButton.IsPressed = false;
                    LarvaeViewModel.LarvaeEditorView.ScaleButton.IsPressed = false;

                    switch (value)
                    {
                        case EditorModeEnum.DrawLine:
                        case EditorModeEnum.MakingLine:
                            LarvaeViewModel.LarvaeEditorView.LineButton.IsPressed = true;
                            break;
                        case EditorModeEnum.DrawDot:
                            LarvaeViewModel.LarvaeEditorView.DotButton.IsPressed = true;
                            break;
                        case EditorModeEnum.DrawCircle:
                        case EditorModeEnum.MakingCircle:
                            LarvaeViewModel.LarvaeEditorView.CircleButton.IsPressed = true;
                            break;
                        case EditorModeEnum.Delete:
                            LarvaeViewModel.LarvaeEditorView.DeleteBtn.IsChecked = true;
                            break;
                        case EditorModeEnum.Measure:
                        case EditorModeEnum.MakingMeasure:
                            LarvaeViewModel.LarvaeEditorView.MeasureButton.IsPressed = true;
                            break;
                        case EditorModeEnum.DrawScale:
                            LarvaeViewModel.LarvaeEditorView.ScaleButton.IsPressed = true;
                            break;
                        case EditorModeEnum.None:
                            LarvaeViewModel.LarvaeEditorView.LineButton.IsPressed = false;
                            LarvaeViewModel.LarvaeEditorView.DotButton.IsPressed = false;
                            LarvaeViewModel.LarvaeEditorView.CircleButton.IsPressed = false;
                            LarvaeViewModel.LarvaeEditorView.DeleteBtn.IsChecked = false;
                            LarvaeViewModel.LarvaeEditorView.MeasureButton.IsPressed = false;
                            LarvaeViewModel.EnableUI(true);
                            break;

                    }
                }

                UpdateButtons();
                RaisePropertyChanged("Mode");
            }
        }

        public BitmapImage LarvaeImage
        {
            get { return _larvaeImage; }
            set
            {
                _larvaeImage = value;
                if (value != null)
                {
                    OriginalWidth = _larvaeImage.PixelWidth;
                    OriginalHeight = _larvaeImage.PixelHeight;
                }
                else
                {
                    OriginalWidth = 0;
                    OriginalHeight = 0;
                }


                //Width = _larvaeImage.PixelWidth;
                //Height = _larvaeImage.PixelHeight;
                LarvaeViewModel.LarvaeStatusbarView.lblImgSize.Content = $"{OriginalWidth} x {OriginalHeight}";

                RaisePropertyChanged("LarvaeImage");
                RaisePropertyChanged("CanMeasure");
                RaisePropertyChanged("CanSetScale");

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
                LarvaeViewModel.LarvaeEditorView.ScrollViewer.HorizontalScrollBarVisibility = width >
                    LarvaeViewModel
                        .LarvaeEditorView
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
                LarvaeViewModel.LarvaeEditorView.ScrollViewer.VerticalScrollBarVisibility = height >
                    LarvaeViewModel
                        .LarvaeEditorView
                        .ScrollViewer
                        .Height
                        ? ScrollBarVisibility.Hidden
                        : ScrollBarVisibility.Auto;
                RaisePropertyChanged("Height");
            }
        }

        public double Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                if (LarvaeViewModel != null && !LarvaeViewModel.LarvaeFileViewModel.ChangingFile)
                {
                    AdjustImage();
                    RaisePropertyChanged("LarvaeImage");
                }

                RaisePropertyChanged("Brightness");
            }
        }

        public double Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                if (LarvaeViewModel != null && !LarvaeViewModel.LarvaeFileViewModel.ChangingFile)
                {
                    AdjustImage();
                    RaisePropertyChanged("LarvaeImage");
                }

                RaisePropertyChanged("Contrast");
            }
        }

        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set
            {
                originalImage = value;
                if (value != null)
                {
                    if (Global.API.Settings.AutoMeasureScale &&
                        (LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale == null ||
                         LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale == 0.0m))
                    {
                        AutoMeasureScale();
                    }
                }

            }
        }

        public decimal PixelLength
        {
            get { return pixelLength; }
            set { pixelLength = value; }
        }


        public bool CanMeasure
        {
            get { return LarvaeImage != null; }
        }


        public bool ShapeChangeFlag
        {
            get { return shapeChangeFlag; }
            set { shapeChangeFlag = value; }
        }

        public bool IsScaleDrawn
        {
            get { return isScaleDrawn; }
            set
            {
                isScaleDrawn = value;
                //if (isScaleDrawn)
                //{
                //    LarvaeViewModel.LarvaeEditorView.ScaleDrawn.Visibility = Visibility.Visible;
                //    LarvaeViewModel.LarvaeEditorView.ScaleNotDrawn.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    LarvaeViewModel.LarvaeEditorView.ScaleDrawn.Visibility = Visibility.Collapsed;
                //    LarvaeViewModel.LarvaeEditorView.ScaleNotDrawn.Visibility = Visibility.Visible;
                //}
                RaisePropertyChanged("IsScaleDrawn");
            }
        }

        public bool IsMeasureDrawn
        {
            get { return isMeasureDrawn; }
            set
            {
                isMeasureDrawn = value;
                RaisePropertyChanged("IsScaleDrawn");
            }
        }

        public ObservableCollection<string> Units { get; set; } = new ObservableCollection<string>
            {"µm", "mm", "cm"};

        public string MeasureUnit
        {
            get { return measureUnit; }
            set
            {
                measureUnit = value;
                RaisePropertyChanged("MeasureUnit");
            }
        }
        public Visibility LineButtonVisibility
        {
            get
            {
                if (LarvaeViewModel != null 
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Any(x => x.Parameter.ShapeType.ToLower() == "line")
                    )
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility DotButtonVisibility
        {
            get
            {
                if (LarvaeViewModel != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Any(x => x.Parameter.ShapeType.ToLower() == "dot")
                )
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility CircleButtonVisibility
        {
            get
            {
                if (LarvaeViewModel != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult != null
                    && LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Any(x => x.Parameter.ShapeType.ToLower() == "circle")
                )
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }
        public bool IsContextmenuOpen { get; set; }
        //public System.Drawing.Rectangle ScaleRectangle { get; private set; }
        public bool IsMeasuring { get; private set; }
        public List<Guid> MeasuredFileIDs { get; set; } = new List<Guid>();




        public LarvaeEditorViewModel()
        {
            Mode = EditorModeEnum.None;
            Brightness = 0;
            Contrast = 0;
            LineColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
            MeasureColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
            LineWidth = 2;
            DotWidth = 12;
            CircleWidth = 4;
            MeasureLineWidth = 2;
            MeasureFontSize = 12;
            UndoRedo = new LarvaeUndoRedo(this);
            LoadUserPreferences();
        }

        public async void GetPixelLength()
        {
            try
            {
                var selectedFileId = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID;
                var oldvalue = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale;
                LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = false;

                decimal temp = await Task.Run(() => ScaleMeasureTool.Measure(OriginalImage, 128));

                if (LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID != selectedFileId)
                {
                    LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = true;
                    return;
                }

                PixelLength = temp;

                if (PixelLength == 0) throw new Exception("Could not determine the scale");

                var newvalue = PixelLength;
                if (oldvalue != newvalue)
                {
                    if (PixelLength == 0)
                    {
                        LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale = null;
                    }
                    else
                    {
                        LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale = PixelLength;
                    }
                    

                    LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = true;
                    DtoLarvaeEggFile dtofile =
                        (DtoLarvaeEggFile)
                        Helper.ConvertType(LarvaeViewModel.LarvaeFileViewModel.SelectedFile, typeof(DtoLarvaeEggFile));
                    var updateFileResult = Global.API.UpdateLarvaeFile(LarvaeViewModel.LarvaeAnalysis.Type, dtofile);
                    if (!updateFileResult.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox(
                            $"Error saving {LarvaeViewModel.LarvaeAnalysis.Type} File to Web API\n" + updateFileResult.ErrorMessage, "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = false;
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

            if (PixelLength == 0)
            {
                LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): ?";
            }
            else
            {
                LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): {PixelLength}";
            }
            

            LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = true;
        }

        public void SaveUserPreferences()
        {
            try
            {
                Properties.Settings.Default.LineColor = LineColor.ToString();
                Properties.Settings.Default.LineWidth = (int)LineWidth;
                Properties.Settings.Default.MeasureColor = MeasureColor.ToString();
                Properties.Settings.Default.MeasureLineWidth = (int)MeasureLineWidth;
                Properties.Settings.Default.MeasureFontSize = (int)MeasureFontSize;
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
            RaisePropertyChanged("LineButtonVisibility");
            RaisePropertyChanged("DotButtonVisibility");
            RaisePropertyChanged("CircleButtonVisibility");
            RaisePropertyChanged("CanDrawLine");
            RaisePropertyChanged("CanDrawDot");
            RaisePropertyChanged("CanDrawCircle");
            RaisePropertyChanged("CanDelete");
            RaisePropertyChanged("CanUndo");
            RaisePropertyChanged("CanRedo");
            RaisePropertyChanged("CanSetScale");
            RaisePropertyChanged("ContextmenuVisibility");
        }

        public void LoadUserPreferences()
        {
            try
            {
                LineColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.LineColor);
                LineWidth = Properties.Settings.Default.LineWidth;
                DotWidth = Properties.Settings.Default.DotWidth;
                CircleWidth = Properties.Settings.Default.CircleWidth;
                MeasureColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.MeasureColor);
                MeasureLineWidth = Properties.Settings.Default.MeasureLineWidth;
                MeasureFontSize = Properties.Settings.Default.MeasureFontSize;
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error saving user preferences", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, e);
            }
        }

        public void RefreshShapes()
        {
            LarvaeViewModel.LarvaeEditorView.Dispatcher.Invoke(() =>
            {
                LineShapes = new ObservableCollection<Line>();
                DotShapes = new ObservableCollection<Shape>();
                float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
                ObservableCollection<Line> lines = new ObservableCollection<Line>();
                ObservableCollection<Shape> dots = new ObservableCollection<Shape>();
                ObservableCollection<Shape> circles = new ObservableCollection<Shape>();

                var visibleParameterResults = new List<LarvaeAnnotationParameterResult>();


                bool allAnnotations = LarvaeViewModel.ActiveTab == "AnnotationAll";

                if (allAnnotations)
                {
                    var selectedAllParameterResults =
                        LarvaeViewModel.LarvaeAllAnnotationViewModel.SelectedLarvaeAnnotationParameterResults.ToList();
                    foreach (var p in selectedAllParameterResults.Where(x =>
                        x.IsVisible && x.FileID == LarvaeViewModel.LarvaeFileViewModel.SelectedFile?.ID))
                    {
                        LarvaeAnnotationParameterResult l = p.DeepClone();
                        l.IsVisible = true;
                        visibleParameterResults.Add(l);
                    }
                }
                else
                {
                    if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation != null)
                    {
                        visibleParameterResults.AddRange(LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Where(x =>
                            x.IsVisible && x.FileID == LarvaeViewModel.LarvaeFileViewModel.SelectedFile?.ID).ToList());
                    }
                }

                foreach (var parameter in visibleParameterResults)
                {
                    var color = parameter.Parameter.Color;
                    if (allAnnotations) color = parameter.MultiUserColor;
                    foreach (LarvaeLine l in parameter.Lines)
                    {
                        Line line = new Line()
                        {
                            X1 = (float)l.X1 * zoomfactor,
                            Y1 = (float)l.Y1 * zoomfactor,
                            X2 = (float)l.X2 * zoomfactor,
                            Y2 = (float)l.Y2 * zoomfactor,
                            Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(color)),
                            StrokeThickness = (float)l.Width * zoomfactor,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round,
                            Tag = l.ID.ToString().ToUpper()

                        };
                        lines.Add(line);
                    }

                    foreach (LarvaeDot d in parameter.Dots)
                    {

                        SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));

                        Shape shape = new Ellipse()
                        {
                            Fill = brush,
                            Width = (float)d.Width * zoomfactor,
                            Height = (float)d.Width * zoomfactor,
                            SnapsToDevicePixels = false

                        };
                        var left = (float)d.X * zoomfactor - (float)d.Width / 2 * zoomfactor;
                        var top = (float)d.Y * zoomfactor - (float)d.Width / 2 * zoomfactor;
                        Canvas.SetLeft(shape, left);
                        Canvas.SetTop(shape, top);
                        dots.Add(shape);
                    }

                    if (parameter.Circle != null)
                    {
                        LarvaeCircle c = parameter.Circle;
                        SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));

                        Shape shape = new Ellipse()
                        {
                            Stroke = brush,
                            StrokeThickness = c.Width * zoomfactor,
                            Width = (float)c.Diameter * zoomfactor,
                            Height = (float)c.Diameter * zoomfactor,
                            SnapsToDevicePixels = false
                        };
                        var left = (float)c.CenterX * zoomfactor - (float)c.Diameter / 2 * zoomfactor;
                        var top = (float)c.CenterY * zoomfactor - (float)c.Diameter / 2 * zoomfactor;
                        Canvas.SetLeft(shape, left);
                        Canvas.SetTop(shape, top);
                        circles.Add(shape);

                        Line line = new Line()
                        {
                            X1 = (float)c.X1 * zoomfactor,
                            Y1 = (float)c.Y1 * zoomfactor,
                            X2 = (float)c.X2 * zoomfactor,
                            Y2 = (float)c.Y2 * zoomfactor,
                            Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(color)),
                            StrokeThickness = (float)c.Width * zoomfactor,
                            StrokeDashCap = PenLineCap.Round,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round,
                            StrokeDashArray = new DoubleCollection() { 0.5, 2.5 },

                        };
                        circles.Add(line);
                    }
                }

                RefreshMeasures();

                LineShapes = lines;
                DotShapes = dots;
                CircleShapes = circles;
                ScaleShapes = ScaleShapes;
            });
        }

        public void RefreshMeasures(bool onlyLast = false)
        {
            if (LarvaeViewModel?.LarvaeFileViewModel.SelectedFile == null) return;

            float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
            ObservableCollection<Line> measures = new ObservableCollection<Line>();
            ObservableCollection<TextBlock> text = new ObservableCollection<TextBlock>();

            var scale = LarvaeViewModel?.LarvaeFileViewModel.SelectedFile.Scale;
            decimal tmpScale = 1;
            if (scale != null && scale != 0)
            {
                tmpScale = (decimal)scale;
            }


            foreach (var line in originalMeasureShapes)
            {
                var isLongest = true;

                var length = (decimal)Math.Sqrt((int)Math.Abs(Math.Pow((line.X2 - line.X1), 2) + Math.Pow((line.Y2 - line.Y1), 2)));
                var totalLength = length;


                //if (!originalMeasureShapes.Any(x => x != line && int.Parse(((dynamic)x.Tag).LineGroup)) ==
                //        int.Parse(((dynamic)line.Tag).LineGroup))
                //{
                //    isLongest = true;
                //}
                //else
                //{
                foreach (var groupLine in originalMeasureShapes.Where(x => x != line && int.Parse(((dynamic)x.Tag).LineGroup) == int.Parse(((dynamic)line.Tag).LineGroup)))
                {

                    var groupLineLength = (decimal)Math.Sqrt(Math.Abs(Math.Pow((groupLine.X2 - groupLine.X1), 2) + Math.Pow((groupLine.Y2 - groupLine.Y1), 2)));
                    totalLength += groupLineLength;
                    if (groupLineLength == length && groupLine.X1 < line.X1)
                    {
                        isLongest = false;
                        break;
                    }
                    else if (groupLineLength > length)
                    {
                        isLongest = false;
                        break;
                    }
                }
                //}


                Line l = new Line()
                {
                    X1 = line.X1 * zoomfactor,
                    Y1 = line.Y1 * zoomfactor,
                    X2 = line.X2 * zoomfactor,
                    Y2 = line.Y2 * zoomfactor,
                    Stroke = line.Stroke,
                    StrokeThickness = line.StrokeThickness * zoomfactor,
                    StrokeDashCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeDashArray = new DoubleCollection() { 0.25, 1.5 },
                    Uid = line.Uid,
                    IsHitTestVisible = true,
                    Tag = line.Tag
                };

                Line l2 = new Line()
                {
                    X1 = l.X1,
                    Y1 = l.Y1,
                    X2 = l.X2,
                    Y2 = l.Y2,
                    StrokeThickness = 30 * zoomfactor,
                    IsHitTestVisible = true,
                    Uid = l.Uid,
                    Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                };

                measures.Add(l);
                measures.Add(l2);

                if (!isLongest) continue;



                var tb = new TextBlock()
                {
                    Text = "Placeholder",
                    Foreground = l.Stroke,
                    FontSize = int.Parse(((dynamic)l.Tag).FontSize) * zoomfactor,
                };


                if (scale != null && scale != 0)
                {
                    var lengthText = "";

                    totalLength = totalLength / tmpScale;

                    if (totalLength < 0.001m)
                    {
                        var tempLength = totalLength * 1000000;
                        tempLength = decimal.Round((decimal)tempLength, 0);
                        lengthText = tempLength.ToString() + " nm";
                    }
                    else if (totalLength < 1m)
                    {
                        var tempLength = totalLength * 1000;
                        tempLength = decimal.Round((decimal)tempLength, 1);
                        lengthText = tempLength.ToString() + " µm";
                    }
                    else if (totalLength > 100m)
                    {
                        var tempLength = totalLength / 10;
                        tempLength = decimal.Round((decimal)tempLength, 2);
                        lengthText = tempLength.ToString() + " cm";
                    }
                    else
                    {
                        totalLength = decimal.Round((decimal)totalLength, 2);
                        lengthText = totalLength.ToString() + " mm";
                    }

                    tb.Text = lengthText;
                }
                else
                {
                    tb.Text = Math.Sqrt((int)Math.Abs(Math.Pow((line.X2 - line.X1), 2) + Math.Pow((line.Y2 - line.Y1), 2))).ToString("N0") + " px";
                }


                //atan2 for angle
                var radians = Math.Atan2(line.Y2 - line.Y1, line.X2 - line.X1);

                //radians into degrees
                var angle = radians * (180 / Math.PI);

                var size = Helper.MeasureString(tb);

                var left = (line.X2 * zoomfactor + line.X1 * zoomfactor - size.Width) / 2 + Math.Sin(radians) * size.Height * 0.6;
                if (left < 0) left = 0;

                var top = (line.Y2 * zoomfactor + line.Y1 * zoomfactor - size.Height) / 2 - Math.Cos(radians) * size.Height * 0.6;
                if (top < 0) top = 0;

                if (angle < -90 || angle > 90)
                {
                    angle += 180;
                }

                tb.Margin = new Thickness(left, top, 0, 0);

                tb.RenderTransform = new RotateTransform(angle, size.Width / 2, size.Height / 2);

                text.Add(tb);
            }


            MeasureShapes = measures;
            TextShapes = text;
        }

        public void RemoveShapes()
        {
            OriginalMeasureShapes = new ObservableCollection<Line>();
            MeasureShapes = new ObservableCollection<Line>();
            TextShapes = new ObservableCollection<TextBlock>();
        }


        public void AddLine(List<LarvaeLine> l)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Lines.AddRange(l);
            selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            CalculateResult();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void RemoveLine(List<LarvaeLine> l)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Lines.RemoveAll(x => l.Select(y => y.ID).Contains(x.ID));

            selectedParameter.Result = 0;
            selectedParameter.CalculatedResult = null;
            selectedParameter.File = null;

            //CalculateResult();
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void AddDot(LarvaeDot d)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Dots.Add(d);
            selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            CalculateResult();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void RemoveDot(LarvaeDot d)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Dots.Remove(d);

            if (!selectedParameter.Dots.Any())
            {
                selectedParameter.FileID = Guid.Empty;
                selectedParameter.File = null;
            }
            selectedParameter.CalculatedResult = null;
            CalculateResult();
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void AddCircle(LarvaeCircle c)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Circle = c;
            selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            CalculateResult();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void RemoveCircle(LarvaeCircle c)
        {
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            selectedParameter.Circle = null;

            selectedParameter.Result = 0;
            selectedParameter.CalculatedResult = null;
            selectedParameter.File = null;
            selectedParameter.FileID = Guid.Empty;
            //CalculateResult();
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
            RefreshShapes();
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            ShapeChangeFlag = true;
        }

        public void AdjustImage()
        {
            try
            {
                if (OriginalImage != null)
                {
                    if (Brightness == 0 && Contrast == 0)
                    {
                        LarvaeImage = BitmapConverter.Bitmap2BitmapImage(OriginalImage);
                    }
                    else
                    {
                        LarvaeImage = ImageManipulator.SetBrightnessContrast(new Bitmap(OriginalImage), Brightness, Contrast);
                    }

                }
            }
            catch (Exception e)
            {
                Helper.ShowWinUIMessageBox("Error adjusting image", "Error", MessageBoxButton.OK, MessageBoxImage.Error,
                    e);
            }
        }
        public void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LarvaeViewModel.LarvaeStatusbarViewModel.IsFittingImage)
            {
                LarvaeViewModel.LarvaeStatusbarViewModel.FitImage();
            }
            else
            {
                RefreshShapes();
            }
        }

        public void ParentCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //here comes the code for drawing, dotting, deleting

            if (IsContextmenuOpen) return;


            if (e.ChangedButton == MouseButton.Left)
            {
                if (LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly && !(Mode == EditorModeEnum.DrawScale || Mode == EditorModeEnum.Measure || Mode == EditorModeEnum.MakingMeasure || Mode == EditorModeEnum.Delete))
                {
                    new WinUIMessageBoxService().Show("The selected sample is ReadOnly!", "Error", MessageBoxButton.OK,
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
                    case EditorModeEnum.DrawCircle:
                        DrawCircle(e);
                        break;
                    case EditorModeEnum.MakingCircle:
                        MakeCircle(e);
                        break;
                    case EditorModeEnum.Delete:
                        DeleteAnnotation(e);
                        break;
                    case EditorModeEnum.DrawScale:
                        DrawScale(e);
                        break;
                    case EditorModeEnum.Measure:
                        DrawMeasure(e);
                        break;
                    case EditorModeEnum.MakingMeasure:
                        MakingMeasure(e);
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
                    case EditorModeEnum.MakingMeasure:
                        MakingMeasure(e);
                        break;
                    case EditorModeEnum.MakingCircle:
                        StopMakeCircle(e);
                        break;
                }

            }
            UpdateButtons();
        }

        private void DrawScale(MouseButtonEventArgs e)
        {
            if (!ScaleShapes.Any())
            {
                float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
                var position = e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas);
                Line line = new Line()
                {
                    X1 = position.X,
                    Y1 = position.Y,
                    X2 = position.X,
                    Y2 = position.Y,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(LineColor.ToString())),
                    StrokeThickness = ((int)LineWidth * zoomfactor),
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

        private void DrawMeasure(MouseButtonEventArgs e)
        {
            //if (!MeasureShapes.Any())
            //{
            float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
            var position = e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas);
            var id = Guid.NewGuid();
            Line line = new Line()
            {
                X1 = position.X / zoomfactor,
                Y1 = position.Y / zoomfactor,
                X2 = position.X / zoomfactor,
                Y2 = position.Y / zoomfactor,
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(MeasureColor.ToString())),
                StrokeThickness = (int)MeasureLineWidth,
                StrokeDashCap = PenLineCap.Round,
                StrokeDashArray = new DoubleCollection() { 0.25, 1.5 },
                Uid = id.ToString(),



            };

            string lineGroup = OriginalMeasureShapes.Count.ToString();

            dynamic tag = new ExpandoObject();
            tag.FontSize = MeasureFontSize.ToString();
            tag.LineGroup = lineGroup;
            line.Tag = tag;

            OriginalMeasureShapes.Add(line);
            Mode = EditorModeEnum.MakingMeasure;
            LarvaeViewModel.EnableUI(false);

            //}
            //else
            //{
            //    Mode = EditorModeEnum.None;
            //}
            RefreshShapes();
        }

        private void MakingMeasure(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                OriginalMeasureShapes.Remove(OriginalMeasureShapes.Last());
                //UndoRedo.Undo();

                CancelMeasure();
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                Line l = new Line()
                {
                    X1 = OriginalMeasureShapes.Last().X2,
                    Y1 = OriginalMeasureShapes.Last().Y2,
                    X2 = OriginalMeasureShapes.Last().X2,
                    Y2 = OriginalMeasureShapes.Last().Y2,
                    Stroke = OriginalMeasureShapes.Last().Stroke,
                    StrokeThickness = OriginalMeasureShapes.Last().StrokeThickness,
                    Tag = OriginalMeasureShapes.Last().Tag
                };

                UndoRedo.InsertInUnDoRedoForAddMeasure(OriginalMeasureShapes.Last(), this);


                OriginalMeasureShapes.Add(l);
            }

            RefreshMeasures();
            UpdateButtons();
        }

        private void DrawLine(MouseButtonEventArgs e)
        {
            if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;
            if (!LarvaeViewModel.LarvaeOwnAnnotationViewModel.CanEdit) return;
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation == null)
            {
                LarvaeViewModel.LarvaeOwnAnnotationViewModel.CreateNewAnnotation();
            }
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters == null || LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count != 1) return;
            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];

            if (selectedParameter == null) return;
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Any())
            {
                var result = Helper.ShowWinUIDialog($"There is already a line drawn for this parameter. Do you want to delete it and draw a new one?", $"Line already present", MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    UndoRedo.InsertInUnDoRedoForDelete(new List<LarvaeLine>(selectedParameter.Lines), this);

                    RemoveLine(selectedParameter.Lines);
                    return;
                }
                else
                {
                    return;
                }
            }

            LarvaeViewModel.EnableUI(false);
            float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;


            var lapr = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];

            lapr.FileID = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID;
            lapr.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            lapr.AnnotationID = LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.ID;
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();

            LarvaeLine l = new LarvaeLine()
            {
                ID = Guid.NewGuid(),
                X1 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor),
                X2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor),
                Y1 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor),
                Y2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor),
                Width = (int)LineWidth,
                LineIndex = 0,

            };

            // a parameter that is being drawn should always be visible
            lapr.IsVisible = true;

            lapr.Lines.Add(l);
            Mode = EditorModeEnum.MakingLine;

            //UndoRedo.InsertInUnDoRedoForAddLine(lapr.Lines, this);
            RefreshShapes();
        }

        private void MakeLine(MouseButtonEventArgs e)
        {
            try
            {
                if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;
                if (Mode == EditorModeEnum.MakingLine)
                {
                    float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;

                    LarvaeLine l = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Last();

                    l.X2 = (int)(e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor);
                    l.Y2
                        = (int)(e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor);


                    LarvaeLine line = new LarvaeLine()
                    {
                        ID = Guid.NewGuid(),
                        X1 = l.X2,
                        X2 = l.X2,
                        Y1 = l.Y2,
                        Y2 = l.Y2,
                        Width = (int)LineWidth,
                        LineIndex = l.LineIndex + 1
                    };

                    LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Add(line);

                    RefreshShapes();
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
                if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;

                var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
                if (selectedParameter.Lines.Any())
                {
                    LarvaeLine last = selectedParameter.Lines.Last();

                    selectedParameter.Lines.Remove(last);
                    

                    if (!selectedParameter.Lines.Any())
                    {
                        selectedParameter.FileID = Guid.Empty;
                        selectedParameter.File = null;
                        Mode = EditorModeEnum.DrawLine;
                    }
                    else
                    {
                        UndoRedo.InsertInUnDoRedoForAddLine(selectedParameter.Lines, this);
                        Mode = EditorModeEnum.None;
                    }
                    RefreshShapes();
                    UpdateButtons();

                    CalculateResult();
                    LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;

                    LarvaeViewModel.EnableUI(true);
                }
            }
            catch (Exception)
            {
                Mode = EditorModeEnum.DrawLine;
                LarvaeViewModel.EnableUI(true);

            }
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
            LarvaeViewModel.EnableUI(true);
            UpdateButtons();


        }

        private void DrawDot(MouseButtonEventArgs e)
        {
            if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;
            if (!LarvaeViewModel.LarvaeOwnAnnotationViewModel.CanEdit) return;
            if (!CanDrawDot) return;

            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation == null)
            {
                LarvaeViewModel.LarvaeOwnAnnotationViewModel.CreateNewAnnotation();
            }
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters == null || LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count > 1) return;

            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];

            if (selectedParameter == null) return;

            if (selectedParameter.Dots.Any() && selectedParameter.FileID != LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID)
            {
                var result = Helper.ShowWinUIDialog($"There are already dots drawn for this parameter on another image. Do you want to delete them?", $"Dots already present", MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    selectedParameter.Dots.Clear();

                    selectedParameter.File = null;
                    selectedParameter.CalculatedResult = null;
                    CalculateResult();
                    LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
                    RefreshShapes();
                    LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
                    ShapeChangeFlag = true;
                    return;
                }
                else
                {
                    return;
                }
            }
            //Tuple<LinePoint, double> closestPoint =
            //    GetClosestLinePointWithDistance(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
            //if (closestPoint != null)
            //{
            //    if (closestPoint.Item2 <= 50)
            //    {
            //        if (
            //            !closestPoint.Item1.ParentCombinedLine.ContainsDotAt(new Point(closestPoint.Item1.X,
            //                closestPoint.Item1.Y)))
            //        {
            float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;


            selectedParameter.FileID = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID;
            selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();

            LarvaeDot d = new LarvaeDot()
            {
                ID = Guid.NewGuid(),
                X = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor),
                Y = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor),
                Width = (int)DotWidth,
            };

            // a parameter that is being drawn should always be visible
            selectedParameter.IsVisible = true;
            var closestDot = CalculateClosestDot(selectedParameter.Dots,
                e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas));
            if (closestDot == null || closestDot.Item2 >= closestDot.Item1.Width)
            {
                selectedParameter.Dots.Add(d);
                UndoRedo.InsertInUnDoRedoForAddDot(d, this);
                RefreshShapes();
                UpdateButtons();
                CalculateResult();
                LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
            }
        }

        private void DrawCircle(MouseButtonEventArgs e)
        {
            if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;
            if (!LarvaeViewModel.LarvaeOwnAnnotationViewModel.CanEdit) return;
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation == null)
            {
                LarvaeViewModel.LarvaeOwnAnnotationViewModel.CreateNewAnnotation();
            }
            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters == null || LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count > 1) return;


            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];

            if (selectedParameter == null) return;


            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Circle != null)
            {
                var result = Helper.ShowWinUIDialog($"There is already a line drawn for this parameter. Do you want to delete it and draw a new one?", $"Circle already present", MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    UndoRedo.InsertInUnDoRedoForDeleteCircle(selectedParameter.Circle, this);

                    RemoveCircle(selectedParameter.Circle);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (!CanDrawCircle) return;
            //Tuple<LinePoint, double> closestPoint =
            //    GetClosestLinePointWithDistance(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
            //if (closestPoint != null)
            //{
            //    if (closestPoint.Item2 <= 50)
            //    {
            //        if (
            //            !closestPoint.Item1.ParentCombinedLine.ContainsDotAt(new Point(closestPoint.Item1.X,
            //                closestPoint.Item1.Y)))
            //        {
            float zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;


            selectedParameter.FileID = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID;
            selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();

            LarvaeCircle c = new LarvaeCircle()
            {
                ID = Guid.NewGuid(),
                X1 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor),
                Y1 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor),
                X2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor),
                Y2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor),
                Width = (int)CircleWidth,
            };

            // a parameter that is being drawn should always be visible
            selectedParameter.IsVisible = true;

            selectedParameter.Circle = c;
            UndoRedo.InsertInUnDoRedoForAddCircle(c, this);
            Mode = EditorModeEnum.MakingCircle;
            RefreshShapes();
            UpdateButtons();
            CalculateResult();
            LarvaeViewModel.EnableUI(false);
            LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
        }

        private void MakeCircle(MouseButtonEventArgs e)
        {
            try
            {
                if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;

                if (Mode == EditorModeEnum.MakingCircle)
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Mode = EditorModeEnum.None;
                    }

                    CalculateResult();
                    RefreshShapes();
                    LarvaeViewModel.EnableUI(true);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private void StopMakeCircle(MouseButtonEventArgs e)
        {
            try
            {
                if (LarvaeViewModel.ActiveTab != "AnnotationOwn") return;

                if (Mode == EditorModeEnum.MakingCircle)
                {
                    if (e.ChangedButton == MouseButton.Right)
                    {
                        var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
                        selectedParameter.Circle = null;
                        selectedParameter.FileID = Guid.Empty;
                        selectedParameter.File = null;
                        Mode = EditorModeEnum.DrawCircle;
                    }

                    CalculateResult();
                    RefreshShapes();
                    LarvaeViewModel.EnableUI(true);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        public void CalculateResult(LarvaeAnnotationParameterResult paramResult = null)
        {
            var selectedParameter = paramResult;
            if (paramResult == null)
            {
                if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters == null || LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Count != 1) return;
                selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];
            }

            if (selectedParameter.Parameter.ShapeType.ToLower() == "dot")
            {
                selectedParameter.Result = selectedParameter.Dots.Count();
                if (selectedParameter.Result == 0)
                {
                    selectedParameter.CalculatedResult = null;
                }
                else
                {
                    selectedParameter.CalculatedResult = selectedParameter.Result.ToString();
                }
            }
            else if (selectedParameter.Parameter.ShapeType.ToLower() == "circle")
            {
                var circle = selectedParameter.Circle;
                if (circle == null)
                {
                    selectedParameter.Result = 0;
                    selectedParameter.CalculatedResult = null;
                }
                else
                {
                    var numberPixels = selectedParameter.Circle.Diameter;
                    selectedParameter.Result = (int)numberPixels;
                    var scale = selectedParameter.File.Scale;
                    if (scale == null)
                    {
                        selectedParameter.CalculatedResult = $"{(int)numberPixels} px";
                    }
                    else
                    {
                        decimal resultMm = (decimal)numberPixels / (decimal)scale;
                        if (resultMm > 10) selectedParameter.CalculatedResult = $"{(resultMm / 10).ToString("0.00")} cm";
                        else if (resultMm < 0.1m) selectedParameter.CalculatedResult = $"{(resultMm * 1000).ToString("0")} µm";
                        else selectedParameter.CalculatedResult = $"{(resultMm).ToString("0.00")} mm";
                    }
                }

            }
            else if (selectedParameter.Parameter.ShapeType.ToLower() == "line")
            {
                //if (selectedParameter.File?.Scale == null)
                //{
                //    return;
                //}
                var numberPixels = (int)selectedParameter.Lines.Sum(x => x.Distance());
                selectedParameter.Result = numberPixels;
                if (selectedParameter.Result == 0)
                {
                    selectedParameter.CalculatedResult = null;
                    return;
                }

                //selectedParameter.File = LarvaeViewModel.LarvaeFileViewModel.SelectedFile;
                if (selectedParameter.File?.Scale == null)
                {
                    selectedParameter.CalculatedResult = $"{(int)numberPixels} px";
                }
                else
                {
                    decimal resultMm = (decimal)numberPixels / (decimal)selectedParameter.File.Scale;
                    if (resultMm > 10) selectedParameter.CalculatedResult = $"{(resultMm / 10).ToString("0.00")} cm";
                    else if (resultMm < 0.1m) selectedParameter.CalculatedResult = $"{(resultMm * 1000).ToString("0")} µm";
                    else selectedParameter.CalculatedResult = $"{(resultMm).ToString("0.00")} mm";
                }
            }

            LarvaeViewModel.LarvaeOwnAnnotationView.AnnotationGrid.RefreshData();
        }

        private void DeleteAnnotation(MouseButtonEventArgs e)
        {
            //measures have priority
            var measure = GetClosestMeasure(e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas));
            if (measure.Any())
            {
                foreach (var m in measure)
                {
                    OriginalMeasureShapes.Remove(m);
                }
                
                UndoRedo.InsertInUnDoRedoForDeleteMeasure(measure, this);
                RefreshMeasures();
                return;
            }

            if (LarvaeViewModel.LarvaeSampleViewModel.SelectedSample.IsReadOnly)
            {
                new WinUIMessageBoxService().Show("The selected sample is ReadOnly!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            if (LarvaeViewModel.ActiveTab != "AnnotationOwn")
            {
                new WinUIMessageBoxService().Show("Deleting is only possible when the Own Annotation panel is active.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            if (!LarvaeViewModel.LarvaeOwnAnnotationViewModel.CanEdit) return;

            if (!LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Any())
            {
                return;
            }

            var selectedParameter = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0];

            if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation == null) return;
            if (selectedParameter.Lines.Count == 0 && selectedParameter.Dots.Count == 0 && selectedParameter.Circle == null) return;
            if (e.ChangedButton == MouseButton.Left && LarvaeViewModel.LarvaeEditorView.Cursor == Cursors.Hand)
            {
                if (selectedParameter.Dots.Count > 0)
                {
                    Tuple<LarvaeDot, double> closestDot = new Tuple<LarvaeDot, double>(new LarvaeDot(), 100);

                    var allDots = selectedParameter.Dots;
                    closestDot = CalculateClosestDot(allDots,
                        e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas));
                    if (allDots.Count > 0 && closestDot.Item2 <= 20)
                    {
                        UndoRedo.InsertInUnDoRedoForDeleteDot(closestDot.Item1, this);
                        RemoveDot(closestDot.Item1);
                    }
                }
                else if (selectedParameter.Lines.Count > 0)
                {
                    UndoRedo.InsertInUnDoRedoForDelete(selectedParameter.Lines, this);

                    RemoveLine(selectedParameter.Lines);
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                }
                else if (selectedParameter.Circle != null)
                {
                    UndoRedo.InsertInUnDoRedoForDeleteCircle(selectedParameter.Circle, this);

                    RemoveCircle(selectedParameter.Circle);
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                }

                LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.RequiresSaving = true;
                //RefreshShapes();
            }
            LarvaeViewModel.LarvaeEditorViewModel.UpdateButtons();

            RefreshShapes();
        }

        public List<Line> GetClosestMeasure(Point p)
        {
            List<Line> lines = new List<Line>();
            var canvas = LarvaeViewModel.LarvaeEditorView.MeasureShapesContainer;
            HitTestResult result = VisualTreeHelper.HitTest(canvas, p);
            if (result != null)
            {
                var line = (Line)result.VisualHit;
                var originalLine = OriginalMeasureShapes.FirstOrDefault(x => x.Uid == line.Uid);
                lines.AddRange(OriginalMeasureShapes.Where(x => x.Tag == originalLine.Tag).ToList());
            }

            return lines;
        }

        private Tuple<LarvaeDot, double> CalculateClosestDot(List<LarvaeDot> allDots, Point p)
        {
            if (!allDots.Any()) return null;
            LarvaeDot dot = allDots[0];
            var zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
            var smallestDistance =
                Math.Abs(Math.Pow(p.X / zoomfactor - allDots[0].X, 2) +
                         Math.Pow(p.Y / zoomfactor - allDots[0].Y, 2));
            foreach (LarvaeDot d in allDots)
            {
                var distance =
                    Math.Abs(Math.Pow(p.X / zoomfactor - d.X, 2) +
                             Math.Pow(p.Y / zoomfactor - d.Y, 2));

                if (distance < smallestDistance)
                {
                    dot = d;
                    smallestDistance = distance;
                }
            }
            smallestDistance = Math.Sqrt(smallestDistance);
            return new Tuple<LarvaeDot, double>(dot, smallestDistance);
        }


        public void ParentCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var zoomfactor = LarvaeViewModel.LarvaeStatusbarViewModel.ZoomFactor;
            try
            {
                if (Mode == EditorModeEnum.MakingLine && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Any())
                {
                    var last = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Last();
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                    last.X2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor);
                    last.Y2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor);
                    LineShapes.FirstOrDefault(x => x.Tag.ToString() == last.ID.ToString().ToUpper()).X2 = (int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X;
                    LineShapes.FirstOrDefault(x => x.Tag.ToString() == last.ID.ToString().ToUpper()).Y2 = (int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y;
                }
                else if (Mode == EditorModeEnum.Delete)
                {
                    var measure = GetClosestMeasure(e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas));

                    if (measure.Any())
                    {
                        LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Hand;
                        //if (linepointwithdistance != null && linepointwithdistance.Item2 <= 30)
                        //{
                        //    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Hand;
                        //}
                        //else if (measure != null) 
                        //else
                        //{
                        //    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                        //}
                    }
                    else if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Any() && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Dots.Any())
                    {
                        Tuple<LarvaeDot, double> closestDot = new Tuple<LarvaeDot, double>(new LarvaeDot(), 100);
                        closestDot = CalculateClosestDot(LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Dots, e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas));
                        if (closestDot.Item2 <= 20)
                        {
                            LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                        }
                    }

                    else if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Any() && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Lines.Any())
                    {
                        LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Hand;
                    }
                    else if (LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters.Any() && LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Circle != null)
                    {
                        LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                    }

                    
                }
                else if (Mode == EditorModeEnum.MakingCircle)
                {
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                    var circle = LarvaeViewModel.LarvaeOwnAnnotationViewModel.SelectedParameters[0].Circle;
                    if (circle == null) return;
                    circle.X2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).X / zoomfactor);
                    circle.Y2 = (int)((int)e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas).Y / zoomfactor);

                    RefreshShapes();

                }
                else if (Mode == EditorModeEnum.DrawScale && ScaleShapes.Any())
                {
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                    var position = e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas);
                    var line = ScaleShapes.Last();
                    line.X2 = position.X;
                    //line.Y2 = position.Y; // todo remove?
                    LarvaeViewModel.LarvaeEditorView.ScalePixels.Text =
                        Math.Sqrt(
                            (int)
                            Math.Abs(Math.Pow((line.X2 - line.X1) / zoomfactor, 2) +
                                     Math.Pow((line.Y2 - line.Y1) / zoomfactor, 2))).ToString("N0");
                }
                else if (Mode == EditorModeEnum.MakingMeasure)
                {
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                    var position = e.GetPosition(LarvaeViewModel.LarvaeEditorView.ParentCanvas);
                    var line = originalMeasureShapes.Last();
                    line.X2 = position.X / zoomfactor;
                    line.Y2 = position.Y / zoomfactor;

                    RefreshMeasures(true); //todo option to only refresh last one
                }
                else
                {
                    LarvaeViewModel.LarvaeEditorView.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception ex)
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
        public void DrawCircleBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.DrawCircle;
        }

        public void DeleteBtn_Checked(object sender, RoutedEventArgs e)
        {
            Mode = EditorModeEnum.Delete;
        }

        public void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.Undo();
            UpdateButtons();
        }

        public void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.Redo();
            UpdateButtons();
        }

        public void AutoMeasureScale(bool buttonPressed = false)
        {
            if (!buttonPressed)
            {
                if (MeasuredFileIDs.Contains(LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID)) return;
                else
                {
                    MeasuredFileIDs.Add(LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID);
                }
            }
            if (LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Path != null)
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
            RefreshMeasures();
        }

        public void MeasureTool()
        {
            Mode = Mode == EditorModeEnum.Measure ? EditorModeEnum.None : EditorModeEnum.Measure;
        }

        public void ManualMeasureScale()
        {
            Mode = EditorModeEnum.DrawScale;
            LarvaeViewModel.EnableUI(false);
            LarvaeViewModel.LarvaeEditorView.MeasureScalePanel.Visibility = Visibility.Visible;
        }

        public void DeleteMeasureScale()
        {
            var dtofile = (DtoLarvaeEggFile)Helper.ConvertType(LarvaeViewModel.LarvaeFileViewModel.SelectedFile, typeof(DtoLarvaeEggFile));
            dtofile.Scale = null;
            var deleteResult = Global.API.UpdateLarvaeFile(LarvaeViewModel.LarvaeAnalysis.Type, dtofile);
            if (!deleteResult.Succeeded)
            {
                Helper.ShowWinUIMessageBox(deleteResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale = null;
                LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): ?";
            }

            foreach (var lapr in LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Where(x => x.FileID == LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID))
            {
                CalculateResult(lapr);
            }

        }

        public void ClearScaleLine()
        {
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            LarvaeViewModel.LarvaeEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            Mode = EditorModeEnum.DrawScale;
        }

        public void AcceptScale()
        {
            double pixels;
            double.TryParse(LarvaeViewModel.LarvaeEditorView.ScalePixels.Text.Replace(",", "").Replace(".", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out pixels);
            double unitLength;
            double.TryParse(LarvaeViewModel.LarvaeEditorView.ScaleMilimeters.Text.Replace(",", "").Replace(".", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out unitLength);

            double milimeters = unitLength;
            if (MeasureUnit == "cm")
            {
                milimeters *= 10;
            }
            else if (MeasureUnit == "µm")
            {
                milimeters /= 1000;
            }

            PixelLength = (decimal)(pixels / milimeters);
            var oldvalue = LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale;
            var newvalue = PixelLength;
            if (oldvalue != newvalue)
            {
                LarvaeViewModel.LarvaeFileViewModel.SelectedFile.Scale = PixelLength;
                if (PixelLength == 0)
                {
                    LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): ?";
                }
                else
                {
                    LarvaeViewModel.LarvaeStatusbarViewModel.Info = $"Scale (px/mm): {PixelLength}";
                }
                
                ((dynamic)LarvaeViewModel.LarvaeFileView.FileList.FocusedRowData.Row).Scale = PixelLength;
                LarvaeViewModel.LarvaeFileView.LarvaeFileGrid.RefreshData();
                LarvaeViewModel.LarvaeEditorView.ScaleButton.IsEnabled = true;
                DtoLarvaeEggFile dtofile = (DtoLarvaeEggFile)Helper.ConvertType(LarvaeViewModel.LarvaeFileViewModel.SelectedFile, typeof(DtoLarvaeEggFile));
                var updateFileResult = Global.API.UpdateLarvaeFile(LarvaeViewModel.LarvaeAnalysis.Type, dtofile);
                if (!updateFileResult.Succeeded)
                {
                    Helper.ShowWinUIMessageBox("Error saving File to Web API\n" + updateFileResult.ErrorMessage, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            LarvaeViewModel.LarvaeEditorView.ScalePixels.Text = "0";
            HideMeasureScalePanel();
            foreach (var lapr in LarvaeViewModel.LarvaeOwnAnnotationViewModel.Annotation.LarvaeAnnotationParameterResult.Where(x => x.FileID == LarvaeViewModel.LarvaeFileViewModel.SelectedFile.ID))
            {
                CalculateResult(lapr);
            }
            RefreshShapes();
            Mode = EditorModeEnum.None;
        }

        public void HideMeasureScalePanel()
        {
            ScaleShapes.Clear();
            LarvaeViewModel.LarvaeEditorView.MeasureScalePanel.Visibility = Visibility.Collapsed;
            LarvaeViewModel.LarvaeEditorView.ScalePixels.Text = "0";
            LarvaeViewModel.LarvaeEditorView.ScaleMilimeters.EditValue = 1;
            MeasureUnit = "mm";
        }

        public void CancelScale()
        {
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            LarvaeViewModel.LarvaeEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            HideMeasureScalePanel();
            Mode = EditorModeEnum.None;
        }

        public void CancelMeasure()
        {
            MeasureShapes.Clear();
            IsMeasureDrawn = false;
            RefreshShapes();
            Mode = EditorModeEnum.Measure;
            LarvaeViewModel.EnableUI(true);

        }

        public void bReset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Brightness = 0;
        }
        public void cReset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Contrast = 0;
        }
    }
}
