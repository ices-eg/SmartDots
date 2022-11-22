using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoMaturityAnalysis
    {
        public Guid ID { get; set; }
        public string HeaderInfo { get; set; }
        public List<DtoMaturitySample> MaturitySamples { get; set; }


    }
}
