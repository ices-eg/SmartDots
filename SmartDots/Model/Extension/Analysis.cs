namespace SmartDots.Model
{
    using System;
    using System.Collections.Generic;

    public class Analysis
    {
        public Guid ID { get; set; }
        public int Number { get; set; }
        public Folder Folder { get; set; }
        public List<AnalysisParameter> AnalysisParameters { get; set; }
        public string HeaderInfo { get; set; }
        public bool UserCanPin { get; set; }


    }
}
