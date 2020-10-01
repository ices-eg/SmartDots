using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Service
    {
        public Service()
        {
            SampleSet = new HashSet<SampleSet>();
        }

        public Guid Id { get; set; }
        public Guid ProjectTypeId { get; set; }
        public Guid ClientId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime DateRequest { get; set; }
        public Guid? StatusId { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<SampleSet> SampleSet { get; set; }
        public virtual Client Client { get; set; }
        public virtual ProjectType ProjectType { get; set; }
        public virtual Status Status { get; set; }
    }
}
