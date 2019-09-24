using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Parameter
    {
        public Parameter()
        {
            Outcomes = new HashSet<Outcome>();
            ParameterLinks = new HashSet<ParameterLink>();
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid AnalysisMethodId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Measure { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Outcome> Outcomes { get; set; }
        public virtual ICollection<ParameterLink> ParameterLinks { get; set; }
        public virtual AnalysisMethod AnalysisMethod { get; set; }
        public virtual Product Product { get; set; }
    }
}
