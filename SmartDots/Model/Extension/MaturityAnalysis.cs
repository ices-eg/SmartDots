using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class MaturityAnalysis
    {
        public Guid ID { get; set; }
        public string HeaderInfo { get; set; }
        public List<MaturitySample> MaturitySamples { get; set; }


    }
}
