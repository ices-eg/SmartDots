using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Project
    {
        public Project()
        {
            ProjectType = new HashSet<ProjectType>();
        }

        public Guid Id { get; set; }
        public int LabId { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsSynced { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<ProjectType> ProjectType { get; set; }
    }
}
