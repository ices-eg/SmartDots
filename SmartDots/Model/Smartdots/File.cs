using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoFile
    {
        public Guid ID { get; set; }
        public string Filename { get; set; }
        public long? SampleNumber { get; set; } //ilvo only
        public Guid? SampleID { get; set; }
        public int AnnotationCount { get; set; }
        public bool IsReadOnly { get; set; }
        public virtual ICollection<DtoAnnotation> Annotations { get; set; }
        public virtual DtoSample Sample { get; set; }
        public decimal? Scale { get; set; }

    }
}
