using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface
{
   public partial class Userfeedback : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {

      }

      protected void TextBox1_TextChanged(object sender, EventArgs e)
      {

      }

      protected void lnkSendFeedback_Click(object sender, EventArgs e)
      {
         string strBody;
         lblFillInName.Visible = false;
         lblFillInEmail.Visible = false;
         lblFillInFeedback.Visible = false;
         // Cheks if the user has fill in the name
         if (string.IsNullOrEmpty(txtName.Text))
         {
            lblFillInName.Visible = true;
         }
         // Cheks if the user has fill in the email
         if (string.IsNullOrEmpty(txtEmail.Text))
         {
            lblFillInEmail.Visible = true;
         }
         else
         {
            if (!SmartUtilities.IsValidEmail(txtEmail.Text))
            {
               lblFillInEmail.Text = "* please fill in a valid email";
               lblFillInEmail.Visible = true;
            }
         }

         // Cheks if the user has fill in the feedback
         if (string.IsNullOrEmpty(txtFeedback.Text))
         {
            lblFillInFeedback.Visible = true;
         }
         // Exists the function incase the user has forgot any of the fields
         if (lblFillInFeedback.Visible || lblFillInEmail.Visible || lblFillInName.Visible )
         {
            return;
         }


         // BUilds the body of the message
         strBody = "Email was send by <a href='https://smartdots.ices.dk'>SmartDots</a> feedback page <br>";
         strBody += "<br><b>Name</b>: " + txtName.Text;
         strBody += "<br><b>Email</b> : <a hef='mailto:" + txtEmail.Text + "'>" + txtEmail.Text + "</a>";
         strBody += "<br><b>Role</b> : " + ddlRole.SelectedValue.ToString() ;
         strBody += "<br><b>Issue</b> : " + ddlIssue.SelectedValue.ToString();
         strBody += "<br><b>Comment</b> : <br>" + txtFeedback.Text.ToString();
         strBody += "<br><br><img SRC='http://smartdots.ices.dk/images/SmartDots_logo.png' ALT='SmartDots'></img>";
         strBody += "<br><br><br><br>Save a tree ... please don't print this e-mail unless you really need to.";



         if (imgUploadImage.HasFile)
         {
            string appPath = Request.PhysicalApplicationPath + "Temp\\";
            if (File.Exists(appPath + Server.HtmlEncode(imgUploadImage.FileName)))
            {
               try
               {
                  File.Delete(appPath + Server.HtmlEncode(imgUploadImage.FileName));
               }
               catch (Exception ex2)
               {
               }
            }
            imgUploadImage.SaveAs(appPath + Server.HtmlEncode(imgUploadImage.FileName));
            SmartUtilities.sendEmailWithAttachment("carlos@ices.dk,jane.godiksen@hi.no,joco@aqua.dtu.dk", "Comment from SmartDots feedback page", strBody,new Attachment(appPath + Server.HtmlEncode(imgUploadImage.FileName)));
            Response.Redirect("thankyou.aspx");
         }
         else
         {
            SmartUtilities.sendEmail("carlos@ices.dk,jane.godiksen@hi.no,joco@aqua.dtu.dk", "Comment from SmartDots feedback page", strBody);
            Response.Redirect("thankyou.aspx");
         }
      }
   }
}