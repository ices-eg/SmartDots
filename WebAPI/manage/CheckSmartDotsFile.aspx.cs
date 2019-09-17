using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Configuration;
using WebInterface.App_Code;
using System.Threading.Tasks;

namespace Webinterface.manage
{
    public partial class CheckSmartDotsFile : System.Web.UI.Page
    {
        private Uri _serviceUri = new Uri("http://datsu.ices.dk/DatsuRest/");
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void bttCheckFile_Click(object sender, EventArgs e)
        {

            if (fileUpload.HasFile)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    //                    this.lbMsg.Text = "";
                    sw.Start();
                    using (var client = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            string request = "{FileName:'" + fileUpload.FileName + "', EmailAddress:'" + Session["email"].ToString() + "', DataType:'SMARTDOTS'}";
                            content.Add(new StringContent(request), "Request");
                            content.Add(new StreamContent(fileUpload.FileContent), "File", fileUpload.FileName);
                            client.BaseAddress = _serviceUri;
                            client.Timeout = TimeSpan.FromMilliseconds(24000000);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                            var clientresponse = client.PostAsync("api/ScreenFile", content).Result;
                            if (clientresponse.IsSuccessStatusCode)
                            {
                                screenResults response = clientresponse.Content.ReadAsAsync<screenResults>().Result;
                                if (!string.IsNullOrEmpty(response.SessionID))
                                {
                                    sw.Stop();
                                    lblMessage.Text = string.Format("File screened successfully.  SessionID = {0} File Size = {1}KBs Time taken = {2} secs.", response.SessionID, (double)(fileUpload.FileContent.Length / 1000), (double)(sw.ElapsedMilliseconds / 1000));
                                    hlnkResult.NavigateUrl = "viewScreenResult.aspx?sessionid=" + response.SessionID + "&groupError=0";
                                }
                                else
                                    lblMessage.Text = string.Format("Failed to screen file. Error message : " + response.ScreenResultURL);
                            }
                            else
                                lblMessage.Text = string.Format("Failed to call service. error message: {0}", clientresponse.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = string.Format("Failed to uploaded file. error message: {0}", ex.ToString());
                }
            }
            else
            {
                lblMessage.Text = string.Format("Please select a file to upload stations");
            }
            /*
            int SessionID = 0;
            WebInterface.DATSUService.Result result = null;
            try
            {
                if (fileUpload.HasFile)
                {
                    result = DATSUService.UploadFile(129, Session["email"].ToString(), fileUpload.FileName, fileUpload.FileContent);
                    SessionID = result.SessionID;

                    if (SessionID > 0)
                    {
                        Response.Redirect("viewScreenResult.aspx?SessionID=" + SessionID.ToString());
                    }
                    else
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text = "<br>The file is not valid. <br>" + result.ErrorMessage.ToString();
                    }
                }
            }

            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "<br>The file is not valid. <br>" + ex.ToString();
            }
            */
        }
    }
}