using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class ProjectType
    {
        public ProjectType()
        {
            Service = new HashSet<Service>();
        }

        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Service> Service { get; set; }
        public virtual Project Project { get; set; }
    }
}
