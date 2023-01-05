using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class LarvaeAnnotation
    {
        public Guid ID { get; set; }
        public Guid LarvaeSampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public Guid? LarvaeQualityID { get; set; }
        public string LarvaeQuality { get; set; }
        public Guid? AnalFinPresenceID { get; set; }
        public string AnalFinPresence { get; set; }
        public Guid? PelvicFinPresenceID { get; set; }
        public string PelvicFinPresence { get; set; }
        public Guid? DorsalFinPresenceID { get; set; }
        public string DorsalFinPresence { get; set; }
        public Guid? DevelopmentStageID { get; set; }
        public string DevelopmentStage { get; set; }

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
