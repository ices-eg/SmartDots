using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoLarvaeEggSample
    {
        public Guid ID { get; set; }
        public string StatusCode { get; set; }
        public string StatusColor { get; set; }
        public int StatusRank { get; set; }
        public Dictionary<string, object> SampleProperties { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowApproveToggle { get; set; }
        public bool UserHasApproved { get; set; }

        public List<DtoLarvaeEggFile> Files { get; set; }
        public List<DtoLarvaeEggAnnotation> Annotations { get; set; }
    }
}
