using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AgeReading.Graph
{
    public partial class LegendControl : UserControl
    {
        List<LegendLabel> _Labels = new List<LegendLabel>();

        bool _AllowHighlightingGraphs = true, _AllowDisablingGraphs = true;

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowDisablingGraphs
        {
            get { return _AllowDisablingGraphs; }
            set { _AllowDisablingGraphs = value; }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowHighlightingGraphs
        {
            get { return _AllowHighlightingGraphs; }
            set { _AllowHighlightingGraphs = value; }
        }

        bool _Autosize;

        public bool Autosize
        {
            get { return _Autosize; }
            set
            {
                _Autosize = value;
                if (value)
                {
                    Width = _AutoWidth;
                    Height = _AutoHeight;
                }
            }
        }

        public LegendControl()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        public string NoGraphsLabel
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        public GraphViewer _GraphViewer;
        int _AutoWidth, _AutoHeight;

        /// <summary>
        /// Collection items should implement ILegendItem
        /// </summary>
        protected virtual System.Collections.IEnumerable Items
        {
            get
            {
                if (_GraphViewer == null)
                    return null;
                return _GraphViewer.DisplayedGraphs;
            }
        }

        public void UpdateLegend()
        {
            foreach (LegendLabel lbl in _Labels)
                lbl.Dispose();
            _Labels.Clear();
            int i = 0;
            if (Items != null)
            {
                _AutoWidth = 0;

                foreach (ILegendItem gr in Items)
                {
                    LegendLabel lbl = new LegendLabel();
                    lbl.Parent = this;
                    lbl.SquareColor = gr.Color;
                    lbl.Text = gr.Hint;
                    lbl.Grayed = gr.Hidden;
                    lbl.Left = 0;
                    lbl.Top = i * lbl.Height + label1.Top;
                    lbl.Width = Width;
                    lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    lbl.Tag = gr;
                    lbl.Cursor = Cursors.Arrow;

                    lbl.MouseClick += new MouseEventHandler(LegendPanel_MouseClick);
                    lbl.MouseEnter += new EventHandler(LegendPanel_MouseEnter);
                    lbl.MouseLeave += new EventHandler(LegendPanel_MouseLeave);

                    lbl.MouseMove += new MouseEventHandler(lbl_MouseMove);
                    lbl.MouseDown += new MouseEventHandler(lbl_MouseDown);

                    _AutoWidth = Math.Max(_AutoWidth, lbl.Left + lbl.MeasureWidth() + 10);
                    _AutoHeight = lbl.Bottom + 10;

                    lbl.Visible = true;
                    _Labels.Add(lbl);
                    i++;
                }

                label1.Visible = false;
            }

            if (i == 0)
            {
                label1.Visible = true;
                _AutoWidth = label1.Right + 10;
                _AutoHeight = label1.Bottom + 10;
            }
            if (_Autosize)
            {
                Width = _AutoWidth;
                Height = _AutoHeight;
            }
        }

        protected virtual void lbl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newPoint = PointToClient((sender as Control).PointToScreen(e.Location));
                OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, newPoint.X, newPoint.Y, e.Delta));
            }
        }

        protected virtual void lbl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newPoint = PointToClient((sender as Control).PointToScreen(e.Location));
                OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, newPoint.X, newPoint.Y, e.Delta));
            }
        }

        public delegate void HandleLabelHilighted(ILegendItem item);
        public event HandleLabelHilighted OnLabelHilighted;

        LegendLabel _ActiveLabel;

        public ILegendItem ActiveItem
        {
            get
            {
                if (_ActiveLabel == null)
                    return null;
                return _ActiveLabel.Tag as ILegendItem;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _ActiveLabel = null;

        }

        void LegendPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!_AllowHighlightingGraphs)
                return;
            LegendLabel lbl = sender as LegendLabel;
            lbl.BackColor = BackColor;

            if (OnLabelHilighted != null)
                OnLabelHilighted(null);
        }

        void LegendPanel_MouseEnter(object sender, EventArgs e)
        {
            _ActiveLabel = sender as LegendLabel;
            if (!_AllowHighlightingGraphs)
                return;
            LegendLabel lbl = sender as LegendLabel;
            lbl.BackColor = _HighlightedColor;
            if (OnLabelHilighted != null)
                OnLabelHilighted((ILegendItem)lbl.Tag);
        }

        public delegate void HandleLabelGrayed(ILegendItem graph, bool grayed);
        public event HandleLabelGrayed OnLabelGrayed;

        virtual protected void LegendPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (!_AllowDisablingGraphs)
                return;
            LegendLabel label = sender as LegendLabel;
            label.Grayed = !label.Grayed;
            if (OnLabelGrayed != null)
                OnLabelGrayed((ILegendItem)label.Tag, label.Grayed);
        }

        private Color _HighlightedColor = Color.LemonChiffon;

        public System.Drawing.Color HighlightedColor
        {
            get { return _HighlightedColor; }
            set
            {
                _HighlightedColor = value;
                Invalidate();
            }
        }

    }
}
