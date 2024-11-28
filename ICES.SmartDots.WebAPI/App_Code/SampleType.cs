using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class SampleType
    {
        public SampleType()
        {
            Samples = new HashSet<Sample>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Sample> Samples { get; set; }
    }
}
