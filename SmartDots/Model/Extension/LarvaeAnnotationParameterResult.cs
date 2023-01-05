using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class LarvaeAnnotationParameterResult
    {
        public Guid ID { get; set; }
        public Guid LarvaeAnnotationID { get; set; }
        public Guid LarvaeParameterID { get; set; }
        public Guid LarvaeFileID { get; set; }
        public int Result { get; set; }

        public LarvaeFile File { get; set; }
        public LarvaeParameter Parameter { get; set; }
        public List<LarvaeLine> Lines { get; set; } = new List<LarvaeLine>();
        public List<LarvaeDot> Dots { get; set; } = new List<LarvaeDot>();

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
