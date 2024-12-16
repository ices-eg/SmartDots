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
using System.Windows.Threading;
using DevExpress.Data.Helpers;
using DevExpress.Internal.WinApi;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Helpers;
using SmartDots.Model;
using SmartDots.View;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Line = System.Windows.Shapes.Line;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;

namespace SmartDots.ViewModel
{
    public class AgeReadingEditorViewModel : AgeReadingBaseViewModel
    {
        private object lineColor;
        private object measureColor;
        private object dotColor;
        private int lineWidth;
        private int measureLineWidth;
        private int measureFontSize;
        private int dotWidth;
        private List<Dot> originalDots = new List<Dot>();
        private ObservableCollection<Line> lineShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> topLevelLineShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> scaleShapes = new ObservableCollection<Line>();
        private ObservableCollection<Shape> dotShapes = new ObservableCollection<Shape>();
        private ObservableCollection<Line> originalMeasureShapes = new ObservableCollection<Line>();
        private ObservableCollection<Line> measureShapes = new ObservableCollection<Line>();
        private ObservableCollection<TextBlock> textShapes = new ObservableCollection<TextBlock>();
        private CombinedLine activeCombinedLine;
        private CombinedLine tempCombinedLine;
        private EditorModeEnum mode;
        private Image tracker;
        private Bitmap originalImage;
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
        private bool hideLines;
        private bool isScaleDrawn;
        private bool isMeasureDrawn;
        private string measureUnit = "mm";
        private UndoRedo undoRedo;
        private string dotType = "Seawater";
        private string dotShape = "Dot";
        private Tuple<Dot, double> closestDot;
        private Dictionary<int, Tuple<Action, string, string>> shortcutActions = new Dictionary<int, Tuple<Action, string, string>>();

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

        public object DotWidth
        {
            get { return dotWidth; }
            set
            {
                dotWidth = int.Parse(value.ToString());
                RaisePropertyChanged("DotWidth");
            }
        }

        public List<Tuple<string, string>> DotTypes { get; set; } = new List<Tuple<string, string>>() { new Tuple<string, string>("Seawater", "Seawater"), new Tuple<string, string>("Freshwater", "Freshwater/Absorbed"), new Tuple<string, string>("Non-counting mark", "Non-counting mark") };

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

        public ObservableCollection<Line> TopLevelLineShapes
        {
            get { return topLevelLineShapes; }
            set
            {
                topLevelLineShapes = value;
                RaisePropertyChanged("TopLevelLineShapes");
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

        public ObservableCollection<Line> OriginalMeasureShapes
        {
            get { return originalMeasureShapes; }
            set
            {
                originalMeasureShapes = value;
            }
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

        public CombinedLine ActiveCombinedLine
        {
            get { return activeCombinedLine; }
            set
            {
                activeCombinedLine = value;
                activeCombinedLine?.CalculateDotIndices();
                RaisePropertyChanged("ActiveCombinedLine");
                AgeReadingViewModel.AgeReadingView.BrightnessGraph.graphViewer.SetAnnotations(AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations);
                AgeReadingViewModel.AgeReadingView.RednessGraph.graphViewer.SetAnnotations(AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations);
                //AgeReadingViewModel.AgeReadingView.GrowthGraph.graphViewer.SetAnnotations(AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations);
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
                    AgeReadingViewModel.AgeReadingEditorView.MeasureButton.IsPressed = false;

                    switch (value)
                    {
                        case EditorModeEnum.DrawLine:
                            AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = true;
                            HideLines = false;
                            break;
                        case EditorModeEnum.MakingLine:
                            HideLines = false;
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
                        case EditorModeEnum.Measure:
                            AgeReadingViewModel.AgeReadingEditorView.MeasureButton.IsPressed = true;
                            HideLines = false;
                            break;
                        case EditorModeEnum.None:
                            AgeReadingViewModel.AgeReadingEditorView.LineButton.IsPressed = false;
                            AgeReadingViewModel.AgeReadingEditorView.DotButton.IsPressed = false;
                            AgeReadingViewModel.AgeReadingEditorView.DeleteBtn.IsChecked = false;
                            AgeReadingViewModel.AgeReadingEditorView.MeasureButton.IsPressed = false;
                            AgeReadingViewModel.EnableUI(true);
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

        public List<Tuple<Annotation, List<decimal>>> LineBrightness
        {
            get
            {
                var img = BitmapConverter.BitmapImage2Bitmap(OtolithImage);
                List<Tuple<Annotation, List<decimal>>> values = new List<Tuple<Annotation, List<decimal>>>();
                foreach (var annotation in AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations)
                {
                    if (annotation.CombinedLines.Any())
                    {
                        values.Add(new Tuple<Annotation, List<decimal>>(annotation, annotation.CombinedLines.FirstOrDefault().GetLineBrightness(img)));
                    }
                }
                return values;
            }
        }

        public List<Tuple<Annotation, List<decimal>>> LineRedness
        {
            get
            {
                var img = BitmapConverter.BitmapImage2Bitmap(OtolithImage);
                List<Tuple<Annotation, List<decimal>>> values = new List<Tuple<Annotation, List<decimal>>>();
                foreach (var annotation in AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations)
                {
                    if (annotation.CombinedLines.Any())
                    {
                        values.Add(new Tuple<Annotation, List<decimal>>(annotation, annotation.CombinedLines.FirstOrDefault().GetLineRedness(img)));
                    }
                }
                return values;
            }
        }

        public List<Tuple<Annotation, List<decimal>>> LineGrowth
        {
            get
            {
                List<Tuple<Annotation, List<decimal>>> values = new List<Tuple<Annotation, List<decimal>>>();
                foreach (var annotation in AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations)
                {
                    if (annotation.CombinedLines.Any())
                    {
                        values.Add(new Tuple<Annotation, List<decimal>>(annotation, annotation.CombinedLines.FirstOrDefault().CalculateLineGrowth()));
                    }
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

        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set
            {
                originalImage = value;
                if (Global.API.Settings.AutoMeasureScale && (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale == null || AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale == 0.0m))
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
                       && AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile != null
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        Global.API.Settings.AnnotateWithoutSample);
            }
        }

        public bool CanDrawDot
        {
            get
            {
                return AgeReadingViewModel?.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count == 1
                       && ActiveCombinedLine != null
                       && Mode != EditorModeEnum.MakingLine
                       && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null
                       && !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
                       && AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile != null
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        Global.API.Settings.AnnotateWithoutSample);
            }
        }

        public bool CanDelete
        {
            get
            {
                return
                       MeasureShapes.Any() ||
                       (AgeReadingViewModel?.AgeReadingAnnotationViewModel?.SelectedAnnotations.Count < 2
                       && ActiveCombinedLine != null
                       && Mode != EditorModeEnum.MakingLine
                       && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null
                       && !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsFixed
                       && AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile != null
                       && !AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly
                       &&
                       (AgeReadingViewModel?.AgeReadingSampleViewModel.Sample != null ||
                        Global.API.Settings.AnnotateWithoutSample));
            }
        }

        public bool IsScaleDrawn
        {
            get { return isScaleDrawn; }
            set
            {
                isScaleDrawn = value;
                //if (isScaleDrawn)
                //{
                //    AgeReadingViewModel.AgeReadingEditorView.ScaleDrawn.Visibility = Visibility.Visible;
                //    AgeReadingViewModel.AgeReadingEditorView.ScaleNotDrawn.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    AgeReadingViewModel.AgeReadingEditorView.ScaleDrawn.Visibility = Visibility.Collapsed;
                //    AgeReadingViewModel.AgeReadingEditorView.ScaleNotDrawn.Visibility = Visibility.Visible;
                //}
                RaisePropertyChanged("IsScaleDrawn");
            }
        }

        public ObservableCollection<string> Units { get; set; } = new ObservableCollection<string>
            { "µm", "mm", "cm" };

        public string MeasureUnit
        {
            get
            {
                return measureUnit;
            }
            set
            {
                measureUnit = value;
                RaisePropertyChanged("MeasureUnit");
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

        public bool IsContextmenuVisible
        {
            get
            {
                if (ClosestDot != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsContextmenuOpen { get; set; }


        public System.Drawing.Rectangle ScaleRectangle { get; private set; }
        public bool IsMeasuring { get; private set; }
        public List<Guid> MeasuredFileIDs { get; set; } = new List<Guid>();

        public bool HideLines
        {
            get { return hideLines; }
            set
            {
                if (hideLines != value) RefreshShapes();
                hideLines = value;
                RaisePropertyChanged("HideLines");
            }
        }

        public Tuple<Dot, double> ClosestDot
        {
            get { return closestDot; }
            set
            {
                closestDot = value;
                RaisePropertyChanged("ClosestDotWidth");
                RaisePropertyChanged("ClosestDotColor");
                RaisePropertyChanged("ClosestDotDotShape");
                RaisePropertyChanged("ClosestDotDotType");
            }
        }

        public object ClosestDotWidth
        {
            get
            {
                if (ClosestDot == null) return 0;
                return closestDot.Item1.Width;
            }
            set
            {
                ClosestDot.Item1.Width = int.Parse(value.ToString());
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
                    .Width = int.Parse(value.ToString());
                RaisePropertyChanged("ClosestDotWidth");
                RefreshShapes();
            }
        }

        public object ClosestDotColor
        {
            get
            {
                if (ClosestDot == null) return "#000000";
                return (Color)ColorConverter.ConvertFromString(closestDot.Item1.Color);
            }
            set
            {
                ClosestDot.Item1.Color = value.ToString();
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
                    .Color = value.ToString();
                RaisePropertyChanged("ClosestDotColor");
                RefreshShapes();
            }
        }

        public string ClosestDotDotShape
        {
            get
            {
                if (ClosestDot == null) return "Dot";
                return closestDot.Item1.DotShape;
            }
            set
            {
                ClosestDot.Item1.DotShape = value;
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
                    .DotShape = value;
                RaisePropertyChanged("ClosestDotDotShape");
                RefreshShapes();
            }
        }

        public string ClosestDotDotType
        {
            get
            {
                if (ClosestDot == null) return "Seawater";
                return closestDot.Item1.DotType;
            }
            set
            {
                ClosestDot.Item1.DotType = value;
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
                    .DotType = value;
                RaisePropertyChanged("ClosestDotDotType");
                RefreshShapes();
            }
        }

        public Dictionary<int, Tuple<Action, string, string>> ShortcutActions { get => shortcutActions; set => shortcutActions = value; }

        public AgeReadingEditorViewModel()
        {
            Mode = EditorModeEnum.DrawLine;
            Brightness = 0;
            Contrast = 0;
            LineColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
            MeasureColor = (Color)ColorConverter.ConvertFromString("#FFFF00FF");
            LineWidth = 2;
            MeasureLineWidth = 1;
            MeasureFontSize = 12;
            DotColor = (Color)ColorConverter.ConvertFromString("#FF00FF00");
            DotWidth = 10;
            UndoRedo = new UndoRedo(this);
            InitializeShortcuts();
        }

        public void CalculateAge()
        {
            var annotation = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation;
            if (annotation == null) return;
            if (annotation.HasAq3())
            {
                AgeReadingViewModel.AgeReadingAnnotationViewModel.SetAge(null);
                return;
            }
            int age = 0;
            foreach (
                CombinedLine combinedLine in
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines)
            {
                if (combinedLine.Dots.Count > age)
                {
                    age = combinedLine.Dots.Count(x => x.DotType != "Non-counting mark");
                }
            }
            AgeReadingViewModel.AgeReadingAnnotationViewModel.SetAge(age);
        }

        public async void GetPixelLength()
        {
            try
            {
                var selectedFileId = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID;
                var oldvalue = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale;
                AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = false;

                decimal temp = await Task.Run(() => ScaleMeasureTool.Measure(OriginalImage, 128));

                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID != selectedFileId)
                {
                    AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
                    return;
                }

                PixelLength = temp;

                if (PixelLength == 0) throw new Exception("Could not determine the scale");

                var newvalue = PixelLength;
                if (oldvalue != newvalue)
                {
                    AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale = PixelLength;
                    ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).Scale = PixelLength;
                    AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                    AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
                    DtoFile dtofile =
                        (DtoFile)
                        Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
                    var updateFileResult = Global.API.UpdateFile(dtofile);
                    if (!updateFileResult.Succeeded)
                    {
                        Helper.ShowWinUIMessageBox("Error saving File to Web API\n" + updateFileResult.ErrorMessage, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = false;
                        return;
                    }
                    AgeReadingViewModel.UpdateGraphs(false, false, false, true);
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



        public void UpdateButtons()
        {
            RaisePropertyChanged("CanDrawLine");
            RaisePropertyChanged("CanDrawDot");
            RaisePropertyChanged("CanDelete");
            RaisePropertyChanged("ContextmenuVisibility");
            AgeReadingViewModel.AgeReadingEditorView.LineButton.UpdateButtonBackground(
                AgeReadingViewModel.AgeReadingEditorView.LineButton);
            AgeReadingViewModel.AgeReadingEditorView.DotButton.UpdateButtonBackground(
                AgeReadingViewModel.AgeReadingEditorView.DotButton);
        }



        public void RefreshShapes(bool updategraphs = true)
        {
            AgeReadingViewModel.AgeReadingEditorView.Dispatcher.Invoke(() =>
            {
                LineShapes = new ObservableCollection<Line>();
                TopLevelLineShapes = new ObservableCollection<Line>();
                DotShapes = new ObservableCollection<Shape>();
                float zoomfactor = AgeReadingViewModel == null
                    ? 0
                    : AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
                ObservableCollection<Line> lines = new ObservableCollection<Line>();
                ObservableCollection<Line> topLevelLines = new ObservableCollection<Line>();
                ObservableCollection<Shape> dots = new ObservableCollection<Shape>();

                foreach (var annotation in AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations)
                {
                    foreach (CombinedLine cl in annotation.CombinedLines)
                    {
                        foreach (Model.Line l in cl.Lines)
                        {
                            Line line = new Line()
                            {
                                X1 = (float)l.X1 * zoomfactor,
                                Y1 = (float)l.Y1 * zoomfactor,
                                X2 = (float)l.X2 * zoomfactor,
                                Y2 = (float)l.Y2 * zoomfactor,
                                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(l.Color)),
                                StrokeThickness = (float)l.Width * zoomfactor,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round

                            };
                            lines.Add(line);
                        }

                        if (!annotation.HasAq3())
                        {
                            foreach (Dot d in cl.Dots)
                            {
                                if (d.DotType == "Non-counting mark")
                                {
                                    var radius = (float)d.Width;
                                    var dotWidth = ((radius) * zoomfactor);
                                    Shape l = new Ellipse()
                                    {
                                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000")),
                                        Width = dotWidth,
                                        Height = dotWidth, // not an error, they are the same

                                    };
                                    var left = (float)d.X * zoomfactor - (radius) / 2 * zoomfactor;
                                    var top = (float)d.Y * zoomfactor - (radius) / 2 * zoomfactor;
                                    Canvas.SetLeft(l, left);
                                    Canvas.SetTop(l, top);
                                    dots.Add(l);

                                    var line = new Line()
                                    {
                                        X1 = (float)(left + (dotWidth / 3)),
                                        Y1 = (float)(top + (dotWidth / 3)),
                                        X2 = (float)(left + (dotWidth / 3) * 2),
                                        Y2 = (float)(top + (dotWidth / 3) * 2),
                                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF")),
                                        StrokeThickness = dotWidth / 8,
                                        StrokeStartLineCap = PenLineCap.Square,
                                        StrokeEndLineCap = PenLineCap.Square,
                                        Tag = "Non-counting mark"
                                    };

                                    var line2 = new Line()
                                    {
                                        X1 = (float)(left + (dotWidth / 3)),
                                        Y1 = (float)(top + (dotWidth / 3) * 2),
                                        X2 = (float)(left + (dotWidth / 3) * 2),
                                        Y2 = (float)(top + (dotWidth / 3)),
                                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF")),
                                        StrokeThickness = dotWidth / 8,
                                        StrokeStartLineCap = PenLineCap.Square,
                                        StrokeEndLineCap = PenLineCap.Square,
                                        Tag = "Non-counting mark"
                                    };

                                    topLevelLines.Add(line);
                                    topLevelLines.Add(line2);

                                }
                                else
                                {
                                    bool isOddDot = ((float)(annotation.CombinedLines.SelectMany(x => x.Dots).Count(x => x.Color == d.Color)) / (float)annotation.CombinedLines.SelectMany(x => x.Dots).Count()) < 0.34f;
                                    SolidColorBrush brush;
                                    if (AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations.Count == 1 || isOddDot || annotation.MultiUserColor == null) brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(d.Color));
                                    else brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(annotation.MultiUserColor));

                                    if (d.DotShape == "Cross")
                                    {
                                        Shape c = new Cross()
                                        {
                                            Fill = brush,
                                            Width = (float)d.Width * zoomfactor,
                                            X = (float)d.X * zoomfactor,
                                            Y = (float)d.Y * zoomfactor,
                                        };
                                        var line1 = ((Cross)c).Line1;
                                        line1.StrokeStartLineCap = PenLineCap.Round;
                                        line1.StrokeEndLineCap = PenLineCap.Round;
                                        line1.Tag = "Cross";

                                        var line2 = ((Cross)c).Line2;
                                        line2.StrokeStartLineCap = PenLineCap.Round;
                                        line2.StrokeEndLineCap = PenLineCap.Round;
                                        line2.Tag = "Cross";

                                        lines.Add(line1);
                                        lines.Add(line2);
                                    }
                                    else
                                    {
                                        Shape l = new Ellipse()
                                        {
                                            Fill = brush,
                                            Width = (float)d.Width * zoomfactor,
                                            Height = (float)d.Width * zoomfactor,
                                            SnapsToDevicePixels = false

                                        };
                                        var left = (float)d.X * zoomfactor - (float)d.Width / 2 * zoomfactor;
                                        var top = (float)d.Y * zoomfactor - (float)d.Width / 2 * zoomfactor;
                                        Canvas.SetLeft(l, left);
                                        Canvas.SetTop(l, top);
                                        dots.Add(l);
                                    }
                                    if (d.DotType == "Freshwater")
                                    {
                                        var radius = (float)d.Width * 1.8;
                                        Shape l = new Ellipse()
                                        {
                                            Stroke = brush,
                                            Width = ((radius) * zoomfactor),
                                            Height = ((radius) * zoomfactor),
                                            StrokeThickness = (float)d.Width / 8 * zoomfactor,
                                            StrokeDashArray = new DoubleCollection() { 3, 3 }

                                        };
                                        var left = (float)d.X * zoomfactor - (radius) / 2 * zoomfactor;
                                        var top = (float)d.Y * zoomfactor - (radius) / 2 * zoomfactor;
                                        Canvas.SetLeft(l, left);
                                        Canvas.SetTop(l, top);
                                        dots.Add(l);
                                    }
                                }
                            }

                        }

                        if (Mode != EditorModeEnum.MakingLine && cl.Lines.Any())
                        {
                            var lastLine = cl.Lines.OrderBy(x => x.LineIndex).Last();
                            var n = Math.Atan2(lastLine.Y1 - lastLine.Y2, lastLine.X2 - lastLine.X1) * 180 / Math.PI;
                            Point point1 = new Point(lastLine.X2, lastLine.Y2);
                            var cos = Math.Cos((n + 30) * (Math.PI / 180.0)) * 10;
                            var sin = Math.Sin((n + 30) * (Math.PI / 180.0)) * 10;
                            Point point2 = new Point(lastLine.X2 - cos, lastLine.Y2 + sin);
                            cos = Math.Cos((n - 30) * (Math.PI / 180.0)) * 10;
                            sin = Math.Sin((n - 30) * (Math.PI / 180.0)) * 10;
                            Point point3 = new Point(lastLine.X2 - cos, lastLine.Y2 + sin);
                            var line1 = new Line()
                            {
                                X1 = (float)point1.X * zoomfactor,
                                Y1 = (float)point1.Y * zoomfactor,
                                X2 = (float)point2.X * zoomfactor,
                                Y2 = (float)point2.Y * zoomfactor,
                                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(lastLine.Color)),
                                StrokeThickness = (lastLine.Width * zoomfactor),
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };
                            var line2 = new Line()
                            {
                                X1 = (float)point1.X * zoomfactor,
                                Y1 = (float)point1.Y * zoomfactor,
                                X2 = (float)point3.X * zoomfactor,
                                Y2 = (float)point3.Y * zoomfactor,
                                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(lastLine.Color)),
                                StrokeThickness = (lastLine.Width * zoomfactor),
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };
                            lines.Add(line1);
                            lines.Add(line2);
                        }

                    }

                }
                RefreshMeasures();

                if (HideLines)
                {
                    foreach (var line in lines.Where(x => x.Tag == null))
                    {
                        line.StrokeThickness = 0;
                    }
                }

                LineShapes = lines;
                TopLevelLineShapes = topLevelLines;
                DotShapes = dots;
                ScaleShapes = ScaleShapes;

                if (updategraphs) AgeReadingViewModel.UpdateGraphs();
                CalculateAge();
            });
        }

        public void RefreshMeasures(bool onlyLast = false)
        {
            if (HideLines)
            {
                MeasureShapes = new ObservableCollection<Line>();
                TextShapes = new ObservableCollection<TextBlock>();
                return;
            }

            float zoomfactor = AgeReadingViewModel == null
                ? 0
                : AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
            ObservableCollection<Line> measures = new ObservableCollection<Line>();
            ObservableCollection<TextBlock> text = new ObservableCollection<TextBlock>();

            foreach (var line in originalMeasureShapes)
            {
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
                var tb = new TextBlock()
                {
                    Text = "Placeholder",
                    Foreground = l.Stroke,
                    FontSize = int.Parse(l.Tag.ToString()) * zoomfactor,
                };

                var scale = AgeReadingViewModel?.AgeReadingFileViewModel.SelectedFile.Scale;

                if (scale != null && scale != 0)
                {
                    var lengthText = "";
                    var length = (decimal)Math.Sqrt((int)Math.Abs(Math.Pow((line.X2 - line.X1), 2) + Math.Pow((line.Y2 - line.Y1), 2))) / scale;

                    if (length < 0.001m)
                    {
                        var tempLength = length * 1000000;
                        tempLength = decimal.Round((decimal)tempLength, 0);
                        lengthText = tempLength.ToString() + " nm";
                    }
                    else if (length < 1m)
                    {
                        var tempLength = length * 1000;
                        tempLength = decimal.Round((decimal)tempLength, 1);
                        lengthText = tempLength.ToString() + " µm";
                    }
                    else
                    {
                        length = decimal.Round((decimal)length, 2);
                        lengthText = length.ToString() + " mm";
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
                        .GetClosestPointWithDistance(new Point(p.X / zoomfactor, p.Y / zoomfactor));
            }

            Tuple<LinePoint, double> point = null;
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Count > 1)
            {
                foreach (
                    CombinedLine l in AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines)
                {
                    if (l.Points.Count > 0)
                        point = l.GetClosestPointWithDistance(new Point(p.X / zoomfactor, p.Y / zoomfactor));

                    if (closestPoint != null && (point != null && point.Item2 < closestPoint.Item2))
                    {
                        closestPoint = point;
                    }
                }
            }
            return closestPoint;
        }

        public Line GetClosesMeasure(Point p)
        {
            var canvas = AgeReadingViewModel.AgeReadingEditorView.MeasureShapesContainer;
            HitTestResult result = VisualTreeHelper.HitTest(canvas, p);
            if (result != null)
            {
                var line = (Line)result.VisualHit;
                var originalLine = OriginalMeasureShapes.FirstOrDefault(x => x.Uid == line.Uid);
                return originalLine;
            }

            return null;
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
                    if (Brightness == 0 && Contrast == 0)
                    {
                        OtolithImage = BitmapConverter.Bitmap2BitmapImage(OriginalImage);
                    }
                    else
                    {
                        OtolithImage = ImageManipulator.SetBrightnessContrast(new Bitmap(OriginalImage), Brightness, Contrast);
                    }
                    AgeReadingViewModel.UpdateGraphs(true, true, false, false);
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

            if (IsContextmenuOpen) return;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly && !(Mode == EditorModeEnum.DrawScale || Mode == EditorModeEnum.Measure || Mode == EditorModeEnum.MakingMeasure))
                {
                    new WinUIMessageBoxService().Show("The selected file is ReadOnly!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsReadOnly
                    && !(Mode == EditorModeEnum.DrawScale || Mode == EditorModeEnum.Measure || Mode == EditorModeEnum.MakingMeasure))
                {
                    new WinUIMessageBoxService().Show("The selected annotation is ReadOnly!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (Mode != EditorModeEnum.Measure && Mode != EditorModeEnum.MakingMeasure && Mode != EditorModeEnum.Delete)
                {
                    if (AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations.Count > 1)
                    {
                        new WinUIMessageBoxService().Show("Can not draw any shapes when multiple annotations are selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
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
                }

                ClosestDot = null;

                if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null
                    && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any()
                    && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.Any())
                {
                    var allDots = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots;
                    var closestDot = CalculateClosestDot(allDots, e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                    if (closestDot.Item2 <= 20) ClosestDot = closestDot;
                }
            }
            else
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
            float zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
            var position = e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas);
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
                Tag = MeasureFontSize.ToString()

            };
            OriginalMeasureShapes.Add(line);
            UndoRedo.InsertInUnDoRedoForAddMeasure(line, this);
            Mode = EditorModeEnum.MakingMeasure;
            AgeReadingViewModel.EnableUI(false);

            //}
            //else
            //{
            //    Mode = EditorModeEnum.None;
            //}
            RefreshShapes();
        }

        private void MakingMeasure(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right) OriginalMeasureShapes.Remove(OriginalMeasureShapes.Last());
            CancelMeasure();
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
                X1 = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X / zoomfactor),
                X2 = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X / zoomfactor),
                Y1 = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y / zoomfactor),
                Y2 = (int)((int)e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y / zoomfactor),
                Width = (int)LineWidth,
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
                        = (int)(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).X / zoomfactor);
                    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last()
                            .Lines.Last()
                            .Y2
                        = (int)(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas).Y / zoomfactor);

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
                        Width = (int)LineWidth,
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
                    if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Last().Lines.Any())
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
            if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.HasAq3()) return;
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
                            Width = (int)DotWidth,
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
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
        }

        private void DeleteLineOrDot(MouseButtonEventArgs e)
        {
            //measures have priority
            var measure = GetClosesMeasure(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
            if (measure != null)
            {
                OriginalMeasureShapes.Remove(measure);
                UndoRedo.InsertInUnDoRedoForDeleteMeasure(measure, this);
                RefreshMeasures();
                return;
            }

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
            AgeReadingViewModel.AgeReadingAnnotationViewModel.RefreshActions();
        }

        private Tuple<Dot, double> CalculateClosestDot(List<Dot> allDots, Point p)
        {
            if (!allDots.Any()) return null;
            Dot dot = allDots[0];
            var smallestDistance =
                Math.Abs(Math.Pow(p.X / AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - allDots[0].X, 2) +
                         Math.Pow(p.Y / AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - allDots[0].Y, 2));
            foreach (Dot d in allDots)
            {
                var distance =
                    Math.Abs(Math.Pow(p.X / AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - d.X, 2) +
                             Math.Pow(p.Y / AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - d.Y, 2));

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
            var zoomfactor = AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
            try
            {
                //if (AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation != null &&
                //    !AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.HasAq3() &&
                //AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                //{
                Tuple<Dot, double> closestDot = new Tuple<Dot, double>(new Dot(), 100);

                //Dichtste punt wordt berekend
                //var allDots = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots;
                var allDots = AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations
                    .Where(x => !x.HasAq3() && x.CombinedLines.Any())
                    .SelectMany(x => x.CombinedLines[0].Dots).ToList();
                closestDot = CalculateClosestDot(allDots,
                        e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                if (allDots.Count > 0 && closestDot.Item2 <= 20)
                {
                    AgeReadingViewModel.AgeReadingStatusbarViewModel.Info =
                        closestDot.Item1.DotType == "Non-counting mark" ? "Non-counting mark" : $"Age {closestDot.Item1.DotIndex} ({closestDot.Item1.DotType})";
                    var annotation = AgeReadingViewModel.AgeReadingAnnotationViewModel.SelectedAnnotations.FirstOrDefault(x => x.ID == closestDot.Item1.AnnotationID);
                    AgeReadingViewModel.AgeReadingStatusbarViewModel.Info += $" - {annotation.LabTechnician}";
                }
                else
                {
                    AgeReadingViewModel.AgeReadingStatusbarViewModel.Info = "";
                }
                //}

                if (Mode == EditorModeEnum.MakingLine && AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Any())
                {
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
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
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;





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
                                AgeReadingViewModel.UpdateGraphs(true, true, false, false);
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
                else if (Mode == EditorModeEnum.Delete || Mode == EditorModeEnum.SelectLine)
                {
                    Tuple<LinePoint, double> linepointwithdistance = GetClosestLinePointWithDistance(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));
                    Line measure = GetClosesMeasure(e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas));

                    if (linepointwithdistance != null && linepointwithdistance.Item2 <= 30)
                    {
                        AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Hand;
                    }
                    else if (measure != null) AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Hand;
                    else
                    {
                        AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                    }
                }
                else if (Mode == EditorModeEnum.DrawScale && ScaleShapes.Any())
                {
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                    var position = e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas);
                    var line = ScaleShapes.Last();
                    line.X2 = position.X;
                    AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text =
                        Math.Sqrt(
                            (int)
                            Math.Abs(Math.Pow((line.X2 - line.X1) / zoomfactor, 2) +
                                     Math.Pow((line.Y2 - line.Y1) / zoomfactor, 2))).ToString("N0");
                }
                else if (Mode == EditorModeEnum.MakingMeasure)
                {
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
                    var position = e.GetPosition(AgeReadingViewModel.AgeReadingEditorView.ParentCanvas);
                    var line = originalMeasureShapes.Last();
                    line.X2 = position.X / zoomfactor;
                    line.Y2 = position.Y / zoomfactor;

                    RefreshMeasures(true); //todo option to only refresh last one
                }
                else
                {
                    AgeReadingViewModel.AgeReadingEditorView.Cursor = Cursors.Arrow;
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
                if (MeasuredFileIDs.Contains(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.ID)) return;
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
            RefreshMeasures();
        }

        public void MeasureTool()
        {
            Mode = Mode == EditorModeEnum.Measure ? EditorModeEnum.None : EditorModeEnum.Measure;
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
            ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).Scale = null;
            AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
            var dtofile = (DtoFile)Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
            var deleteResult = Global.API.UpdateFile(dtofile);
            if (!deleteResult.Succeeded)
                Helper.ShowWinUIMessageBox(deleteResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            AgeReadingViewModel.UpdateGraphs(false, false, false, true);
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
            double.TryParse(AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out pixels);
            double unitLength;
            double.TryParse(AgeReadingViewModel.AgeReadingEditorView.ScaleMilimeters.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out unitLength);

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
            var oldvalue = AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale;
            var newvalue = PixelLength;
            if (oldvalue != newvalue)
            {
                AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.Scale = PixelLength;
                ((dynamic)AgeReadingViewModel.AgeReadingFileView.FileList.FocusedRowData.Row).Scale = PixelLength;
                AgeReadingViewModel.AgeReadingFileView.FileGrid.RefreshData();
                AgeReadingViewModel.AgeReadingEditorView.ScaleButton.IsEnabled = true;
                DtoFile dtofile = (DtoFile)Helper.ConvertType(AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile, typeof(DtoFile));
                var updateFileResult = Global.API.UpdateFile(dtofile);
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
            HideMeasureScalePanel();
            RefreshShapes();
            AgeReadingViewModel.UpdateGraphs(false, false, false, true);
            Mode = EditorModeEnum.None;
        }

        public void HideMeasureScalePanel()
        {
            ScaleShapes.Clear();
            AgeReadingViewModel.AgeReadingEditorView.MeasureScalePanel.Visibility = Visibility.Collapsed;
            AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text = "0";
            AgeReadingViewModel.AgeReadingEditorView.ScaleMilimeters.EditValue = 1;
            MeasureUnit = "mm";

        }

        public void CancelScale()
        {
            ScaleShapes.Clear();
            IsScaleDrawn = false;
            AgeReadingViewModel.AgeReadingEditorView.ScalePixels.Text = "0";
            RefreshShapes();
            HideMeasureScalePanel();
            Mode = EditorModeEnum.None;
        }

        public void CancelMeasure()
        {
            MeasureShapes.Clear();
            IsMeasureDrawn = false;
            RefreshShapes();
            Mode = EditorModeEnum.None;
            AgeReadingViewModel.EnableUI(true);

        }

        public void DeleteClosestDot(object sender, RoutedEventArgs e)
        {
            if (ClosestDot == null) return;
            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.Remove(
                AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
                );
            UndoRedo.InsertInUnDoRedoForDeleteDot(ClosestDot.Item1, this);

            //AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.Remove(
            //    AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines[0].Dots.FirstOrDefault(x => x.Location == ClosestDot.Item1.Location)
            //    );
            ClosestDot = null;
            RefreshShapes();
        }

        public void Graphs_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.ToggleGraphs();
        }

        public void KeyActions_Click(object sender, RoutedEventArgs e)
        {
            AgeReadingViewModel.AgeReadingKeyMappingView = new AgeReadingKeyMappingView(AgeReadingViewModel);
            bool hasQa = AgeReadingViewModel.AgeReadingAnnotationViewModel.Qualities.Any(x => x.Code.ToUpper().Contains("QA"));

            AgeReadingViewModel.AgeReadingKeyMappingViewModel.ToggleEdgeSettings(AgeReadingViewModel.Analysis.ShowEdgeColumn, hasQa);
            AgeReadingViewModel.AgeReadingKeyMappingView.ShowDialog();
        }

        public void bReset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Brightness = 0;
        }
        public void cReset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Contrast = 0;
        }


        #region shortcuts
        public void InitializeShortcuts()
        {
            ShortcutActions = new Dictionary<int, Tuple<Action, string, string>>()
            {

                //{ 0, new Tuple<Action, string, string>(Action0, "Set Approved + Select next file", "") },
                { 1, new Tuple<Action, string, string>(Action1, "Set AQ1 + Select next file", "") },
                { 2, new Tuple<Action, string, string>(Action2, "Set AQ2 + Select next file", "")},
                { 3, new Tuple<Action, string, string>(Action3, "Set AQ3 + Select next file", "")},
                { 4, new Tuple<Action, string, string>(Action4, "Set AQ3__QA + Select next file", "")},
                { 5, new Tuple<Action, string, string>(Action5, "Set AQ1 + Set Approved + Select next file", "")},
                { 6, new Tuple<Action, string, string>(Action6, "Set AQ2 + Set Approved + Select next file", "")},
                { 7, new Tuple<Action, string, string>(Action7, "Set AQ3 + Set Approved + Select next file", "")},
                { 8, new Tuple<Action, string, string>(Action8, "Set AQ3__QA + Set Approved + Select next file", "")},
                { 9, new Tuple<Action, string, string>(Action9, "Set AQ1 + Add comment + Select next file", "")},
                { 10, new Tuple<Action, string, string>(Action10, "Set AQ2 + Add comment + Select next file", "")},
                { 11, new Tuple<Action, string, string>(Action11, "Set AQ3 + Add comment + Select next file", "")},
                { 12, new Tuple<Action, string, string>(Action12, "Set AQ3__QA + Add comment + Select next file", "")},
                { 13, new Tuple<Action, string, string>(Action13, "Set AQ1 + Add comment + Set Approved + Select next file", "")},
                { 14, new Tuple<Action, string, string>(Action14, "Set AQ2 + Add comment + Set Approved + Select next file", "")},
                { 15, new Tuple<Action, string, string>(Action15, "Set AQ3 + Add comment + Set Approved + Select next file", "")},
                { 16, new Tuple<Action, string, string>(Action16, "Set AQ3__QA + Add comment + Set Approved + Select next file", "")},
                { 17, new Tuple<Action, string, string>(Action17, "Set AQ1 + Set edge: opaque + Set Approved + Select next file", "")},
                { 18, new Tuple<Action, string, string>(Action18, "Set AQ2 + Set edge: opaque + Set Approved + Select next file", "")},
                { 19, new Tuple<Action, string, string>(Action19, "Set AQ3 + Set edge: opaque + Set Approved + Select next file", "")},
                { 20, new Tuple<Action, string, string>(Action20, "Set AQ3__QA + Set edge: opaque + Set Approved + Select next file", "")},
                { 21, new Tuple<Action, string, string>(Action21, "Set AQ1 + Set edge: translucent + Set Approved + Select next file", "")},
                { 22, new Tuple<Action, string, string>(Action22, "Set AQ2 + Set edge: translucent + Set Approved + Select next file", "")},
                { 23, new Tuple<Action, string, string>(Action23, "Set AQ3 + Set edge: translucent + Set Approved + Select next file", "")},
                { 24, new Tuple<Action, string, string>(Action24, "Set AQ3__QA + Set edge: translucent + Set Approved + Select next file", "")},
            };
        }

        public bool SetQualitySubAction(int q)
        {
            var annotation = AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation;
            if (annotation == null)
            {
                AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowErrorToast("Error setting quality", "No annotation currently active");

                return false;
            }

            var quality = AgeReadingViewModel.AgeReadingAnnotationViewModel.Qualities.FirstOrDefault(x => new List<string>() { "AQ" + q, "QS" + q }.Contains(x.Code.ToUpper().Trim()));

            if (q == 0){
                quality = AgeReadingViewModel.AgeReadingAnnotationViewModel.Qualities.FirstOrDefault(x => x.Code.Contains("QA"));
            }

            if (quality == null)
            {
                return false;
            }

            var result1 = AgeReadingViewModel.AgeReadingAnnotationViewModel.SetQuality(annotation, quality.ID, annotation.QualityID);

            if (result1 == false)
            {
                return false;
            }

            return true;
        }

        public void ActionGroup0(int action)
        {
            if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApprove)
            {
                Helper.ShowWinUIMessageBox(AgeReadingViewModel.AgeReadingAnnotationViewModel.ApproveAnnotationTooltip, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AgeReadingViewModel.AgeReadingAnnotationViewModel.Approve();

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }

        public void ActionGroup1(int qualitycode, int action)
        {
            if (!SetQualitySubAction(qualitycode)) return;

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }

        public void ActionGroup2(int qualitycode, int action)
        {
            if (!SetQualitySubAction(qualitycode)) return;

            if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApprove)
            {
                Helper.ShowWinUIMessageBox(AgeReadingViewModel.AgeReadingAnnotationViewModel.ApproveAnnotationTooltip, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AgeReadingViewModel.AgeReadingAnnotationViewModel.Approve();

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }

        public void ActionGroup3(int qualitycode, int action)
        {
            if (!SetQualitySubAction(qualitycode)) return;

            AgeReadingViewModel.AgeReadingAnnotationViewModel.EditAnnotation();

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }

        public void ActionGroup4(int qualitycode, int action)
        {
            if (!SetQualitySubAction(qualitycode)) return;

            AgeReadingViewModel.AgeReadingAnnotationViewModel.EditAnnotation();

            if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApprove)
            {
                Helper.ShowWinUIMessageBox(AgeReadingViewModel.AgeReadingAnnotationViewModel.ApproveAnnotationTooltip, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AgeReadingViewModel.AgeReadingAnnotationViewModel.Approve();

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }

        public void ActionGroup5(int qualitycode, int action, string edge)
        {
            if (!SetQualitySubAction(qualitycode)) return;

            AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.Edge = "Opaque";

            if (!AgeReadingViewModel.AgeReadingAnnotationViewModel.CanApprove)
            {
                Helper.ShowWinUIMessageBox(AgeReadingViewModel.AgeReadingAnnotationViewModel.ApproveAnnotationTooltip, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AgeReadingViewModel.AgeReadingAnnotationViewModel.Approve();

            AgeReadingViewModel.AgeReadingFileView.FileList.MoveNextRow();
            AgeReadingViewModel.AgeReadingView.MainWindowViewModel.ShowSuccessToast("Action complete", ShortcutActions[action].Item2);
        }


        //public void Action0()
        //{
        //    ActionGroup0(0);
        //}
        public void Action1()
        {
            ActionGroup1(1, 1);
        }
        public void Action2()
        {
            ActionGroup1(2, 2);
        }
        public void Action3()
        {
            ActionGroup1(3, 3);
        }

        public void Action4()
        {
            ActionGroup1(0, 4);
        }
        public void Action5()
        {
            ActionGroup2(1, 5);
        }

        public void Action6()
        {
            ActionGroup2(2, 6);
        }

        public void Action7()
        {
            ActionGroup2(3, 7);
        }
        public void Action8()
        {
            ActionGroup2(0, 8);
        }
        public void Action9()
        {
            ActionGroup3(1, 9);
        }
        public void Action10()
        {
            ActionGroup3(2, 10);
        }
        public void Action11()
        {
            ActionGroup3(3, 11);
        }
        public void Action12()
        {
            ActionGroup3(0, 12);
        }

        public void Action13()
        {
            ActionGroup4(1, 13);
        }

        public void Action14()
        {
            ActionGroup4(2, 14);
        }

        public void Action15()
        {
            ActionGroup4(3, 15);
        }
        public void Action16()
        {
            ActionGroup4(0, 16);
        }

        public void Action17()
        {
            ActionGroup5(1, 17, "Opaque");
        }

        public void Action18()
        {
            ActionGroup5(2, 18, "Opaque");
        }

        public void Action19()
        {
            ActionGroup5(3, 19, "Opaque");
        }
        public void Action20()
        {
            ActionGroup5(0, 20, "Opaque");
        }
        public void Action21()
        {
            ActionGroup5(1, 21, "Translucent");
        }

        public void Action22()
        {
            ActionGroup5(2, 22, "Translucent");
        }

        public void Action23()
        {
            ActionGroup5(3, 23, "Translucent");
        }

        public void Action24()
        {
            ActionGroup5(0, 24, "Translucent");
        }
        #endregion
    }
}
