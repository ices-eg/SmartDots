using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class LarvaeAnalysis
    {
        public Guid ID { get; set; }
        public string HeaderInfo { get; set; }
        public List<LarvaeSample> LarvaeSamples { get; set; }


    }
}
