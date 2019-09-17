using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class UsersExpertise : System.Web.UI.Page
    {
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
            if (WebInterface.App_Code.RunDBOperations.checkIfUserIsCountryCoordinator(Session["user"].ToString()))
            {
                if (!IsPostBack)
                {
                    lblCountryName.Text = "Please select an age reader from " + Session["Country"].ToString() + " to edit the age reader expertise: ";

                    SQLUsersList.SelectCommand = "SELECT     Email, SmartUser, isCountryCoordinator, tblCodeID_Country FROM         dbo.tblDoYouHaveAccess where tblCodeID_Country = " + Session["tblCodeID_Country"];
                    ddlListUsers.DataSourceID = "SQLUsersList";
                    ddlListUsers.DataBind();
                }
            }
            else
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }

            if (!IsPostBack)
            {
                addAllStocksToPanel();
            }
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            if (ViewState["controsladded"] == null)
                addAllStocksToPanel();
        }

        protected void lnkAddNew_Click(object sender, EventArgs e)
        {
            // Show the add new skills
            pnlAddNewSkills.Visible = true;
            if (ddlSpeciesList.SelectedIndex > 0 )
            {
                makeAllCheckBoxesInvisible();
                makeStocksVisiblePanel();
            }
        }

        public bool checkSelectedStocksOrAreas()
        {
            bool stockSelected = false;
            foreach (object o in pnlListAreas.Controls)
            {
                if (o.GetType() == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)o;
                    if (chk.Checked && chk.Visible)
                    {
                        AddSkill(chk.ID);
                        stockSelected = true;
                    }
                }
            }
            return stockSelected;
        }

        public string getStockCode(string strStockCode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                {
                    SqlDataReader rdr = null;
                    // create and open a connection object
                    string strSQL = "SELECT  tblCodeID, Code FROM  dbo.tblCode WHERE (Code = @stockcode) AND ( (tblCodeGroupID = 4) or tblCodeGroupID = 14  or tblCodeGroupID = 2  )";
                    using (SqlCommand cmd = new SqlCommand(strSQL, conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@stockcode", strStockCode));
                        conn.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            return rdr["tblCodeID"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "0";
            }

            return "0";
        }

        protected void AddSkill(string strStockCode)
        {
            string strSQL = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                {
                    strSQL = "INSERT INTO [dbo].[tblAgeReadersSkills] ([SmartUser],[Species],[tblCodeID_StockorArea],[ExpertiseLevel],[tblCodeID_PreparationMethod],[CoordinatorInsertedSkill]) ";
                    strSQL = strSQL + "VALUES('" + ddlListUsers.SelectedValue + "','" + ddlSpeciesList.SelectedValue.ToString() + "'," + getStockCode(strStockCode) + "," + ddlExpertiseLevel.SelectedIndex + "," + ddlPreparationMethod.SelectedValue +",'" + Session["user"] + "')";
                    cn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strSQL;
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                Session["message"] = Session["message"].ToString() + "The record for the stock: " + getStockCode(strStockCode) + " could not be inserted";
//                Response.Redirect("UsersExpertise.aspx?message=Sorry, there was a problem! Please try again later or contact the administrator.");
                return;
            }
        }

        public void addAllStocksToPanel()
        {
            int count = 1;

            try
            {
                String ConnectionString = ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand();
                    DataSet ds = new DataSet();
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "SELECT tblcodeid_Stock, StockCode FROM dbo.vw_StocksSpecies GROUP BY tblcodeid_Stock, StockCode union all SELECT tblCodeID_Area, Area FROM dbo.tblSpeciesGSAAreas GROUP BY tblCodeID_Area, Area union all SELECT        tblCodeID, Code FROM dbo.tblCode WHERE tblCodeGroupID = 2  and Code <> '27' order by stockcode ";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    ///////////////////////////////////////////////////////////////////
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        CheckBox chk = new CheckBox();
                        chk.Text = "&nbsp;" + dr.ItemArray[1].ToString().Trim() + "&nbsp;&nbsp;&nbsp;";
                        chk.ID = dr.ItemArray[1].ToString().Trim();
                        chk.ToolTip = dr.ItemArray[1].ToString().Trim();
                        chk.AutoPostBack = false;
                        chk.Visible = false;
                        pnlListAreas.Controls.Add(chk);
                        count++;
                    }
                    cn.Close();

                }
            }
            catch (SqlException)
            { }
            ViewState["controlsadded"] = true;
        }

        public void makeAllCheckBoxesInvisible()
        {

            foreach (object o in pnlListAreas.Controls)
            {
                if (o.GetType() == typeof(LiteralControl))
                {
                    LiteralControl l = (LiteralControl)o;
//                    l.Visible = false;
                }

                if (o.GetType() == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)o;
                    chk.Visible = false;
                }
            }
        }

        public void makeAllCheckBoxesCheckedOrUnChecked( bool value)
        {

            foreach (object o in pnlListAreas.Controls)
            {
                if (o.GetType() == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)o;
                    chk.Checked = value;
                }
            }
        }


        public void putCheckBoxVisible(string strNameStock)
        {
            bool makeNextVisible = false;

            foreach (object o in pnlListAreas.Controls)
            {
                if (makeNextVisible)
                {
                    if (o.GetType() == typeof(LiteralControl))
                    {

                        LiteralControl l = (LiteralControl)o;
                        l.Visible = true;
                    }
                    makeNextVisible = false;
                }

                if (o.GetType() == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)o;
                    if (chk.ID == strNameStock)
                    {
                        chk.Visible = true;
                    }
                }
            }
        }


      public void makeStocksVisiblePanel()
      {
         int count = 1;

         try
         {
            // This willl choose the logic of wether to show the stocks or the GSA areas

            String ConnectionString = ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               DataSet ds = new DataSet();
               cn.Open();
               cmd.Connection = cn;
               if (rdbStocks.Checked)
               {
                  cmd.CommandText = "select * from [dbo].[vw_StocksSpecies] where species = '" + ddlSpeciesList.SelectedValue.ToString() + "' ";
               }
               if (rdbMediterranium.Checked)
               {
                  cmd.CommandText = "select * from [dbo].[tblSpeciesGSAAreas] where species = '" + ddlSpeciesList.SelectedValue.ToString() + "' ";
               }
               if (rdbICESAreas.Checked)
               {
                  cmd.CommandText = "select * from tblcode where tblCodeGroupID = 2 ";
               }
               cmd.CommandType = CommandType.Text;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);
               ///////////////////////////////////////////////////////////////////
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  putCheckBoxVisible(dr.ItemArray[1].ToString());
               }
               cn.Close();

            }
         }
         catch (SqlException)
         { }
      }

        /// <summary>
        /// This function will recalculate the boxes that should be show in the add-expertise panel
        /// </summary>
        protected void recalculateCheckBoxes()
        {           
            lnkSelectAll.Text = "Select all";
            makeAllCheckBoxesCheckedOrUnChecked(false);
            makeAllCheckBoxesInvisible();
            makeStocksVisiblePanel();
        }

        protected void ddlSpeciesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            recalculateCheckBoxes();
        }

        protected void bttAddSkils_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            if (ddlSpeciesList.SelectedIndex > 0)
            {
                if (checkSelectedStocksOrAreas())
                {
                    makeAllCheckBoxesInvisible();
                    GridView1.DataBind();
                    pnlAddNewSkills.Visible = false;
                }
                else
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Please select at least a stock";
                }

            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Please select a species";
            }
        }

        protected void ddlListUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            makeAllCheckBoxesInvisible();
            pnlAddNewSkills.Visible = false;

            DataView d = (DataView)SQLUsersList.Select(DataSourceSelectArguments.Empty);
             if (d.Count < 1)
            {

            }
        }

        protected void lnkSelectAll_Click(object sender, EventArgs e)
        {

            if (lnkSelectAll.Text == "Select all")
            {
                lnkSelectAll.Text = "Unselect all";
                makeAllCheckBoxesCheckedOrUnChecked(true);
            }
            else
            {
                lnkSelectAll.Text = "Select all";
                makeAllCheckBoxesCheckedOrUnChecked(false);
            }
            makeAllCheckBoxesInvisible();
            makeStocksVisiblePanel();


        }

        protected void rdb_CheckedChanged(object sender, EventArgs e)
        {
            recalculateCheckBoxes();
            resourceListSpecies();
        }

        protected void resourceListSpecies()
        {
            ddlSpeciesList.DataTextField = "Code";
            ddlSpeciesList.DataTextField = "Code";
            if (rdbMediterranium.Checked)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SELECT ' Please select a species' as [Code], ' Please select a species' as [Description]  union all SELECT Species as [Description], Species as code FROM dbo.tblSpeciesGSAAreas group by Species", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                lblListOfStockOrListAreas.Text = "List of GSA areas:";
                ddlSpeciesList.DataSource = dt;
                ddlSpeciesList.DataSourceID = null;
                ddlSpeciesList.DataBind();


            }
            else
            {
                lblListOfStockOrListAreas.Text = "List of Stocks:";
                ddlSpeciesList.DataSourceID = "sqlListSpecies";
                ddlSpeciesList.DataSource = null;
                ddlSpeciesList.DataBind();
            }
        }

        protected void rdbMediterranium_CheckedChanged(object sender, EventArgs e)
        {
            recalculateCheckBoxes();
            resourceListSpecies();            
        }
    }
}