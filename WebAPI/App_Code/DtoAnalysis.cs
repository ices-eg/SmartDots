using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.App_Code
{
   public class DtoAnalysis
   {
      public Guid Id { get; set; }
      public int Number { get; set; }
      public DtoFolder Folder { get; set; }
      public List<DtoAnalysisParameter> AnalysisParameters { get; set; }
      public string HeaderInfo { get; set; }
      public bool UserCanPin { get; set; }

      public bool ShowEdgeColumn { get; set; }
      public bool ShowNucleusColumn { get; set; }

   }
}
