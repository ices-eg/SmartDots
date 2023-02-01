using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeEggDot
    {
        public Guid ID { get; set; }
        public Guid AnnotationParameterResultID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
    }
}
