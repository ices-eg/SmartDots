using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public partial class Quality
    {
        public Quality()
        {
            Outcomes = new HashSet<Outcome>();
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public string HexColor { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Outcome> Outcomes { get; set; }
//        public virtual Product Product { get; set; }
    }
}