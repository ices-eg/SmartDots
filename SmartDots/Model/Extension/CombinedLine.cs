using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Windows.Point;

namespace SmartDots.Model
{
    [Serializable]

    public class CombinedLine
    {
        public Guid ID { get; set; }
        public List<Line> Lines { get; set; }
        public List<Dot> Dots { get; set; }
        public List<LinePoint> Points { get; set; }
        public bool IsFixed { get; set; }

        public List<int> DotIndex { get; set; } = new List<int>();

        public List<Point> DotPoints
        {
            get
            {
                List<Point> points = new List<Point>();

                foreach (Dot d in Dots)
                {
                    points.Add(d.Location);
                }
                return points;
            }
        }

        public List<int> DotDistances
        {
            get
            {
                List<int> distances = new List<int>();
                int dotCounter = 0;
                int numberOfDots = Dots.Count;
                int pointCounter = 0;

                List<LinePoint> linePoints = Points;

                List<Point> dotPoints = DotPoints;

                while (dotCounter < numberOfDots)
                {
                    foreach (LinePoint p in linePoints)
                    {
                        if (p.X == (int)dotPoints[dotCounter].X && p.Y == (int)dotPoints[dotCounter].Y)
                        {
                            dotCounter++;
                            distances.Add(pointCounter);
                            pointCounter = 0;
                        }
                        else
                        {
                            pointCounter++;
                        }
                    }
                }
                return distances;
            }
        }

        public CombinedLine()
        {
            ID = Guid.NewGuid();
            Lines = new List<Line>();
            Dots = new List<Dot>();
        }

        public List<decimal> GetLineBrightness(Bitmap img)
        {
            List<decimal> brightness = new List<decimal>();

            try
            {
                if (Points == null) RecalculatePoints();
                if (Points != null)
                {
                    foreach (var point in Points)
                    {
                        brightness.Add(new decimal(img.GetPixel(point.X, point.Y).GetBrightness()));
                    }
                }
            }
            catch (Exception)
            {
                return new List<decimal>();
            }

            return brightness;
        }

        public List<decimal> GetLineRedness(Bitmap img)
        {
            List<decimal> redness = new List<decimal>();

            try
            {
                if (Points == null) RecalculatePoints();
                if (Points != null)
                {
                    foreach (var point in Points)
                    {
                        decimal red = img.GetPixel(point.X, point.Y).R;
                        decimal green = img.GetPixel(point.X, point.Y).G;
                        decimal blue = img.GetPixel(point.X, point.Y).B;

                        decimal redval = (red - (blue + green) / 2) / 255;
                        if (redval < 0) redval = 0;

                        redness.Add(redval);
                    }
                }
            }
            catch (Exception)
            {
                return new List<decimal>();
            }

            return redness;
        }

        public LinePoint GetClosestPoint(Point currentlocation)
        {
            return new List<LinePoint>(Points.OrderBy(point => Distance(point, currentlocation))).First();
        }

        public Tuple<LinePoint, double> GetClosestPointWithDistance(Point currentlocation)
        {
            var points = Points.ToList();
            LinePoint p = new List<LinePoint>(points.OrderBy(point => Distance(point, currentlocation))).First();
            double d = Distance(p, currentlocation);
            return new Tuple<LinePoint, double>(p, d);
        }

        public double Distance(LinePoint source, Point target)
        {
            return Math.Pow(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2), 0.5);
        }

        public List<decimal> CalculateLineGrowth()
        {
            List<decimal> indices = new List<decimal>();
            indices.Add(0);
            CalculateDotIndices();
            indices.AddRange(DotIndex.Select(i => (decimal)i));
            return indices;
        }

        public void AddLine(Line line)
        {
            Lines.Add(line);
            RecalculatePoints();
        }

        public void RemoveLine(Line line)
        {
            Lines.Remove(line);
            RecalculatePoints();
        }

        public void RecalculatePoints()
        {
            List<LinePoint> pointList = new List<LinePoint>();
            LinePoint last = null;
            var points = new List<Point>();
            foreach (Line l in Lines.OrderBy(x => x.LineIndex))
            {
                points.AddRange(l.GetPoints(l.X1, l.Y1, l.X2, l.Y2));
            }

            foreach (Point p in points)
            {
                LinePoint lp = new LinePoint() { X = (int)p.X, Y = (int)p.Y, Index = points.IndexOf(p), ParentCombinedLine = this };
                if (!lp.Equals(last))
                {
                    pointList.Add(lp);
                    last = lp;
                }
            }

            Points = pointList;
        }

        public void AddDot(Dot dot)
        {
            Dots.Add(dot);
            CalculateDotIndices();
        }

        public void RemoveDot(Dot dot)
        {
            Dots.Remove(dot);
            CalculateDotIndices();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            CombinedLine l = obj as CombinedLine;
            if (l.ID == ID)
            {
                return true;
            }
            return false;
        }

        internal bool ContainsDotAt(Point point)
        {
            foreach (Dot d in Dots)
            {
                if (d.Location == point)
                    return true;
            }
            return false;
        }

        public void CalculateDotIndices()
        {
            if (Points == null) RecalculatePoints();
            List<LinePoint> linepoints = new List<LinePoint>();
            try
            {
                linepoints = Dots.Where(d => d.DotType != "Non-counting mark").Select(d => Points.Find(x => x.Location == d.Location)).OrderBy(x => x.Index).ToList();
                if (linepoints.Count != Dots.Count(d => d.DotType != "Non-counting mark"))
                {
                    //snap dots to the closest location on the line
                    foreach (var dot in Dots.Where(d => d.DotType != "Non-counting mark"))
                    {
                        dot.Location = GetClosestPoint(dot.Location).Location;
                    }
                    linepoints = Dots.Where(d => d.DotType != "Non-counting mark").Select(d => Points.Find(x => x.Location == d.Location)).OrderBy(x => x.Index).ToList();
                }
                for (int i = 0; i < linepoints.Count; i++)
                {
                    var dot = Dots.FirstOrDefault(x => x.Location == linepoints[i].Location && x.DotType != "Non-counting mark");
                    if (dot != null) dot.DotIndex = i + 1;
                }
                List<int> indices = Dots.Where(d => d.DotType != "Non-counting mark").Select(d => Points.Find(x => x.Location == d.Location)).ToList().Select(lp => Points.FindIndex(x => x.Location == lp.Location)).ToList();
                indices.Sort();
                DotIndex = indices;
                Dots = Dots.OrderBy(x => x.DotIndex).ToList();
            }
            catch (Exception e)
            {
                //not possible to calculate
            }

        }
    }
    [Serializable]

    public class LinePoint
    {
        public int X { get; set; }

        public int Y { get; set; }
        public int Index { get; set; }

        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
        }

        public CombinedLine ParentCombinedLine { get; set; }

        public bool Equals(LinePoint p)
        {
            if (p == null)
            {
                return false;
            }
            return (X == p.X) && (Y == p.Y);
        }
    }
}
