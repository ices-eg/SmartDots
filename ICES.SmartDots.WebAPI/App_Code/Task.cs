using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Task
    {
        public Task()
        {
            Device = new HashSet<Device>();
            Preparation = new HashSet<Preparation>();
        }

        public Guid Id { get; set; }
        public int LabId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Device> Device { get; set; }
        public virtual ICollection<Preparation> Preparation { get; set; }
    }
}
