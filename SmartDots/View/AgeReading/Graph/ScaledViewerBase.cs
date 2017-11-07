using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace AgeReading.Graph
{
    public partial class ScaledViewerBase : UserControl
    {
        #region Appearance-related fields and constants
        public delegate void LabelFormatter(object Sender, double Value, ref string FormattedValue);
        public delegate void ValueTransformation(object Sender, bool forwardTransform, ref double Value);
        string _DefaultXFormat = "{0}", _DefaultYFormat = "{0}";
        Padding _AdditionalPadding;
        bool _AlwaysShowZeroX, _AlwaysShowZeroY, _CenterX, _CenterY;
        bool _AntiAlias = true;
        private int plan;

        const int kDistanceForNonFittingPoints = 5;

        GridSettings _XGrid = new GridSettings();
        GridSettings _YGrid = new GridSettings();

        const int SmallRulerDash = 5, BigRulerDash = 10, RulerTextSpacing = 4, MinDistanceBetweenLabels = 5;
        #endregion
        #region Designer properties
        [Category("Axes")]
        public event LabelFormatter FormatXValue, FormatYValue;
        [Category("Axes")]
        public event ValueTransformation TransformX, TransformY;


        [Category("Axes"), DefaultValue("{0}")]
        public string DefaultYFormat
        {
            get { return _DefaultYFormat; }
            set { _DefaultYFormat = value; UpdateScaling(); }
        }

        [Category("Axes"), DefaultValue("{0}")]
        public string DefaultXFormat
        {
            get { return _DefaultXFormat; }
            set { _DefaultXFormat = value; UpdateScaling(); }
        }

        [Category("Appearance"), DefaultValue(true)]
        public bool AntiAlias
        {
            get { return _AntiAlias; }
            set { _AntiAlias = value; Invalidate(); }
        }

        [Category("Axes"), DefaultValue(false)]
        public bool CenterY
        {
            get { return _CenterY; }
            set { _CenterY = value; UpdateScaling(); }
        }

        [Category("Axes"), DefaultValue(false)]
        public bool CenterX
        {
            get { return _CenterX; }
            set { _CenterX = value; UpdateScaling(); }
        }

        [Category("Axes"), DefaultValue(false)]
        public bool AlwaysShowZeroY
        {
            get { return _AlwaysShowZeroY; }
            set { _AlwaysShowZeroY = value; UpdateScaling(); }
        }

        [Category("Axes"), DefaultValue(false)]
        public bool AlwaysShowZeroX
        {
            get { return _AlwaysShowZeroX; }
            set { _AlwaysShowZeroX = value; UpdateScaling(); }
        }

        [Category("Appearance")]
        public System.Windows.Forms.Padding AdditionalPadding
        {
            get { return _AdditionalPadding; }
            set { _AdditionalPadding = value; UpdateScaling(); }
        }

        [Category("Axes")]
        public GridSettings XGrid
        {
            get { return _XGrid; }
            set { _XGrid = value; UpdateScaling(); }
        }

        [Category("Axes")]
        public GridSettings YGrid
        {
            get { return _YGrid; }
            set { _YGrid = value; }
        }
        #endregion
        #region Coordinate mapping functions
        public int MapX(double transformedValue)
        {
            int val = (int)Math.Round(((transformedValue - TransformedBounds.MinX) * _DataRectangle.Width) / TransformedBounds.DeltaX);
            if (val < 0)
                val = -kDistanceForNonFittingPoints;
            if (val > _DataRectangle.Right)
                val = _DataRectangle.Right + kDistanceForNonFittingPoints;
            return _DataRectangle.Left + val;
        }

        public int MapWidth(double transformedValue)
        {
            int val = (int)Math.Round(((transformedValue) * _DataRectangle.Width) / TransformedBounds.DeltaX);
            if (val < 0)
                val = 0;
            if (val > _DataRectangle.Width)
                val = _DataRectangle.Width;
            return val;
        }

        public int MapY(double transformedValue)
        {
            int val = (int)Math.Round(((transformedValue - TransformedBounds.MinY) * _DataRectangle.Height) / TransformedBounds.DeltaY);
            if (val > _DataRectangle.Bottom)
                val = _DataRectangle.Bottom + kDistanceForNonFittingPoints;
            if (val < 0)
                val = -kDistanceForNonFittingPoints;
            return _DataRectangle.Bottom - val;
        }

        public int MapHeight(double transformedValue)
        {
            int val = (int)Math.Round(((transformedValue) * _DataRectangle.Height) / TransformedBounds.DeltaY);
            if (val > _DataRectangle.Height)
                val = _DataRectangle.Height;
            if (val < 0)
                val = 0;
            return val;
        }

        public int MapX(double value, bool transform)
        {
            if (transform && (TransformX != null))
                TransformX(this, true, ref value);
            return MapX(value);
        }

        public int MapY(double value, bool transform)
        {
            if (transform && (TransformY != null))
                TransformY(this, true, ref value);
            return MapY(value);
        }

        public double UnmapX(int value, bool reverseTransform)
        {
            double x = TransformedBounds.MinX + (value - _DataRectangle.Left) * TransformedBounds.DeltaX / _DataRectangle.Width;
            if (reverseTransform && (TransformX != null))
                TransformX(this, false, ref x);
            return x;
        }

        public double UnmapWidth(int value)
        {
            return (value) * TransformedBounds.DeltaX / _DataRectangle.Width;
        }

        public double UnmapY(int value, bool reverseTransform)
        {
            double y = TransformedBounds.MinY + (_DataRectangle.Bottom - value) * TransformedBounds.DeltaY / _DataRectangle.Height;
            if (reverseTransform && (TransformY != null))
                TransformY(this, false, ref y);
            return y;
        }

        public double UnmapHeight(int value)
        {
            return (value) * TransformedBounds.DeltaY / _DataRectangle.Height;
        }
        #endregion
        #region Grid-related objects
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class GridSettings
        {
            public enum GridSpacingKind
            {
                FixedSpacing,
                MaxLinesPerGraph,
                MinPixelsBetweenLines
            }

            const double kDefaultSpacing = 40;

            GridSpacingKind _SpacingKind = GridSpacingKind.MinPixelsBetweenLines;
            double _SpacingParameter = kDefaultSpacing;
            double _SpacingDivider = 0;
            Color _LineColor = Color.FromArgb(150, 150, 150);
            float _LineWidth = 1;
            bool _ProportionalToTransformedScale = true;
            bool _ShowGridLines = true;
            bool _ShowLabels = false;
            bool _DistributeLabelsEvenly = true;
            bool _TransformLabelValues = true;
            #region Properties
            public bool ProportionalToTransformedScale
            {
                get { return _ProportionalToTransformedScale; }
                set { _ProportionalToTransformedScale = value; }
            }

            public bool TransformLabelValues
            {
                get { return _TransformLabelValues; }
                set { _TransformLabelValues = value; }
            }

            public bool ShowGridLines
            {
                get { return _ShowGridLines; }
                set { _ShowGridLines = value; }
            }

            public bool ShowLabels
            {
                get { return _ShowLabels; }
                set { _ShowLabels = value; }
            }

            [DefaultValue(true)]
            public bool DistributeLabelsEvenly
            {
                get { return _DistributeLabelsEvenly; }
                set { _DistributeLabelsEvenly = value; }
            }

            [DefaultValue(GridSpacingKind.MinPixelsBetweenLines)]
            public GridSpacingKind SpacingKind
            {
                get { return _SpacingKind; }
                set { _SpacingKind = value; }
            }
            [DefaultValue(kDefaultSpacing)]
            public double SpacingParameter
            {
                get { return _SpacingParameter; }
                set { _SpacingParameter = value; }
            }
            [DefaultValue(0.0)]
            public double SpacingDivider
            {
                get { return _SpacingDivider; }
                set { _SpacingDivider = value; }
            }
            public System.Drawing.Color LineColor
            {
                get { return _LineColor; }
                set { _LineColor = value; }
            }

            public bool ShouldSerializeLineColor()
            {
                return LineColor != Color.FromArgb(150, 150, 150);
            }

            [DefaultValue(1.0F)]
            public float LineWidth
            {
                get { return _LineWidth; }
                set { _LineWidth = value; }
            }
            #endregion
            public override string ToString()
            {
                switch (SpacingKind)
                {
                    case GridSpacingKind.FixedSpacing:
                        return string.Format("Every {0} unit(s)", SpacingParameter);
                    case GridSpacingKind.MaxLinesPerGraph:
                        return string.Format("Exactly {0} lines(s) per graph", (int)SpacingParameter);
                    case GridSpacingKind.MinPixelsBetweenLines:
                        return string.Format("Every {0} pixels", (int)SpacingParameter);
                    default:
                        return "(???)";
                }
            }

            internal GridDimension CreateDimensionObjectTemplate(double minTransformed, double maxTransformed, double minNonTransformed, double maxNonTransformed, int pixelRange, int p)
            {
                double gridStart, gridSpacing, gridEnd;
                bool gridTransformed;
                int plan = p;


                gridStart = _ProportionalToTransformedScale ? minTransformed : minNonTransformed;
                gridEnd = _ProportionalToTransformedScale ? maxTransformed : maxNonTransformed;
                gridTransformed = _ProportionalToTransformedScale;




                gridSpacing = 1;

                switch (_SpacingKind)
                {
                    case GridSpacingKind.FixedSpacing:
                        gridSpacing = _SpacingParameter;
                        break;
                    case GridSpacingKind.MaxLinesPerGraph:
                        if (_ProportionalToTransformedScale)
                            gridSpacing = (maxTransformed - minTransformed) / ((_SpacingParameter == 0) ? 1 : _SpacingParameter);
                        else
                            gridSpacing = (maxNonTransformed - minNonTransformed) / ((_SpacingParameter == 0) ? 1 : _SpacingParameter);
                        break;
                    case GridSpacingKind.MinPixelsBetweenLines:
                        if (_ProportionalToTransformedScale)
                            gridSpacing = (_SpacingParameter * (maxTransformed - minTransformed)) / pixelRange;
                        else
                            gridSpacing = (_SpacingParameter * (minNonTransformed - minNonTransformed)) / pixelRange;
                        break;
                }

                if (_SpacingDivider != 0)
                {
                    gridStart = Math.Floor(gridStart / _SpacingDivider) * _SpacingDivider;
                    gridSpacing = Math.Ceiling(gridSpacing / _SpacingDivider) * _SpacingDivider;
                }

                if (gridSpacing == 0)
                    gridSpacing = 1;

                List<GridLine> lines = new List<GridLine>();

                for (double val = gridStart; val < gridEnd; val += Math.Abs(gridSpacing))
                    lines.Add(new GridLine { RawValue = val });

                return new GridDimension { Transformed = gridTransformed, Data = lines.ToArray() };
            }

            internal Pen CreatePen()
            {
                return new Pen(_LineColor, _LineWidth);
            }
        }

        internal class GridLine
        {
            public double RawValue;
            public int ScreenCoordinate, TextCoordinate;
            public string Label;
            public int LabelSize;
            public bool LabelVisible;
        }

        internal class GridDimension
        {
            public bool Transformed;
            public GridLine[] Data;

            internal void ComputeLabelVisibility(bool spaceEvenly, bool VerticalAxis)
            {
                int direction = VerticalAxis ? -1 : 1;
                if (Data.Length != 0)
                {
                    int minLabelPositionIncrement = 1;
                    for (; ; )
                    {
                        int nextAvailablePosition = Data[0].TextCoordinate + (Data[0].LabelSize + MinDistanceBetweenLabels) * direction;
                        int lastVisibleLabel = 0;

                        foreach (GridLine line in Data)
                            line.LabelVisible = false;

                        GridLine lastLine = Data[Data.Length - 1];
                        Data[0].LabelVisible = true;
                        lastLine.LabelVisible = true;

                        bool restart = false;
                        for (int i = minLabelPositionIncrement; i < (Data.Length - 1); i += minLabelPositionIncrement)
                        {
                            GridLine line = Data[i];
                            if (line.TextCoordinate * direction < nextAvailablePosition * direction)
                                continue;
                            if ((line.TextCoordinate + (line.LabelSize + MinDistanceBetweenLabels) * direction) * direction > lastLine.TextCoordinate * direction)
                                break;
                            line.LabelVisible = true;

                            if (spaceEvenly && (lastVisibleLabel != 0))
                            {
                                int thisIncrement = i - lastVisibleLabel;
                                if (thisIncrement > minLabelPositionIncrement)
                                {
                                    minLabelPositionIncrement = thisIncrement;
                                    restart = true;
                                    break;
                                }
                            }

                            nextAvailablePosition = line.TextCoordinate + (Data[0].LabelSize + MinDistanceBetweenLabels) * direction;
                            lastVisibleLabel = i;
                        }
                        if (!restart)
                            break;
                    }
                }
            }
        }

        GridDimension _XGridObj, _YGridObj;
        #endregion

        public ScaledViewerBase()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        Rectangle _DataRectangle;
        GraphBounds _TransformedBounds, _ForcedBounds;
        bool _ForceCustomBounds;

        public bool ForceCustomBounds
        {
            get { return _ForceCustomBounds; }
            set { _ForceCustomBounds = value; UpdateScaling(); }
        }

        public void ForceNewBounds(double minX, double maxX, double minY, double maxY)
        {
            if (TransformX != null)
            {
                TransformX(this, true, ref minX);
                TransformX(this, true, ref maxX);
            }
            if (TransformY != null)
            {
                TransformY(this, true, ref minY);
                TransformY(this, true, ref maxY);
            }

            _ForcedBounds = new GraphBounds(minX, maxX, minY, maxY);
            ForceCustomBounds = true;
        }

        protected GraphBounds TransformedBounds
        {
            get
            {
                if (_ForceCustomBounds)
                {
                    GraphBounds forcedBounds = _ForcedBounds;
                    if (double.IsNaN(forcedBounds.MinX))
                        forcedBounds.MinX = _TransformedBounds.MinX;
                    if (double.IsNaN(forcedBounds.MaxX))
                        forcedBounds.MaxX = _TransformedBounds.MaxX;
                    if (double.IsNaN(forcedBounds.MinY))
                        forcedBounds.MinY = _TransformedBounds.MinY;
                    if (double.IsNaN(forcedBounds.MaxY))
                        forcedBounds.MaxY = _TransformedBounds.MaxY;
                    return forcedBounds;
                }
                else
                    return _TransformedBounds;
            }
        }

        protected Rectangle DataRectangle
        {
            get { return _DataRectangle; }
        }

        public virtual void UpdateScaling()
        {
            GraphBounds nonTransformedBounds;
            GetRawDataBounds(out nonTransformedBounds, out _TransformedBounds);
            if (_AlwaysShowZeroX)
            {
                _TransformedBounds.MinX = Math.Min(0, _TransformedBounds.MinX);
                _TransformedBounds.MaxX = Math.Max(0, _TransformedBounds.MaxX);
            }
            if (/*_AlwaysShowZeroY*/plan == 1 || plan == 2)
            {
                _TransformedBounds.MinY = 0;
                _TransformedBounds.MaxY = 1;
                //_TransformedBounds.MinY = Math.Min(0, _TransformedBounds.MinY);
                //_TransformedBounds.MaxY = Math.Max(0, _TransformedBounds.MaxY);
            }
            if (_CenterX)
            {
                double max = Math.Max(Math.Abs(_TransformedBounds.MinX), Math.Abs(_TransformedBounds.MaxX));
                _TransformedBounds.MinX = -max;
                _TransformedBounds.MaxX = max;
            }
            if (_CenterY)
            {
                double max = Math.Max(Math.Abs(_TransformedBounds.MinY), Math.Abs(_TransformedBounds.MaxY));
                _TransformedBounds.MinY = -max;
                _TransformedBounds.MaxY = max;
            }

            GraphBounds transformedBounds = _TransformedBounds;

            if (_ForceCustomBounds)
            {
                transformedBounds = TransformedBounds;
                double minX = transformedBounds.MinX, minY = transformedBounds.MinY, maxX = transformedBounds.MaxX, maxY = transformedBounds.MaxY;
                if (TransformX != null)
                {
                    TransformX(this, false, ref minX);
                    TransformX(this, false, ref maxX);
                }
                if (TransformY != null)
                {
                    TransformY(this, false, ref minY);
                    TransformY(this, false, ref maxY);
                }
                nonTransformedBounds = new GraphBounds(minX, maxX, minY, maxY);
            }

            _DataRectangle = new Rectangle(_AdditionalPadding.Left, _AdditionalPadding.Top, Width - _AdditionalPadding.Horizontal - 1, Height - _AdditionalPadding.Vertical - 1);

            try
            {
                Graphics gr = Graphics.FromHwnd(Handle);
                //Create list of grid points, screen coordinates will be assigned later
                _YGridObj = _YGrid.CreateDimensionObjectTemplate(transformedBounds.MinY, transformedBounds.MaxY, nonTransformedBounds.MinY, nonTransformedBounds.MaxY, _DataRectangle.Height, plan);

                /* Grid/labels computation algorithm:
                 *  1. Compute fixed Y padding (font height)
                 *  2. Determine Y labels, place them, compute X padding (max. Y label width)
                 *  3. Determine X labels, place them
                 *  4. Compute label visibility
                 */

                int maxLabelWidth = 0;
                int yPadding = Size.Ceiling(gr.MeasureString("M", Font)).Height + BigRulerDash + RulerTextSpacing;
                if (!_XGrid.ShowLabels)
                    yPadding = 5;

                //Reflect Y padding only. Used by MapY()
                _DataRectangle = new Rectangle(_AdditionalPadding.Left, _AdditionalPadding.Top, Width - _AdditionalPadding.Horizontal - 1, Height - _AdditionalPadding.Vertical - 1 - yPadding);

                for (int i = 0; i < _YGridObj.Data.Length; i++)
                {
                    GridLine line = _YGridObj.Data[i];
                    double rawVal = line.RawValue;
                    if (_YGrid.TransformLabelValues && !_YGrid.ProportionalToTransformedScale)
                        DoTransformY(ref rawVal);
                    string val = string.Format(_DefaultYFormat, rawVal);
                    if (FormatYValue != null)
                        FormatYValue(this, line.RawValue, ref val);
                    line.Label = val;
                    Size labelSize = Size.Ceiling(gr.MeasureString(val, Font));
                    line.LabelSize = labelSize.Height;
                    maxLabelWidth = Math.Max(maxLabelWidth, labelSize.Width);

                    line.ScreenCoordinate = MapY(line.RawValue, !_YGridObj.Transformed);
                    if (i == 0)
                        line.TextCoordinate = Math.Min(line.ScreenCoordinate - line.LabelSize / 2, _DataRectangle.Bottom - line.LabelSize);
                    else if (i == (_YGridObj.Data.Length - 1))
                        line.TextCoordinate = Math.Max(line.ScreenCoordinate - line.LabelSize / 2, _DataRectangle.Top);
                    else
                        line.TextCoordinate = line.ScreenCoordinate - line.LabelSize / 2;
                }

                int xPadding = maxLabelWidth + BigRulerDash + RulerTextSpacing;
                if (!_YGrid.ShowLabels)
                    xPadding = 0;

                //Reflect both X and Y padding
                _DataRectangle = new Rectangle(_AdditionalPadding.Left + xPadding, _AdditionalPadding.Top, Width - _AdditionalPadding.Horizontal - 1 - xPadding, Height - _AdditionalPadding.Vertical - 1 - yPadding);

                _XGridObj = _XGrid.CreateDimensionObjectTemplate(transformedBounds.MinX, transformedBounds.MaxX, nonTransformedBounds.MinX, nonTransformedBounds.MaxX, _DataRectangle.Width, plan);

                for (int i = 0; i < _XGridObj.Data.Length; i++)
                {
                    GridLine line = _XGridObj.Data[i];
                    double rawVal = line.RawValue;
                    if (_XGrid.TransformLabelValues && !_XGrid.ProportionalToTransformedScale)
                        DoTransformX(ref rawVal);

                    string val = string.Format(_DefaultXFormat, rawVal);
                    if (FormatXValue != null)
                        FormatXValue(this, line.RawValue, ref val);
                    line.Label = val;
                    line.LabelSize = Size.Ceiling(gr.MeasureString(val, Font)).Width;
                    line.LabelVisible = true;

                    line.ScreenCoordinate = MapX(line.RawValue, !_YGridObj.Transformed);
                    if (i == 0)
                        line.TextCoordinate = Math.Max(line.ScreenCoordinate - line.LabelSize / 2, _DataRectangle.Left);
                    else if (i == (_XGridObj.Data.Length - 1))
                        line.TextCoordinate = Math.Min(line.ScreenCoordinate - line.LabelSize / 2, _DataRectangle.Right - line.LabelSize);
                    else
                        line.TextCoordinate = line.ScreenCoordinate - line.LabelSize / 2;
                }

                _YGridObj.ComputeLabelVisibility(_YGrid.DistributeLabelsEvenly && _YGridObj.Transformed, true);
                _XGridObj.ComputeLabelVisibility(_XGrid.DistributeLabelsEvenly && _XGridObj.Transformed, false);
                Invalidate();
            }
            catch (Exception)
            {

            }

            
        }

        protected virtual void GetRawDataBounds(out GraphBounds nonTransformedBounds, out GraphBounds transformedBounds)
        {
            nonTransformedBounds = new GraphBounds();
            transformedBounds = new GraphBounds();
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateScaling();
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_AntiAlias)
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            base.OnPaint(e);

            Pen borderPen = Pens.Black;

            Pen xGridPen = _XGrid.CreatePen(), yGridPen = _YGrid.CreatePen();
            e.Graphics.DrawRectangle(borderPen, _DataRectangle);

            int textHeight = Size.Ceiling(e.Graphics.MeasureString("M", Font)).Height;
            if (_XGridObj != null)
                foreach (GridLine line in _XGridObj.Data)
                {
                    if (_XGrid.ShowGridLines)
                        e.Graphics.DrawLine(xGridPen, line.ScreenCoordinate, _DataRectangle.Top, line.ScreenCoordinate, _DataRectangle.Bottom);
                    if (_XGrid.ShowLabels)
                    {
                        if (line.LabelVisible)
                        {
                            e.Graphics.DrawString(line.Label, Font, Brushes.Black, line.TextCoordinate, ClientRectangle.Bottom - _AdditionalPadding.Bottom - textHeight);
                            e.Graphics.DrawLine(borderPen, line.ScreenCoordinate, _DataRectangle.Bottom + 2, line.ScreenCoordinate, _DataRectangle.Bottom + BigRulerDash + 1);
                        }
                        else
                            e.Graphics.DrawLine(borderPen, line.ScreenCoordinate, _DataRectangle.Bottom + 2, line.ScreenCoordinate, _DataRectangle.Bottom + SmallRulerDash + 1);
                    }
                }

            if (_YGridObj != null)
                foreach (GridLine line in _YGridObj.Data)
                {
                    if (_YGrid.ShowGridLines)
                        e.Graphics.DrawLine(yGridPen, _DataRectangle.Left, line.ScreenCoordinate, _DataRectangle.Right, line.ScreenCoordinate);
                    if (_YGrid.ShowLabels)
                    {
                        if (line.LabelVisible)
                        {
                            e.Graphics.DrawString(line.Label, Font, Brushes.Black, AdditionalPadding.Left, line.TextCoordinate);
                            e.Graphics.DrawLine(borderPen, _DataRectangle.Left - BigRulerDash, line.ScreenCoordinate, _DataRectangle.Left - 2, line.ScreenCoordinate);
                        }
                        else
                            e.Graphics.DrawLine(borderPen, _DataRectangle.Left - SmallRulerDash, line.ScreenCoordinate, _DataRectangle.Left - 2, line.ScreenCoordinate);
                    }
                }
        }

        #region Various helper methods
        protected void DoTransformX(ref double x)
        {
            if (TransformX != null)
                TransformX(this, true, ref x);
        }

        protected void DoTransformY(ref double y)
        {
            if (TransformY != null)
                TransformY(this, true, ref y);
        }

        #endregion


        internal void SetPlan(int plan)
        {
            this.plan = plan;
        }
    }
}
