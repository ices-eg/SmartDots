using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoLarvaeEggAnalysis
    {
        public Guid ID { get; set; }
        public string HeaderInfo { get; set; }
        public string Type { get; set; }
        public bool AllowSetScale { get; set; }
        public List<DtoLarvaeEggSample> LarvaeSamples { get; set; }

        public List<DtoLarvaeEggParameter> LarvaeParameters { get; set; }
    }
}
