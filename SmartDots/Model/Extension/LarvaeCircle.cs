using System;
using System.Drawing;
using Point = System.Windows.Point;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeCircle
    {
        public System.Guid ID { get; set; }
        public System.Guid AnnotationParameterResultID { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
        public LarvaeAnnotationParameterResult LarvaeAnnotationParameterResult { get; set; }


        public double Diameter
        {
            get
            {
                return Math.Pow(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2), 0.5);
            }
        }

        public int CenterX
        {
            get
            {
                return (X1 + X2)/2;
            }
        }

        public int CenterY
        {
            get
            {
                return (Y1 + Y2) / 2;
            }
        }


    }
}
