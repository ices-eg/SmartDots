using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class StatusLog
    {
        public Guid Id { get; set; }
        public Guid StatusId { get; set; }
        public Guid RecordId { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Status Status { get; set; }
    }
}
