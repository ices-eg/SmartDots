using System;

namespace SmartDots.Model
{

    public class DtoLine
    {
        public Guid ID { get; set; }
        public Guid AnnotationID { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
        public string Color { get; set; }
        public int LineIndex { get; set; }
    }
}

