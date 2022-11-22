using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoMaturitySample
    {
        public Guid ID { get; set; }
        public string StatusCode { get; set; }
        public string StatusColor { get; set; }
        public int StatusRank { get; set; }
        public Dictionary<string, string> SampleProperties { get; set; }
        public Dictionary<string, string> AnnotationProperties { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowApproveToggle { get; set; }
        public bool UserHasApproved { get; set; }


        public List<DtoMaturityFile> Files { get; set; }
        public List<DtoMaturityAnnotation> Annotations { get; set; }

    }
}
