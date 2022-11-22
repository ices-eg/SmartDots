using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoAnnotationProperty
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; }
        public dynamic Value { get; set; }
        public bool ShownInList { get; set; }
    }
}
