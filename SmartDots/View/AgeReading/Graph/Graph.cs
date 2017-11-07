using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AgeReading.Graph
{
    public interface ILegendItem
    {
        Color Color { get; }
        bool Hidden { get; }
        string Hint { get; }
    }

    public class Graph
    {
        public Dictionary<double, double> Data = new Dictionary<double, double>();

        public List<KeyValuePair<double, double>> SortedData = new List<KeyValuePair<double, double>>();
        bool _SortedDataUpdatePending = true;

        public void AddPoint(double x, double y)
        {
            Data[x] = y;
            _SortedDataUpdatePending = true;
        }

        public List<KeyValuePair<double, double>> SortedPoints
        {
            get
            {
                if (_SortedDataUpdatePending)
                {
                    SortedData.Clear();
                    SortedData.AddRange(Data);
                    SortedData.Sort((left, right) => left.Key.CompareTo(right.Key));
                    _SortedDataUpdatePending = false;
                }
                return SortedData;
            }
        }

        public KeyValuePair<double, double> GetPointByIndex(int sortedPointIndex)
        {
            if (SortedPoints == null)
                throw new InvalidOperationException();
            return SortedData[sortedPointIndex];
        }

        public double GetValue(double key)
        {
            return Data[key];
        }

        public int PointCount
        {
            get
            {
                return SortedPoints.Count;
            }
        }

        public delegate double MathFunction(double x);
        

        public static Graph IterateCombinedLine(List<decimal> values)
        {
            Graph gr = new Graph();

            int number = values.Count;

            for (int i = 0; i < number; i++)
            {
                gr.AddPoint(i, (double)values[i]);
            }
            return gr;
        }

    }
}
