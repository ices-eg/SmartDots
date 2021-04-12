using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Client
    {
        public Client()
        {
            Service = new HashSet<Service>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Service> Service { get; set; }
    }
}
