using System;
using System.Collections.Generic;
using System.Windows;

namespace SmartDots.Model
{
    [Serializable]
    public class Line
    {
        public System.Guid ID { get; set; }
        public System.Guid AnnotationID { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
        public string Color { get; set; }
        public Nullable<int> LineIndex { get; set; }

        public CombinedLine ParentCombinedLine { get; set; }

        public List<Point> GetPoints(int X1, int Y1, int X2, int Y2)
        {
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);

            int sx = X1 < X2 ? 1 : -1;
            int sy = Y1 < Y2 ? 1 : -1;

            int err = dx - dy;

            var points = new List<Point>();
            var points2 = new List<Point>();


            while (true)
            {
                points.Add(new Point(X1, Y1));
                if (X1 == X2 && Y1 == Y2) break;

                int e2 = 2*err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    X1 = X1 + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    Y1 = Y1 + sy;
                }
            }
            return points;
        }
    }
}
