using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AgeReading.Graph
{
    public partial class InteractiveGraphViewer : GraphViewer
    {
        public new event GraphMouseEventHandler MouseMove, MouseDown, MouseUp, MouseDoubleClick, MouseClick;

        const int kHintOffset = 5, kSmallTrackerSize = 10;

        public class Tracker
        {
            int _LineWidth = 1;
            bool _Hidden;
            Color _LineColor;
            double _X, _Y;
            internal InteractiveGraphViewer _Viewer;

            public enum TrackerStyle
            {
                Cross,
                Horizontal,
                Vertical,
                SmallX,
            }

            private void InvalidateViewer()
            {
                if (_Viewer != null)
                    _Viewer.Invalidate();
            }

            TrackerStyle _Style;
            #region Properties
            public int LineWidth
            {
                get { return _LineWidth; }
                set { _LineWidth = value; InvalidateViewer(); }
            }

            public System.Drawing.Color LineColor
            {
                get { return _LineColor; }
                set { _LineColor = value; InvalidateViewer(); }
            }

            public double Y
            {
                get { return _Y; }
                set { _Y = value; InvalidateViewer(); }
            }

            public double X
            {
                get { return _X; }
                set { _X = value; InvalidateViewer(); }
            }
            public bool Hidden
            {
                get { return _Hidden; }
                set { _Hidden = value; InvalidateViewer(); }
            }

            public TrackerStyle Style
            {
                get { return _Style; }
                set { _Style = value; InvalidateViewer(); }
            }

            #endregion

            internal Tracker(InteractiveGraphViewer viewer)
            {
                _Viewer = viewer;
            }

            public Tracker()
            {
            }

            public void Delete()
            {
                _Viewer._Trackers.Remove(this);
            }

        }
        public class FloatingHint
        {
            string _Text;
            bool _Hidden = false;
            double _X, _Y;
            int _FrameThickness = 1;
            Color _FrameColor = Color.Black, _FillColor = Color.FromArgb(200, Color.White), _TextColor = Color.Black;
            InteractiveGraphViewer _Viewer;
            Font _Font;
            const int kPadding = 4;

            #region Properties
            public string Text
            {
                get { return _Text; }
                set { _Text = value; }
            }
            public double Y
            {
                get { return _Y; }
                set { _Y = value; }
            }
            public double X
            {
                get { return _X; }
                set { _X = value; }
            }
            public int FrameThickness
            {
                get { return _FrameThickness; }
                set { _FrameThickness = value; }
            }
            public Color FillColor
            {
                get { return _FillColor; }
                set { _FillColor = value; }
            }
            public System.Drawing.Color FrameColor
            {
                get { return _FrameColor; }
                set { _FrameColor = value; }
            }
            public bool Hidden
            {
                get { return _Hidden; }
                set { _Hidden = value; }
            }
            public Color TextColor
            {
                get { return _TextColor; }
                set { _TextColor = value; }
            }
            public Font Font
            {
                get { return _Font; }
                set { _Font = value; }
            }

            public void Show(GraphMouseEventArgs e, string text)
            {
                _Hidden = false;
                _X = e.DataX;
                _Y = e.DataY;
                _Text = text;
            }

            #endregion

            public FloatingHint(InteractiveGraphViewer viewer)
            {
                _Viewer = viewer;
            }

            public Size MeasureOrDraw(Graphics gr, Point lowerLeftCorner, bool measureOnly)
            {
                Font font = _Font;
                if (font == null)
                    font = _Viewer.Font;
                Size stringSize = Size.Ceiling(gr.MeasureString(_Text, font));
                Size finalSize = new Size(stringSize.Width + kPadding * 2, stringSize.Height + kPadding * 2);
                if (!measureOnly)
                {
                    gr.FillRectangle(new SolidBrush(_FillColor), lowerLeftCorner.X, lowerLeftCorner.Y, finalSize.Width, finalSize.Height);
                    gr.DrawRectangle(new Pen(_FrameColor, _FrameThickness), lowerLeftCorner.X, lowerLeftCorner.Y, finalSize.Width, finalSize.Height);
                    gr.DrawString(_Text, font, new SolidBrush(_TextColor), lowerLeftCorner.X + kPadding, lowerLeftCorner.Y + kPadding);
                }

                return finalSize;
            }
        }

        public class PreviewRect
        {
            double _X1, _Y1, _X2, _Y2;
            InteractiveGraphViewer _Viewer;
            bool _Visible;
            Color _LineColor = Color.Black, _FillColor = Color.FromArgb(100, Color.DarkGoldenrod);
            int _LineWidth = 1;

            #region Properties
            public bool Visible
            {
                get { return _Visible; }
                set { _Visible = value; _Viewer.Invalidate(); }
            }
            public System.Drawing.Color FillColor
            {
                get { return _FillColor; }
                set { _FillColor = value; _Viewer.Invalidate(); }
            }
            public System.Drawing.Color LineColor
            {
                get { return _LineColor; }
                set { _LineColor = value; _Viewer.Invalidate(); }
            }
            public int LineWidth
            {
                get { return _LineWidth; }
                set { _LineWidth = value; _Viewer.Invalidate(); }
            }
            public double Bottom
            {
                get { return Math.Min(_Y1, _Y2); }
            }
            public double Right
            {
                get { return Math.Max(_X1, _X2); }
            }
            public double Top
            {
                get { return Math.Max(_Y1, _Y2); }
            }
            public double Left
            {
                get { return Math.Min(_X1, _X2); }
            }
            public double MinY
            {
                get { return Math.Min(_Y1, _Y2); }
            }
            public double MaxX
            {
                get { return Math.Max(_X1, _X2); }
            }
            public double MaxY
            {
                get { return Math.Max(_Y1, _Y2); }
            }
            public double MinX
            {
                get { return Math.Min(_X1, _X2); }
            }
            public double Y2
            {
                get { return _Y2; }
                set { _Y2 = value; _Viewer.Invalidate(); }
            }
            public double X2
            {
                get { return _X2; }
                set { _X2 = value; _Viewer.Invalidate(); }
            }
            public double Y1
            {
                get { return _Y1; }
                set { _Y1 = value; _Viewer.Invalidate(); }
            }
            public double X1
            {
                get { return _X1; }
                set { _X1 = value; _Viewer.Invalidate(); }
            }
            #endregion

            internal PreviewRect(InteractiveGraphViewer viewer)
            {
                _Viewer = viewer;
            }

        }

        public class DistanceMeasurer
        {
            public const string CapitalDelta = "\u0394";

            double _X1, _Y1, _X2, _Y2;
            InteractiveGraphViewer _Viewer;
            bool _Visible;
            Color _LineColor = Color.Black;
            int _LineWidth = 1;

            #region Properties
            public bool Visible
            {
                get { return _Visible; }
                set { _Visible = value; _Viewer.Invalidate(); }
            }
            public System.Drawing.Color LineColor
            {
                get { return _LineColor; }
                set { _LineColor = value; _Viewer.Invalidate(); }
            }
            public int LineWidth
            {
                get { return _LineWidth; }
                set { _LineWidth = value; _Viewer.Invalidate(); }
            }
            public double Y2
            {
                get { return _Y2; }
                set { _Y2 = value; _Viewer.Invalidate(); }
            }
            public double X2
            {
                get { return _X2; }
                set { _X2 = value; _Viewer.Invalidate(); }
            }
            public double Y1
            {
                get { return _Y1; }
                set { _Y1 = value; _Viewer.Invalidate(); }
            }
            public double X1
            {
                get { return _X1; }
                set { _X1 = value; _Viewer.Invalidate(); }
            }
            #endregion

            internal DistanceMeasurer(InteractiveGraphViewer viewer)
            {
                _Viewer = viewer;
            }
        }

        PreviewRect _PreviewRectangle;
        DistanceMeasurer _DistanceLine;

        public DistanceMeasurer DistanceLine
        {
            set { _DistanceLine = value; }
        }

        public PreviewRect PreviewRectangle
        {
            get { return _PreviewRectangle; }
        }

        List<Tracker> _Trackers = new List<Tracker>();
        List<FloatingHint> _Hints = new List<FloatingHint>();

        public InteractiveGraphViewer()
        {
            _PreviewRectangle = new PreviewRect(this);
            _DistanceLine = new DistanceMeasurer(this);
            InitializeComponent();
        }

        #region Mouse events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            double x = UnmapX(e.X, true), y = UnmapY(e.Y, true);
            if (MouseMove != null)
                MouseMove(this, new GraphMouseEventArgs(e, x, y));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            double x = UnmapX(e.X, true), y = UnmapY(e.Y, true);
            if (MouseDown != null)
                MouseDown(this, new GraphMouseEventArgs(e, x, y));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            double x = UnmapX(e.X, true), y = UnmapY(e.Y, true);
            if (MouseUp != null)
                MouseUp(this, new GraphMouseEventArgs(e, x, y));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            double x = UnmapX(e.X, true), y = UnmapY(e.Y, true);
            if (MouseClick != null)
                MouseClick(this, new GraphMouseEventArgs(e, x, y));
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            double x = UnmapX(e.X, true), y = UnmapY(e.Y, true);
            if (MouseDoubleClick != null)
                MouseDoubleClick(this, new GraphMouseEventArgs(e, x, y));
        }
        #endregion


        public Tracker AddNewTracker(Tracker tracker)
        {
            if (tracker._Viewer != null)
                throw new InvalidOperationException("The tracker is already added to a viewer");
            tracker._Viewer = this;
            _Trackers.Add(tracker);
            return tracker;
        }

        public Tracker CreateTracker(Color color, double x, double y)
        {
            Tracker tracker = new Tracker(this) { LineColor = color, X = x, Y = y };
            _Trackers.Add(tracker);
            return tracker;
        }

        public Tracker CreateTracker(Color color)
        {
            Tracker tracker = new Tracker(this) { LineColor = color, Hidden = true };
            _Trackers.Add(tracker);
            return tracker;
        }

        public FloatingHint CreateFloatingHint(string text, double x, double y)
        {
            FloatingHint hint = new FloatingHint(this) { Text = text, X = x, Y = y };
            _Hints.Add(hint);
            return hint;
        }

        public FloatingHint CreateFloatingHint()
        {
            FloatingHint hint = new FloatingHint(this) { Hidden = true };
            _Hints.Add(hint);
            return hint;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (Tracker trk in _Trackers)
            {
                if (trk.Hidden)
                    continue;
                Pen trackerPen = new Pen(trk.LineColor, trk.LineWidth);

                int x = MapX(trk.X, true), y = MapY(trk.Y, true);
                switch (trk.Style)
                {
                    case Tracker.TrackerStyle.Cross:
                    case Tracker.TrackerStyle.Horizontal:
                        e.Graphics.DrawLine(trackerPen, DataRectangle.Left, y, DataRectangle.Right, y);
                        break;
                }
                switch (trk.Style)
                {
                    case Tracker.TrackerStyle.Cross:
                    case Tracker.TrackerStyle.Vertical:
                        e.Graphics.DrawLine(trackerPen, x, DataRectangle.Top, x, DataRectangle.Bottom);
                        break;
                }
                switch (trk.Style)
                {
                    case Tracker.TrackerStyle.SmallX:
                        e.Graphics.DrawLine(trackerPen, x - kSmallTrackerSize / 2, y + kSmallTrackerSize / 2, x + kSmallTrackerSize / 2, y - kSmallTrackerSize / 2);
                        e.Graphics.DrawLine(trackerPen, x + kSmallTrackerSize / 2, y + kSmallTrackerSize / 2, x - kSmallTrackerSize / 2, y - kSmallTrackerSize / 2);
                        break;
                }
            }

            foreach (FloatingHint hint in _Hints)
            {
                if (hint.Hidden)
                    continue;

                int x = MapX(hint.X, true), y = MapY(hint.Y, true);
                Size neededSize = hint.MeasureOrDraw(e.Graphics, new Point(), true);
                int finalX = x + kHintOffset, finalY = y - kHintOffset - neededSize.Height;
                if ((finalX + neededSize.Width) > DataRectangle.Right)
                    finalX = x - kHintOffset - neededSize.Width;
                if (finalY < DataRectangle.Top)
                    finalY = y + kHintOffset;
                hint.MeasureOrDraw(e.Graphics, new Point(finalX, finalY), false);
            }

            if ((_PreviewRectangle != null) && _PreviewRectangle.Visible)
            {
                Brush br = new SolidBrush(_PreviewRectangle.FillColor);
                Pen pen = new Pen(_PreviewRectangle.LineColor, _PreviewRectangle.LineWidth);

                Rectangle r = Rectangle.FromLTRB(MapX(_PreviewRectangle.Left, true), MapY(_PreviewRectangle.Top, true), MapX(_PreviewRectangle.Right, true), MapY(_PreviewRectangle.Bottom, true));
                e.Graphics.FillRectangle(br, r);
                e.Graphics.DrawRectangle(pen, r);
            }

            if ((_DistanceLine != null) && _DistanceLine.Visible)
            {
                Pen pen = new Pen(_DistanceLine.LineColor, _DistanceLine.LineWidth);
                e.Graphics.DrawLine(pen, MapX(_DistanceLine.X1, true), MapY(_DistanceLine.Y1, true), MapX(_DistanceLine.X2, true), MapY(_DistanceLine.Y2, true));
            }
        }

        string _MaximizedModeTitle = "";

        [Category("Appearance")]
        [DefaultValue("")]
        public string MaximizedModeTitle
        {
            get { return _MaximizedModeTitle; }
            set { _MaximizedModeTitle = value; }
        }

        //internal bool _Maximized;


        internal new void SetPlan(int plan)
        {
            base.SetPlan(plan);
        }
    }

    public delegate void GraphMouseEventHandler(InteractiveGraphViewer sender, GraphMouseEventArgs e);
    public class GraphMouseEventArgs : MouseEventArgs
    {
        double _DataX, _DataY;

        public GraphMouseEventArgs(MouseEventArgs args, double x, double y)
            : base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
        {
            _DataX = x;
            _DataY = y;
        }

        public double DataX { get { return _DataX; } }
        public double DataY { get { return _DataY; } }
    }
}
