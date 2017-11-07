using System;
using System.Windows;

namespace SmartDots.Model
{
    [Serializable]
    public class Dot
    {
        public System.Guid ID { get; set; }
        public System.Guid AnnotationID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public string Color { get; set; }
        public string DotShape { get; set; }
        public string DotType { get; set; }
        public int DotIndex { get; set; }
        public bool IsFixed { get; set; }

        public Point Location
        {
            get
            {
                return new Point(X,Y);
            }
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
            }
        }

        public CombinedLine ParentCombinedLine { get; set; }
    }
}
