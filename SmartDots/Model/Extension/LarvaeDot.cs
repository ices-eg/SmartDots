using System;
using System.Drawing;
using Point = System.Windows.Point;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeDot
    {
        public System.Guid ID { get; set; }
        public System.Guid AnnotationParameterResultID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public LarvaeAnnotationParameterResult LarvaeAnnotationParameterResult { get; set; }


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


    }
}
