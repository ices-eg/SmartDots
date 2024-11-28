using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class AnalysisType
    {
        public AnalysisType()
        {
            Analysis = new HashSet<Analysis>();
        }

        public Guid Id { get; set; }
        public int LabId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Analysis> Analysis { get; set; }
    }
}
