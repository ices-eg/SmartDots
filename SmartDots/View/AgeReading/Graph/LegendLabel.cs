using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AgeReading.Graph
{
    internal partial class LegendLabel : UserControl
    {
        public LegendLabel()
        {
            InitializeComponent();
        }

        private bool _Grayed = false;
        public bool Grayed { get { return _Grayed; } set { _Grayed = value; Invalidate(); } }

        private bool _BoldFont = false;
        public bool BoldFont { get { return _BoldFont; } set { _BoldFont = value; Invalidate(); } }

        private Color _SquareColor = Color.Red;

        public System.Drawing.Color SquareColor
        {
            get { return _SquareColor; }
            set { _SquareColor = value; Invalidate(); }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        const int kPadding = 5;
        const int kMaxOffset = 20;

        public int MeasureWidth()
        {
            Graphics gr = Graphics.FromHwnd(Handle);
            Font boldFont = new Font(Font, FontStyle.Bold);
            int textWidth = (int)Math.Ceiling(gr.MeasureString(Text, BoldFont ? boldFont : Font).Width);
            return kMaxOffset + textWidth + 10;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int dimension = Math.Min(kMaxOffset - kPadding * 2, Height - kPadding * 2);

            int x = (kMaxOffset - dimension) / 2;
            int y = (Height - dimension) / 2;

            Brush textBrush;

            if (Grayed)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, _SquareColor)), x, y, dimension, dimension);
                e.Graphics.DrawRectangle(Pens.DarkGray, x, y, dimension, dimension);
                textBrush = Brushes.DarkGray;
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(_SquareColor), x, y, dimension, dimension);
                e.Graphics.DrawRectangle(Pens.Black, x, y, dimension, dimension);
                textBrush = Brushes.Black;
            }

            StringFormat fmt = new StringFormat();
            fmt.LineAlignment = StringAlignment.Center;

            Font boldFont = new Font(Font, FontStyle.Bold);

            e.Graphics.DrawString(Text, BoldFont ? boldFont : Font, textBrush, new Rectangle(kMaxOffset, 0, Width - kMaxOffset, Height), fmt);
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }
    }
}
