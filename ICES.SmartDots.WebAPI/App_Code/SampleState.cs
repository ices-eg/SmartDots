using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class SampleState
    {
        public SampleState()
        {
            Analyses = new HashSet<Analysis>();
            SampleDuplicateSampleStates = new HashSet<Sample>();
            SampleSampleStates = new HashSet<Sample>();
            SampleSets = new HashSet<SampleSet>();
        }

        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Analysis> Analyses { get; set; }
        public virtual ICollection<Sample> SampleDuplicateSampleStates { get; set; }
        public virtual ICollection<Sample> SampleSampleStates { get; set; }
        public virtual ICollection<SampleSet> SampleSets { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Product Product { get; set; }
    }
}
