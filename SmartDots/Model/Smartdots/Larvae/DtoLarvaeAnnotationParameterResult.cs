using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeAnnotationParameterResult
    {
        public Guid ID { get; set; }
        public Guid AnnotationID { get; set; }
        public Guid ParameterID { get; set; }
        public Guid FileID { get; set; }
        public int Result { get; set; }

        public List<DtoLarvaeDot> Dots { get; set; }
        public List<DtoLarvaeLine> Lines { get; set; }

    }
}
