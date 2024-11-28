using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public class DtoLine
    {
        public Guid Id { get; set; }
        public Guid AnnotationId { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
        public string Color { get; set; }
        public int LineIndex { get; set; }
    }
}
