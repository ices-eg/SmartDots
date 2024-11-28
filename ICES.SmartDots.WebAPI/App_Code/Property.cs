using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Property
    {
        public Property()
        {
            PropertyValues = new HashSet<PropertyValue>();
        }

        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public int? LabId { get; set; }
        public string ColumnName { get; set; }
        public string ObjectType { get; set; }
        public string Label { get; set; }
        public string BindingDatabase { get; set; }
        public string Binding { get; set; }
        public Guid? LookupSetId { get; set; }
        public int? GridOrder { get; set; }
        public int? DetailOrder { get; set; }
        public bool IsReadOnlyIfSynced { get; set; }
        public bool IsBulkEditable { get; set; }
        public bool IsRequired { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<PropertyValue> PropertyValues { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual LookupSet LookupSet { get; set; }
    }
}
