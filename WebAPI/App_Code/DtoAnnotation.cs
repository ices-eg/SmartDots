using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
   public class DtoAnnotation
   {
      public Guid Id { get; set; }
      public Guid? ParameterId { get; set; }
      public Guid FileId { get; set; }
      public Guid? QualityId { get; set; }
      public DateTime DateCreation { get; set; }
      public Guid? LabTechnicianId { get; set; }
      public string LabTechnician { get; set; }
      public int Result { get; set; }
      public bool IsApproved { get; set; }
      public bool IsReadOnly { get; set; }
      public bool IsFixed { get; set; }
      public string Comment { get; set; }
      public List<DtoDot> Dots { get; set; }
      public List<DtoLine> Lines { get; set; }
      // New on this API version
      public string Edge { get; set; }
      public string Nucleus { get; set; }
   }
}

