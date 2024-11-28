using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class SampleSet
    {
        public SampleSet()
        {
            Analyses = new HashSet<Analysis>();
            ParameterLinks = new HashSet<ParameterLink>();
            Samples = new HashSet<Sample>();
        }

        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SampleStateId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? EstimatedDateDelivery { get; set; }
        public DateTime? DateDelivery { get; set; }
        public string LocationReceipt { get; set; }
        public string LocationArchive { get; set; }
        public Guid? StatusId { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Analysis> Analyses { get; set; }
        public virtual ICollection<ParameterLink> ParameterLinks { get; set; }
        public virtual ICollection<Sample> Samples { get; set; }
        public virtual Product Product { get; set; }
        public virtual SampleState SampleState { get; set; }
        public virtual Service Service { get; set; }
        public virtual Status Status { get; set; }
    }
}
