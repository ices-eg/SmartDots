using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class AnalysisMethod
    {
        public AnalysisMethod()
        {
            Parameter = new HashSet<Parameter>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Parameter> Parameter { get; set; }
    }
}
