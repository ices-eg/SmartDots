using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
    public class DtoSample
    {

        public Guid Id { get; set; }
        public string StatusCode { get; set; }
        public string StatusColor { get; set; }
        public int StatusRank { get; set; }
        public Dictionary<string, string> DisplayedProperties { get; set; }
    }
}
