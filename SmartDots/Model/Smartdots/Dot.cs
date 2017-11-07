using System;

namespace SmartDots.Model
{
    public class DtoDot
    {
        public Guid ID { get; set; }
        public Guid AnnotationID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int DotIndex { get; set; }
        public string Color { get; set; }
        public string DotShape { get; set; }
        public string DotType { get; set; }
    }
}

