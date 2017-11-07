using System;
using System.Collections.Generic;
using System.Text;

namespace AgeReading.Graph
{
    public struct GraphBounds
    {
        public double MinX, MaxX, MinY, MaxY;

        public override string ToString()
        {
            return string.Format("X: {0} .. {1}; Y: {2} .. {3}", MinX, MaxX, MinY, MaxY);
        }

        public GraphBounds(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public static GraphBounds CreateInitial()
        {
            return new GraphBounds(double.MaxValue, double.MinValue, double.MaxValue, double.MinValue);
        }

        public double DeltaX { get { return MaxX - MinX; } }
        public double DeltaY { get { return MaxY - MinY; } }

        public void Update(double newX, double newY)
        {
            MinX = Math.Min(MinX, newX);
            MaxX = Math.Max(MaxX, newX);
            MinY = Math.Min(MinY, newY);
            MaxY = Math.Max(MaxY, newY);
        }

        public static GraphBounds Join(GraphBounds left, GraphBounds right)
        {
            return new GraphBounds(
                Math.Min(left.MinX, right.MinX),
                Math.Max(left.MaxX, right.MaxX),
                Math.Min(left.MinY, right.MinY),
                Math.Max(left.MaxY, right.MaxY));
        }

        public double FitX(double value)
        {
            if (value < MinX)
                value = MinX;
            if (value > MaxX)
                value = MaxX;
            return value;
        }

        public double FitY(double value)
        {
            if (value < MinY)
                value = MinY;
            if (value > MaxY)
                value = MaxY;
            return value;
        }
    }
}
