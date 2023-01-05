using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeAnnotation
    {
        public Guid ID { get; set; }
        public Guid LarvaeSampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public Guid? LarvaeQualityID { get; set; }
        public Guid? AnalFinPresenceID { get; set; }
        public Guid? PelvicFinPresenceID { get; set; }
        public Guid? DorsalFinPresenceID { get; set; }
        public Guid? DevelopmentStageID { get; set; }
        public bool IsApproved { get; set; }
        public string Comments { get; set; }

        public List<DtoLarvaeAnnotationParameterResult> AnnotationParameterResult { get; set; } = new List<DtoLarvaeAnnotationParameterResult>();

    }
}
