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
        public Guid? SexID { get; set; }
        public Guid? LarvaeID { get; set; }
        public Guid? LarvaeQualityID { get; set; }
        public bool IsApproved { get; set; }
        public string Comments { get; set; }

    }
}
