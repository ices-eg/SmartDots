using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Status
    {
        public Status()
        {
            Analyses = new HashSet<Analysis>();
            Samples = new HashSet<Sample>();
            SampleSets = new HashSet<SampleSet>();
            Services = new HashSet<Service>();
            StatusLogs = new HashSet<StatusLog>();
        }

        public Guid Id { get; set; }
        public Guid? EntityId { get; set; }
        public int? LabId { get; set; }
        public int Rank { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public string HexColor { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Analysis> Analyses { get; set; }
        public virtual ICollection<Sample> Samples { get; set; }
        public virtual ICollection<SampleSet> SampleSets { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<StatusLog> StatusLogs { get; set; }
    }
}
