using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public class DtoReadabilityQuality
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
    }
}
