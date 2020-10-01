using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public class DtoDot
    {
        public Guid Id { get; set; }
        public Guid AnnotationId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int DotIndex { get; set; }
        public string Color { get; set; }
        public string DotShape { get; set; }
        public string DotType { get; set; }
    }
}
