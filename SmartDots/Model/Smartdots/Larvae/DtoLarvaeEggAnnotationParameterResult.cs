using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeEggAnnotationParameterResult
    {
        public Guid ID { get; set; }
        public Guid AnnotationID { get; set; }
        public Guid ParameterID { get; set; }
        public Guid FileID { get; set; }
        public int Result { get; set; }

        public List<DtoLarvaeEggDot> Dots { get; set; } = new List<DtoLarvaeEggDot>();
        public List<DtoLarvaeEggLine> Lines { get; set; } = new List<DtoLarvaeEggLine>();
        public DtoLarvaeEggCircle Circle { get; set; } = null;

    }
}
