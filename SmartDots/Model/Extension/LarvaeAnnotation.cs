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
        public Guid? SexID { get; set; }
        public string Sex { get; set; }
        public Guid? LarvaeID { get; set; }
        public string Larvae { get; set; }
        public Guid? LarvaeQualityID { get; set; }
        public string LarvaeQuality { get; set; }
        public bool IsApproved { get; set; }
        public bool RequiresSaving { get; set; }
        public string Comments { get; set; }

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
