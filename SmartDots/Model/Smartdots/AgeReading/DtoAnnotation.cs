using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoAnnotation
    {
        public Guid ID { get; set; }
        public Guid? ParameterID { get; set; }
        public Guid FileID { get; set; }
        public Guid? SampleId { get; set; }
        public Guid? AnalysisId { get; set; }
        public Guid? QualityID { get; set; }
        public int Result { get; set; }
        public DateTime DateCreation { get; set; }
        public Guid? LabTechnicianID { get; set; }
        public string LabTechnician { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsFixed { get; set; }
        public string Nucleus { get; set; }
        public string Edge { get; set; }
        public string Comment { get; set; }
        public virtual ICollection<DtoDot> Dots { get; set; }
        public virtual ICollection<DtoLine> Lines { get; set; }
        public List<DtoAnnotationProperty> DynamicProperties { get; set; }
    }
}
