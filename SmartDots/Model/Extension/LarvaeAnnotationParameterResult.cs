using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeAnnotationParameterResult
    {
        public Guid ID { get; set; }
        public Guid AnnotationID { get; set; }
        public Guid ParameterID { get; set; }
        public Guid FileID { get; set; }
        public int Result { get; set; }
        public string CalculatedResult { get; set; }
        public bool IsVisible { get; set; } = true;
        public string MultiUserColor { get; set; }

        public string User
        {
            get
            {
                return Annotation?.User;
            }
        }

        public LarvaeAnnotation Annotation { get; set; }
        public LarvaeFile File { get; set; }
        public LarvaeParameter Parameter { get; set; }
        public List<LarvaeLine> Lines { get; set; } = new List<LarvaeLine>();
        public List<LarvaeDot> Dots { get; set; } = new List<LarvaeDot>();
        public LarvaeCircle Circle { get; set; } = null;

        public string FileName
        {
            get
            {
                return File?.Name;
            }
        }

        public string ParameterName
        {
            get
            {
                return Parameter?.Name;
            }
        }

        public string ParameterColor
        {
            get
            {
                return Parameter?.Color;
            }
        }

    }
}
