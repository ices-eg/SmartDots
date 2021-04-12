using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class PropertyValue
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid RecordId { get; set; }
        public string Value { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Property Property { get; set; }
    }
}
