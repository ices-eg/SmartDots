using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Entity
    {
        public Entity()
        {
            Property = new HashSet<Property>();
            SampleState = new HashSet<SampleState>();
        }

        public Guid Id { get; set; }
        public Guid? ParentEntityId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public string NavigationGroup { get; set; }
        public int? Order { get; set; }
        public string Glyph { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Property> Property { get; set; }
        public virtual ICollection<SampleState> SampleState { get; set; }
        public virtual Entity ParentEntity { get; set; }
        public virtual ICollection<Entity> InverseParentEntity { get; set; }
    }
}
