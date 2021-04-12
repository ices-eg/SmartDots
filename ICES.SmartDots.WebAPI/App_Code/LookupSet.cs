using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class LookupSet
    {
        public LookupSet()
        {
            LookupSetValue = new HashSet<LookupSetValue>();
            Property = new HashSet<Property>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<LookupSetValue> LookupSetValue { get; set; }
        public virtual ICollection<Property> Property { get; set; }
    }
}
