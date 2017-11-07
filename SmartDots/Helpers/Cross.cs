using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartDots.Helpers
{
    public class Cross : Shape
    {
        private float width;
        private double x;
        private double y;
        private Brush fill;
        private Line line1;
        private Line line2;
        protected override Geometry DefiningGeometry { get; }

        public Cross()
        {
            line1 = new Line();
            line2 = new Line();
        }

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                Line1.StrokeThickness = Width/8;
                Line2.StrokeThickness = Width/8;
            }
        }

        public double X
        {
            get { return x; }
            set
            {
                x = value;
                Line1.X1 = x - Width/2;
                Line1.X2 = x + Width / 2;
                Line2.X1 = x - Width / 2;
                Line2.X2 = x + Width / 2;
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                Line1.Y1 = y - Width / 2;
                Line1.Y2 = y + Width / 2;
                Line2.Y1 = y + Width / 2;
                Line2.Y2 = y - Width / 2;
            }
        }

        public Brush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                Line1.Stroke = fill;
                Line2.Stroke = fill;
            }
        }

        public Line Line1
        {
            get { return line1; }
            set { line1 = value; }
        }

        public Line Line2
        {
            get { return line2; }
            set { line2 = value; }
        }
    }
}
