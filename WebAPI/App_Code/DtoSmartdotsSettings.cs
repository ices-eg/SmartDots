using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
   public class DtoSmartdotsSettings
   {
      public bool CanAttachDetachSample { get; set; }
      public bool CanBrowseFolder { get; set; }
      public bool UseSampleStatus { get; set; }
      public bool CanApproveAnnotation { get; set; }
      public bool RequireAQ1ForApproval { get; set; }
      public bool AutoMeasureScale { get; set; }
      public bool CanMarkEventAsCompleted { get; set; }
      public float MinRequiredVersion { get; set; } = 1.5f;

   }
}