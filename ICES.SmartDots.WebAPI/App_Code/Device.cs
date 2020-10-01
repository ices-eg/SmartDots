using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Device
    {
        public Device()
        {
            Preparation = new HashSet<Preparation>();
        }

        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Preparation> Preparation { get; set; }
        public virtual Task Task { get; set; }
    }
}
