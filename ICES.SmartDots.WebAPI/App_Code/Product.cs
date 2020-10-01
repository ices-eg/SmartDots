using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public partial class Product
    {
        public Product()
        {
            Parameter = new HashSet<Parameter>();
            ProcessingMethod = new HashSet<ProcessingMethod>();
            ProductLab = new HashSet<ProductLab>();
            Quality = new HashSet<Quality>();
            SampleSet = new HashSet<SampleSet>();
            SampleState = new HashSet<SampleState>();
            SampleType = new HashSet<SampleType>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Parameter> Parameter { get; set; }
        public virtual ICollection<ProcessingMethod> ProcessingMethod { get; set; }
        public virtual ICollection<ProductLab> ProductLab { get; set; }
        public virtual ICollection<Quality> Quality { get; set; }
        public virtual ICollection<SampleSet> SampleSet { get; set; }
        public virtual ICollection<SampleState> SampleState { get; set; }
        public virtual ICollection<SampleType> SampleType { get; set; }
    }
}