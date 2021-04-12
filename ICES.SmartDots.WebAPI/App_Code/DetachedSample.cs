using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class DetachedSample
    {
        public Guid Id { get; set; }
        public Guid AnalysisId { get; set; }
        public Guid SampleId { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Analysis Analysis { get; set; }
        public virtual Sample Sample { get; set; }
    }
}
