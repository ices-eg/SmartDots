using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public partial class Outcome
    {
        public Outcome()
        {
            Dots = new HashSet<Dot>();
            Lines = new HashSet<Line>();
        }

        public Guid Id { get; set; }
        public Guid? AnalysisId { get; set; }
        public Guid? SampleId { get; set; }
        public Guid? ParameterId { get; set; }
        public Guid FileId { get; set; }
        public Guid? QualityId { get; set; }
        public decimal Result { get; set; }
        public DateTime DateCreation { get; set; }
        public Guid? UserId { get; set; }
        [NotMapped]
        public string LabTechnician { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public bool IsFixed { get; set; }
        public bool Gcrecord { get; set; }

        public virtual ICollection<Dot> Dots { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
//        public virtual Analysis Analysis { get; set; }
//        public virtual File File { get; set; }
//        public virtual Parameter Parameter { get; set; }
        public virtual Quality Quality { get; set; }
//        public virtual Sample Sample { get; set; }
    }
}