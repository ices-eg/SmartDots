using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace WebInterface.App_Code
{
   public class SmartUtilities
   {
      /// <summary>
      /// Validates is the email is valid.
      /// </summary>
      /// <param name="email">String that is supposed to be the email.</param>
      /// <returns></returns>
      public static bool IsValidEmail(string email)
      {
         if (string.IsNullOrWhiteSpace(email))
            return false;

         try
         {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
               // Use IdnMapping class to convert Unicode domain names.
               var idn = new IdnMapping();

               // Pull out and process domain name (throws ArgumentException on invalid)
               var domainName = idn.GetAscii(match.Groups[2].Value);

               return match.Groups[1].Value + domainName;
            }
         }
         catch (RegexMatchTimeoutException e)
         {
            return false;
         }
         catch (ArgumentException e)
         {
            return false;
         }

         try
         {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
         }
         catch (RegexMatchTimeoutException)
         {
            return false;
         }
      }

      /// <summary>
      /// This function will return if the user is a country coordinator or now and setup the other session variables
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static string isNameValidSmartDotsUser(string strUser)
      {
         if (strUser != null)
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
                  cmd.CommandText = "SELECT [Email] FROM [dbo].[tblDoYouHaveAccess] where Email = '"  + strUser + "' or SmartUser = '" + strUser + "'";
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     return dr["Email"].ToString();
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
            }
         }
         return null;
      }


      public static void saveToLog(string strMessage)
      {
         string filePath = HttpContext.Current.Server.MapPath("~/temp/logErrors.txt");

         using (StreamWriter writer = new StreamWriter(filePath, true))
         {
            writer.WriteLine("Message :" + strMessage + "<br/>" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
         }
      }
      public static void saveToLog(Exception ex)
      {
         string filePath = HttpContext.Current.Server.MapPath("~/temp/logErrors.txt");

         using (StreamWriter writer = new StreamWriter(filePath, true))
         {
            writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
               "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
         }
      }

      public static void testWaterMark(string sourceImage, string strWaterMarkFile, string strResultFile)
      {
         //string sourceImage = @"D:\SmartDots\Watermark\CB16.png";
//         string strWaterMarkFile = @"D:\SmartDots\Watermark\SmartDots.png";
//         string strWaterMarkFile = Server.MapPath("~/Download/SmartDots.png");

         //         string strResultFile = @"D:\SmartDots\Watermark\SmartDots_CB16_carlos_watermark.png";
         Bitmap bitmap;
         Bitmap bitmapWaterMark;

         using (Stream bmpStream = System.IO.File.Open(sourceImage, System.IO.FileMode.Open))
         {
            Image imageOtolith = Image.FromStream(bmpStream);

            bitmap = new Bitmap(imageOtolith);
         }

         using (Stream waterMarkStream = System.IO.File.Open(strWaterMarkFile, System.IO.FileMode.Open))
         {
            Image imageWaterMark = Image.FromStream(waterMarkStream);

            bitmapWaterMark = new Bitmap(imageWaterMark);
         }
         // This can be improve by finding the middle of the imaghe file
         DrawWatermark(bitmapWaterMark,bitmap, 1, 600, strResultFile);

      }
      public static string getEventField(string tblEventID, string strFieldName)
      {
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
                  cmd.CommandText = "SELECT      " + strFieldName + " FROM dbo.tblEvent where tblEventID = " + tblEventID;
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     return dr[strFieldName].ToString();
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
               return "Field not found";
         }
         return "Field not found";
      }

      static public string sendEmailWithAttachment(string strEmail, string strSubject, string strBody, Attachment a)
      {
         SmtpClient smtpClient = new SmtpClient();
         MailMessage message = new MailMessage();
         try
         {
            MailAddress fromAddress = new MailAddress("smartdots@ices.dk", "SmartDots - smartdots.ices.dk");
            message.From = fromAddress;
            message.Attachments.Add(a);
            // You can specify the host name or ipaddress of your server
            // Default in IIS will be localhost 
            //smtpClient.Host = "smtp.ices.local";
            smtpClient.Host = "172.16.44.102";
            //Default port will be 25
            smtpClient.Port = 25;
            message.To.Add(strEmail);
            message.Subject = strSubject;

            //Specify true if it is html message
            message.IsBodyHtml = true;


            // Message body content

            message.Body = "<html><title>SmartDots - smartdots.ices.dk</title><body><br>" + strBody + "<br></body></html>";

            // Send SMTP mail
            smtpClient.Send(message);

            return "Email send";
         }
         catch (Exception ex)
         {
            return "There was a problem, email not send: " + ex.Message.ToString();
         }
      }

      static public string sendEmail(string strEmail, string strSubject, string strBody)
      {
         SmtpClient smtpClient = new SmtpClient();
         MailMessage message = new MailMessage();
         try
         {
            MailAddress fromAddress = new MailAddress("smartdots@ices.dk", "SmartDots - smartdots.ices.dk");
            message.From = fromAddress;

            // You can specify the host name or ipaddress of your server
            // Default in IIS will be localhost 
            //smtpClient.Host = "smtp.ices.local";
            smtpClient.Host = "172.16.44.102";
            //Default port will be 25
            smtpClient.Port = 25;
            message.To.Add(strEmail);
            message.Subject = strSubject;

            //Specify true if it is html message
            message.IsBodyHtml = true;


            // Message body content

            message.Body = "<html><title>SmartDots - smartdots.ices.dk</title><body><br>" + strBody + "<br></body></html>";

            // Send SMTP mail
            smtpClient.Send(message);

            return "Email send";
         }
         catch (Exception ex)
         {
            return "There was a problem, email not send: " + ex.Message.ToString();
         }
      }

      /// <summary>
      /// This procedure retuns the year of the eventID 
      /// </summary>
      /// <param name="strEventID">The ID of the Event</param>
      /// <returns></returns>
      public static string getYearEvent(string strEventID)
      {
         
            return RunDBOperations.executeScalarQuery("select cast(year(StartDate) as nvarchar(20)) from tblEvent where tblEventID =" + strEventID);
      }



      private void button3_Click(string sourceImage, string strWaterMarkFile, string strResultFile)
      {
         //String sourceImage = @"D:\SmartDots\Watermark\CB16.png";
         //string strWaterMarkFile = @"D:\SmartDots\Watermark\SmartDots.png";
         Bitmap bitmap;
         Bitmap bitmapWaterMark;

         using (Stream bmpStream = System.IO.File.Open(sourceImage, System.IO.FileMode.Open))
         {
            Image imageOtolith = Image.FromStream(bmpStream);

            bitmap = new Bitmap(imageOtolith);
         }

         using (Stream waterMarkStream = System.IO.File.Open(strWaterMarkFile, System.IO.FileMode.Open))
         {
            Image imageWaterMark = Image.FromStream(waterMarkStream);

            bitmapWaterMark = new Bitmap(imageWaterMark);
         }
         // This can be improve by finding the middle of the imaghe file
         DrawWatermark(bitmapWaterMark,bitmap, 1, 600, strResultFile);

      }

      public static float getPixelBrightness(Bitmap image, int x, int y)
      {
         /*
         using (Bitmap image = new Bitmap("C:\\temp\\test1.png"))
         {
         }
         */
         Color c = image.GetPixel(x, y);
         float f = c.GetBrightness(); //From 0 (black) to 1 (white)
         return f;
      }

      // Copy the watermark image over the result image.
      public static void DrawWatermark(Bitmap watermark_bm,Bitmap result_bm, int x, int y, string strResultFile)
      {
         const byte ALPHA = 128;
         // Set the watermark's pixels' Alpha components.
         Color clr;
         for (int py = 0; py < watermark_bm.Height; py++)
         {
            for (int px = 0; px < watermark_bm.Width; px++)
            {
               clr = watermark_bm.GetPixel(px, py);
               watermark_bm.SetPixel(px, py,
                   Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
            }
         }

         // Set the watermark's transparent color.
         watermark_bm.MakeTransparent(watermark_bm.GetPixel(0, 0));

         // Copy onto the result image.
         using (Graphics gr = Graphics.FromImage(result_bm))
         {
            gr.DrawImage(watermark_bm, x, y);
            
            result_bm.Save(strResultFile);
         }
      }

      public static Image Resize(Image originalImage, int w, int h)
      {
         //Original Image attributes
         int originalWidth = originalImage.Width;
         int originalHeight = originalImage.Height;

         // Figure out the ratio
         double ratioX = (double)w / (double)originalWidth;
         double ratioY = (double)h / (double)originalHeight;
         // use whichever multiplier is smaller
         double ratio = ratioX < ratioY ? ratioX : ratioY;

         // now we can get the new height and width
         int newHeight = Convert.ToInt32(originalHeight * ratio);
         int newWidth = Convert.ToInt32(originalWidth * ratio);

         Image thumbnail = new Bitmap(newWidth, newHeight);
         Graphics graphic = Graphics.FromImage(thumbnail);

         graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
         graphic.SmoothingMode = SmoothingMode.HighQuality;
         graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
         graphic.CompositingQuality = CompositingQuality.HighQuality;

         graphic.Clear(Color.Transparent);
         graphic.DrawImage(originalImage, 0, 0, newWidth, newHeight);

         return thumbnail;
      }

      public static bool copyImageWithRestrictions(string imgFileName, string newFileName, bool resizeImages, bool putWatermark)
      {
         string strWaterMarkFile = HttpContext.Current.Server.MapPath("~/download/smartdotslogo.png");
         if (File.Exists(newFileName))
         {
            return true;
         }

         try
         {
            if (File.Exists(imgFileName))
            {
               using (FileStream fs = new FileStream(imgFileName, FileMode.Open, FileAccess.Read))
               {
                  if (resizeImages)
                  {
                     Image img = Resize(Image.FromFile(imgFileName), 640, 480);
                     //This is the code to rezise the image, not needed in case no resize is needed;
                     if (putWatermark)
                     {
                        using (Bitmap waterMark = (Bitmap)System.Drawing.Image.FromFile(strWaterMarkFile))
                        {
                           Bitmap b = WatermarkImage((Bitmap)img, waterMark, new Point(1, 200));
                           b.Save(newFileName, ImageFormat.Jpeg);
                           b.Dispose();
                           b = null;
                        }
                     }
                     else
                     {
                        img.Save(newFileName, ImageFormat.Jpeg);
                     }
                     img = null;
                  }
                  else
                  {
                     System.IO.File.Copy(imgFileName, newFileName);
                  }

               }
            }
            else
            {
               SmartUtilities.saveToLog("The file does not exist:" + imgFileName.ToString() + Environment.NewLine);
            }
         }
         catch (Exception e)
         {
            SmartUtilities.saveToLog(e);
         }
         return true;
      }

      public static Bitmap WatermarkImage(Bitmap ImageToWatermark, Bitmap Watermark, Point WatermarkPosition)
      {

         float Opacity = 0.1F;
         using (Graphics G = Graphics.FromImage(ImageToWatermark))
         {
            using (ImageAttributes IA = new ImageAttributes())
            {
               ColorMatrix CM = new ColorMatrix();
               CM.Matrix33 = Opacity;
               IA.SetColorMatrix(CM);
               G.DrawImage(Watermark, new Rectangle(WatermarkPosition, Watermark.Size), 0, 0, Watermark.Width, Watermark.Height, GraphicsUnit.Pixel, IA);
            }
         }
         return ImageToWatermark;
      }
      /* Depracated, the other one can draw transparency 
      public static Image DrawWatermark(Bitmap watermark_bm, Bitmap result_bm, int x, int y)
      {
         const byte ALPHA = 0;
         // Set the watermark's pixels' Alpha components.
         Color clr;
         for (int py = 0; py < watermark_bm.Height; py++)
         {
            for (int px = 0; px < watermark_bm.Width; px++)
            {
               clr = watermark_bm.GetPixel(px, py);
               watermark_bm.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
            }
         }

         // Set the watermark's transparent color.
         watermark_bm.MakeTransparent(watermark_bm.GetPixel(0,0));

         // Copy onto the result image.
         using (Graphics gr = Graphics.FromImage(result_bm))
         {
            gr.DrawImage(watermark_bm, x, y);
            //result_bm.Save(strResultFile);
            return result_bm;
         }
      }
      */

      /// <summary>
      /// This is suppose to copy the image with a low resolution to be used in the view event;
      /// </summary>
      /// <param name="tblEventID"></param>
      /// <param name="tblSampleID"></param>
      /// <returns></returns>
      public bool copyImageLowResolution(int tblEventID, int tblSampleID)
      {
         return false;
      }

      /// <summary>
      /// This procedure will download the image and update the size in the database
      /// </summary>
      /// <param name="tblSmartImageID">SmartID in the database</param>
      /// <param name="URL">URL of the image</param>
      /// <returns></returns>
      public static bool updateSizeImage(string tblSmartImageID, string URL)
      {
         try
         {
            System.Net.WebRequest request = System.Net.WebRequest.Create(URL);
            using (System.Net.WebResponse response = request.GetResponse())
            {
               using (System.IO.Stream responseStream = response.GetResponseStream())
               {
                  Bitmap bitmap2 = new Bitmap(responseStream);
                  string strSQL = "update tblSmartImage set imgWith = " + bitmap2.Width.ToString() + " , imgheight = " + bitmap2.Height.ToString() + " where tblSmartImageID = " + tblSmartImageID.ToString();
                  RunDBOperations.runDBSmartDotsSQL(strSQL);
                  return true;
               }
            }
         }
         catch (Exception e)
         {
            saveToLog(e);
            return false;
         }

      }
   }
}