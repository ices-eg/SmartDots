using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoFile
    {
        public Guid ID { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public string DisplayName { get; set; }
        public string SampleNumber { get; set; } 
        public Guid? SampleID { get; set; }
        public int AnnotationCount { get; set; }
        public bool IsReadOnly { get; set; }
        public virtual ICollection<DtoAnnotation> Annotations { get; set; }
        public virtual DtoSample Sample { get; set; }
        public decimal? Scale { get; set; }
        public bool? CanApprove { get; set; } // only prevents approval when false
        public dynamic SampleProperties { get; set; }


    }
}
