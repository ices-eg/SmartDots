using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class ProcessingMethod
    {
        public ProcessingMethod()
        {
            Analysis = new HashSet<Analysis>();
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Analysis> Analysis { get; set; }
        public virtual Product Product { get; set; }
    }
}
