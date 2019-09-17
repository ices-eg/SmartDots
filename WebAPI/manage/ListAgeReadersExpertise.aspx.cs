using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class ListAgeReadersExpertise : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

      protected void ddlCountries_SelectedIndexChanged(object sender, EventArgs e)
      {
         hplnkDownloadExpertise.NavigateUrl = "../download/downloadListSmartusers.ashx?tblCountryID=" + ddlCountries.SelectedValue.ToString();
      }
   }
}