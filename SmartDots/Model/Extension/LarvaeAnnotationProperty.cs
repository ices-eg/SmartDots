using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeAnnotationProperty
    {
        public string Label { get; set; }
        public string Property { get; set; }
        public string VocabCode { get; set; }
        public bool IsRequired { get; set; }
    }
}
