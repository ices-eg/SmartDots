using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class LarvaeFile
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
        public string ThumbnailPath { get; set; }
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public bool? CanApprove { get; set; }
        public decimal? Scale { get; set; }
        public List<DtoAnnotation> Annotations { get; set; }
    }
}
