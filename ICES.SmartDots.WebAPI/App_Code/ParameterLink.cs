using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public partial class ParameterLink
    {
        public Guid Id { get; set; }
        public Guid ParameterId { get; set; }
        public Guid SampleSetId { get; set; }
        public Guid? AnalysisId { get; set; }
        public int? Quantity { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Analysis Analysis { get; set; }
        public virtual Parameter Parameter { get; set; }
        public virtual SampleSet SampleSet { get; set; }
    }
}