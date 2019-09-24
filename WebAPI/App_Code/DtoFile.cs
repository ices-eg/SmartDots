using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
   public class DtoFile
   {
      public Guid Id { get; set; }
      public string Filename { get; set; }
      public string DisplayName { get; set; }
      public string SampleNumber { get; set; }
      public Guid? SampleId { get; set; }
      public int AnnotationCount { get; set; }
      public bool IsReadOnly { get; set; }
      public decimal? Scale { get; set; }
      public List<DtoAnnotation> Annotations { get; set; }
      public DtoSample Sample { get; set; }
   }
}
