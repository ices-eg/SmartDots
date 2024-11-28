using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class LookupSetValue
    {
        public Guid Id { get; set; }
        public Guid LookupSetId { get; set; }
        public string Code { get; set; }
        public string CodeExt { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual LookupSet LookupSet { get; set; }
    }
}
