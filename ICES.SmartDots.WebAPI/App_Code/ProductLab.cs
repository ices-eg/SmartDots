using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class ProductLab
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int LabId { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Product Product { get; set; }
    }
}
