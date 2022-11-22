using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class MaturityAnnotation
    {
        public Guid ID { get; set; }
        public Guid MaturitySampleID { get; set; }
        public Guid UserID { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public Guid? SexID { get; set; }
        public string Sex { get; set; }
        public Guid? MaturityID { get; set; }
        public string Maturity { get; set; }
        public Guid? MaturityQualityID { get; set; }
        public string MaturityQuality { get; set; }
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
