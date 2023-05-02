using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public partial class DtoLarvaeEggAnnotation
    {
        public Guid ID { get; set; }
        public Guid LarvaeEggSampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        //public Guid? SpeciesID { get; set; }
        //public Guid? QualityID { get; set; }
        //public Guid? AnalFinPresenceID { get; set; }
        //public Guid? PelvicFinPresenceID { get; set; }
        //public Guid? DorsalFinPresenceID { get; set; }
        //public Guid? DevelopmentStageID { get; set; }
        //public Guid? EmbryoPresenceID { get; set; }
        //public Guid? EmbryoSizeID { get; set; }
        //public Guid? YolkSegmentationID { get; set; }
        //public Guid? OilGlobulePresenceID { get; set; }
        public Dictionary<string,Guid?> PropertyValues = new Dictionary<string, Guid?>();
        public bool IsApproved { get; set; }
        public string Comments { get; set; }

        public List<DtoLarvaeEggAnnotationParameterResult> AnnotationParameterResult { get; set; } = new List<DtoLarvaeEggAnnotationParameterResult>();

    }
}
