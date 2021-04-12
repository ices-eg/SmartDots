using System;
using System.Collections.Generic;

namespace WebAPI.App_Code
{
    public partial class Preparation
    {
        public Guid Id { get; set; }
        public Guid AnalysisId { get; set; }
        public Guid TaskId { get; set; }
        public Guid DeviceId { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreation { get; set; }
        public Guid? UserId { get; set; }
        public bool Gcrecord { get; set; }

        public virtual Analysis Analysis { get; set; }
        public virtual Device Device { get; set; }
        public virtual Task Task { get; set; }
    }
}
