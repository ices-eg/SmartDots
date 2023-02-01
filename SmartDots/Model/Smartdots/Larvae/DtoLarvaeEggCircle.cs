using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeEggCircle
    {
        public Guid ID { get; set; }
        public Guid AnnotationParameterResultID { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
    }
}
