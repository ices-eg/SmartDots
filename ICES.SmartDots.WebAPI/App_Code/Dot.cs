using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public partial class Dot
    {
        public Guid Id { get; set; }
        public Guid OutcomeId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public string Color { get; set; }
        public bool IsCross { get; set; }
        public string Shape { get; set; }
        public string WaterType { get; set; }
        public bool Gcrecord { get; set; }
        public virtual Outcome Outcome { get; set; }
    }
}