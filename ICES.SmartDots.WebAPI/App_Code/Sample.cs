using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
    public partial class Sample
    {
        public Sample()
        {
            DetachedSamples = new HashSet<DetachedSample>();
            //File = new HashSet<File>();
            Outcomes = new HashSet<Outcome>();
        }

        public Guid Id { get; set; }
        public Guid SampleSetId { get; set; }
        public Guid? SampleTypeId { get; set; }
        public Guid? SampleStateId { get; set; }
        public Guid? DuplicateSampleStateId { get; set; }
        public long Number { get; set; }
        public string NumberExternal { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string PositionReceipt { get; set; }
        public string PositionProcessing { get; set; }
        public string PositionArchive { get; set; }
        public Guid? StatusId { get; set; }
        public bool Gcrecord { get; set; }
        [NotMapped]
        public Dictionary<string, string> DisplayedProperties { get; set; }

        public virtual ICollection<DetachedSample> DetachedSamples { get; set; }
        //public virtual ICollection<File> File { get; set; }
        public virtual ICollection<Outcome> Outcomes { get; set; }
        public virtual SampleState DuplicateSampleState { get; set; }
        public virtual SampleSet SampleSet { get; set; }
        public virtual SampleState SampleState { get; set; }
        public virtual SampleType SampleType { get; set; }
        public virtual Status Status { get; set; }
    }
}
