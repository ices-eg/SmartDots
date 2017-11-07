using System;
using System.Collections.Generic;

namespace SmartDots.Model
{
    public class Sample
    {
        public Guid Id { get; set; }
        public string StatusCode { get; set; } //ilvo only
        public string StatusColor { get; set; } //ilvo only
        public int StatusRank { get; set; } //ilvo only
        public Dictionary<string, string> DisplayedProperties { get; set; }
    }
}
