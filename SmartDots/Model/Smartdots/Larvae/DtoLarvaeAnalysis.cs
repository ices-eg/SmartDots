using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoLarvaeAnalysis
    {
        public Guid ID { get; set; }
        public string HeaderInfo { get; set; }
        public List<DtoLarvaeSample> LarvaeSamples { get; set; }

        public List<DtoLarvaeParameter> LarvaeParameters { get; set; }
    }
}
