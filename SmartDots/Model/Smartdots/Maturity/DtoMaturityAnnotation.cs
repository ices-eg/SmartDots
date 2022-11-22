using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoMaturityAnnotation
    {
        public Guid ID { get; set; }
        public Guid MaturitySampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public Guid? SexID { get; set; }
        public Guid? MaturityID { get; set; }
        public Guid? MaturityQualityID { get; set; }
        public bool IsApproved { get; set; }
        public string Comments { get; set; }

    }
}
