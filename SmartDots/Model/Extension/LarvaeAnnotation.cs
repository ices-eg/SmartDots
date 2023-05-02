using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeAnnotation
    {
        public Guid ID { get; set; }
        public Guid LarvaeEggSampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        //public Guid? SpeciesID { get; set; }
        //public string Species { get; set; }
        //public Guid? QualityID { get; set; }
        //public string Quality { get; set; }
        //public Guid? AnalFinPresenceID { get; set; }
        //public string AnalFinPresence { get; set; }
        //public Guid? PelvicFinPresenceID { get; set; }
        //public string PelvicFinPresence { get; set; }
        //public Guid? DorsalFinPresenceID { get; set; }
        //public string DorsalFinPresence { get; set; }
        //public Guid? DevelopmentStageID { get; set; }
        //public string DevelopmentStage { get; set; }
        //public Guid? EmbryoPresenceID { get; set; }
        //public string EmbryoPresence { get; set; }
        //public Guid? EmbryoSizeID { get; set; }
        //public string EmbryoSize { get; set; }
        //public Guid? YolkSegmentationID { get; set; }
        //public string YolkSegmentation { get; set; }
        //public Guid? OilGlobulePresenceID { get; set; }
        //public string OilGlobulePresence { get; set; }

        public Dictionary<string, Guid?> PropertyValues = new Dictionary<string, Guid?>();

        public string Comments { get; set; }
        public bool RequiresSaving { get; set; }

        public bool IsApproved { get; set; }

        public LarvaeSample LarvaeSample { get; set; }
        public List<LarvaeAnnotationParameterResult> LarvaeAnnotationParameterResult { get; set; }

        public string ApprovedPicture
        {
            get
            {
                if (IsApproved)
                {
                    return "/SmartDots;component/Resources/ok-16.png";
                }
                return null;
            }
        }

    }
}
