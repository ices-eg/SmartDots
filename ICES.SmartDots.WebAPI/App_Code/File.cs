using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
    public partial class File
    {
        public File()
        {
            DbAnnotations = new HashSet<Outcome>();
        }

        public Guid Id { get; set; }
        public Guid? FolderId { get; set; }
        public string Filename { get; set; }
        public long? SampleNumber { get; set; }
        public Guid? SampleId { get; set; }
        public DateTime DateCreation { get; set; }
        public decimal? Scale { get; set; }
        public bool Gcrecord { get; set; }
        [NotMapped]
        public int AnnotationCount { get; set; }

        public virtual ICollection<Outcome> DbAnnotations { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual Sample Sample { get; set; }
    }
}
