using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class DtoAnalysis
    {
        public Guid ID { get; set; }
        public int Number { get; set; }
        public Folder Folder { get; set; }
        public List<DtoAnalysisParameter> AnalysisParameters { get; set; }
        public string HeaderInfo { get; set; }
        public bool UserCanPin { get; set; }
    }
}
