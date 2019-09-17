using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.admin
{
    public partial class CreateEvent : System.Web.UI.Page
    {
        private Uri _serviceUri = new Uri("http://datsu.ices.dk/DatsuRest/");

        protected void Page_Load(object sender, EventArgs e)
        {
            WebInterface.App_Code.RunDBOperations.validateSession();
            string strMessage = Request.QueryString["Message"];
            if (!string.IsNullOrEmpty(strMessage))
            {
                lblMessage.Visible = true;
                if (Session["message"].ToString().Length > 5)
                // lblMessage.Text = strMessage;
                {
                    lblMessage.Text = "<br>" + Session["message"].ToString();
                }
            }


            if ("notvalidated".Equals(Session["user"].ToString()))
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }
            if (RunDBOperations.checkIfUserIsCountryCoordinator(Session["user"].ToString()))
            {
                if (!IsPostBack)
                {
                    lblEmail.Text = Session["email"].ToString();

                }
            }
            else
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx?message=User does not have permission to access this page! Only coordinators can propose new events.");
            }
        }


        public bool checkIfUserIsCountryCoordinator()
        {
            if (Session["user"] != null)
            {
                //lblMessage.Visible = false;
                try
                {
                    String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                    using (SqlConnection cn = new SqlConnection(ConnectionString))
                    {
                        SqlDataAdapter da = new SqlDataAdapter();
                        SqlCommand cmd = new SqlCommand();
                        DataSet ds = new DataSet();
                        cn.Open();
                        cmd.Connection = cn;
                        cmd.CommandText = " SELECT  upper(left(dbo.tblCode.Description,1)) + lower(right(dbo.tblCode.Description,len(dbo.tblCode.Description)-1)) as Country , dbo.tblDoYouHaveAccess.SmartUser, dbo.tblDoYouHaveAccess.Name, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, dbo.tblDoYouHaveAccess.NumLogins, dbo.tblDoYouHaveAccess.LastAccess, dbo.tblDoYouHaveAccess.isCountryCoordinator, dbo.tblDoYouHaveAccess.isSMARTAdministrator FROM  dbo.tblDoYouHaveAccess INNER JOIN dbo.tblCode ON dbo.tblDoYouHaveAccess.tblCodeID_Country = dbo.tblCode.tblCodeID  where SmartUser = '" + Session["user"].ToString() + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        ///////////////////////////////////////////////////////////////////
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Session["email"] = dr["Email"].ToString();
                            Session["CountryCoordinator"] = dr["isCountryCoordinator"].ToString();
                            Session["SmartAdministrator"] = dr["isSMARTAdministrator"].ToString();
                            Session["tblCodeID_Country"] = dr["tblCodeID_Country"].ToString();
                            Session["Country"] = dr["Country"].ToString();
                            if (Session["CountryCoordinator"].ToString().ToLower() == "true")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    String a = exp.Message.ToString();
                }
            }
            return false;
        }

        protected void bttCreateEvent_Click(object sender, EventArgs e)
        {

            lblMessage.Visible = false;
            MembershipUser mu = Membership.GetUser(Session["user"].ToString());
            string strEmailUser = "unvalidateduser@ices.dk";
            if (mu != null)
            {
                strEmailUser = mu.Email.ToString();
            }
            string SessionID = "";
            if (UploadFile.HasFile)
            {
                SessionID = screenFile();

                if (!string.IsNullOrEmpty(SessionID))
                {

                    int indEventID = CreateEvent2();
                    if (indEventID > 0)
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("up_ImportDATSU_session", conn))
                            {
                                conn.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@smartUser", Session["user"].ToString()));
                                cmd.Parameters.Add(new SqlParameter("@eventID", indEventID.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@sessionID", SessionID));
                                cmd.ExecuteNonQuery();
                            }

                        }
                        Response.Redirect("editEvent.aspx?tblEventID=" + indEventID.ToString() + "&SessionID=" + SessionID.ToString());
                    }
                    else
                    {
                        lblMessage.Visible = true;
                        lblMessage.Text += "<br>It was not possible to create the event, please try again!";
                    }
                }
                else
                {
                    lblMessage.Visible = true;
                    //lblMessage.Text += "<br>Sample file not valid, please check your sample file!<br>" + result.ErrorMessage.ToString().Replace("Country", "EDMO code (Institude code)");
                }
            }
            else
            {
                // In the case the user does not want to submit a file it will only create the event
                int indEventID = CreateEvent2();
                if (indEventID > 0)
                {

                    Response.Redirect("editEvent.aspx?tblEventID=" + indEventID.ToString() + "&SessionID=" + SessionID.ToString());
                }
                else
                {
                    lblMessage.Visible = true;
                    lblMessage.Text += "<br>It was not possible to create the event, please try again!";
                }
            }
        }

        protected string screenFile()
        {

            if (UploadFile.HasFile)
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
                            string request = "{FileName:'" + UploadFile.FileName + "', EmailAddress:'" + Session["email"].ToString() + "', DataType:'SMARTDOTS'}";
                            content.Add(new StringContent(request), "Request");
                            content.Add(new StreamContent(UploadFile.FileContent), "File", UploadFile.FileName);
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
                                    if (response.NumberOfErrors < 1)
                                    {
                                        return response.SessionID;
                                    }
                                    lblMessage.Text = string.Format("File screened successfully.  SessionID = {0} File Size = {1}KBs Time taken = {2} secs.", response.SessionID, (double)(UploadFile.FileContent.Length / 1000), (double)(sw.ElapsedMilliseconds / 1000));
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
            return null;
        }

        public int CreateEvent2()
        {
            int intEventID = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "up_web_createEvent";
                    cmd.Parameters.Add(new SqlParameter("@Species", ddlSpecies.SelectedItem.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@NameOfEvent", txtEventName.Text.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@smartUser", Session["user"]));
                    cmd.Parameters.Add(new SqlParameter("@tblCodeID_TypeOfExercise", ddlTypeEvent.SelectedValue.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", txtStartDate.Text.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", txtEndDate.Text.ToString()));
                    if (txtDelegator1.Text != null)
                    {
                        if (txtDelegator1.Text.Length > 3)
                        {
                            cmd.Parameters.Add(new SqlParameter("@EventDelagate1", txtDelegator1.Text.ToString()));
                        }
                    }
                    cmd.Parameters.Add(new SqlParameter("@Purpose", ddlPurpose.SelectedItem.Text.ToString()));
                    SqlParameter countParameter = new SqlParameter("@ID", 0);
                    countParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(countParameter);

                    cmd.ExecuteNonQuery();
                    intEventID = Int32.Parse(cmd.Parameters["@ID"].Value.ToString());
                    if (intEventID > 0)
                    {
                        return intEventID;
                    }

                    cn.Close();
                }
            }
            catch (Exception e)
            {
                Session["message"] = Session["message"].ToString() + "It was not possible to create the event. Please contact the administrator smartdots@ices.dk";
                return intEventID;
            }
            return intEventID;
        }
    }
}