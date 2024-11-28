using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
   public class tblCode
   {
      public int tblCodeId { get; set; }
      public string Code { get; set; }
      public string Description { get; set; }
      public int ForeignId { get; set; }
      public int TblCodeGroupId { get; set; }
      public string CforeignId { get; set; }
      public string OriginDatabase { get; set; }
      public Guid Guid_Code { get; set; }
   }
}