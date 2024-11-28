using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.App_Code
{
    public partial class GetAnalysisSample_Result
    {
        public System.Guid ID { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Duplicate_state { get; set; }
        public long No { get; set; }
        public string No_ext_ { get; set; }
        public string Description { get; set; }
        public string Pos__receipt { get; set; }
        public string Pos__processing { get; set; }
        public string Pos__archive { get; set; }
        public string Status { get; set; }
    }
}
