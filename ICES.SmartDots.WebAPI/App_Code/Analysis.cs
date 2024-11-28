using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
    public partial class Analysis
    {
        public Analysis()
        {
            Outcomes = new HashSet<Outcome>();
            ParameterLinks = new HashSet<ParameterLink>();
        }

        public Guid Id { get; set; }
        public Guid? SampleSetId { get; set; }
        public Guid? AnalysisTypeId { get; set; }
        public Guid? ProcessingMethodId { get; set; }
        public Guid? SampleStateId { get; set; }
        public int Number { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateDue { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public Guid? StatusId { get; set; }
        public bool Gcrecord { get; set; }
        [NotMapped]
        public Folder Folder { get; set; }
        [NotMapped]
        public List<Parameter> Parameters { get; set; }
        [NotMapped]
        public string HeaderInfo { get; set; }

        public virtual ICollection<Outcome> Outcomes { get; set; }
        public virtual ICollection<ParameterLink> ParameterLinks { get; set; }
        public virtual SampleSet SampleSet { get; set; }
        public virtual Status Status { get; set; }
    }
}
