<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewImage.aspx.cs" Inherits="WebInterface.viewImage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smartdots - Users expertise</title>
    <link href="css/Default.css" rel="stylesheet" />
    <link href="css/ribbon.css" rel="stylesheet" />

    <style type="text/css">
        .auto-style1 {
            width: 325px;
            height: 13px;
        }
    </style>

</head>
<body>
    <script>
    // This will draw a circle
    function drawDot(canvas, x, y,size,color)
    {
      canvas.beginPath();
      canvas.arc(x, y, size, 0, 2 * Math.PI);
      canvas.strokeStyle = color;
      canvas.fillStyle = color;
      canvas.fill();
      canvas.stroke();
    }

    // This will draw a line
    function drawLine(canvas, x1, y1,x2,y2,size,color)
    {
      canvas.beginPath();
      ctx.moveTo(x1, y1);
      ctx.lineTo(x2, y2);
      ctx.lineWidth = size;
      ctx.strokeStyle = color;
      canvas.stroke();
    }
    </script>

    <!--#include file="header.html"-->
    <form id="form1" runat="server">
       <br />
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
       <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;">
           <br />
           <br />
                <table cellpadding="0" cellspacing="0" style="width: 986px;">
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ViewEvent.aspx?key=<%=Request.QueryString["tblEventID"].ToString() %>"> View event</a> &gt; View Image Annotations</td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                </table>
             <br />
             <br />
             <br />


           <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
           <br />

           

           <table style="width:984px">
               <tr>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>FishID:</span> <asp:Label ID="lblFishID" runat="server" Text=""></asp:Label></td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Sex:</span> <asp:Label ID="lblSex" runat="server" Text=""></asp:Label></td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Fish Weight:</span> <asp:Label ID="lblWeight" runat="server" Text=""></asp:Label></td>
               </tr>
               <tr>
                   <td><span style='font-weight:bold;font-size:12px;'>Fish Lenght:</span> <asp:Label ID="lblLenght" runat="server" Text=""></asp:Label></td>
                   <td><span style='font-weight:bold;font-size:12px;'>Area:</span> <asp:Label ID="lblArea" runat="server" Text=""></asp:Label></td>
                   <td><span style='font-weight:bold;font-size:12px;'>Catch date:</span> <asp:Label ID="lblDate" runat="server" Text=""></asp:Label></td>
               </tr>
           </table>
            <br />
           <table>
               <tr><td>
                    <asp:Panel ID="pnlImages" runat="server">
                    </asp:Panel>
                   </td>
                 <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                 <td>
                        <br />
                        View only reader: <asp:DropDownList ID="ddlListReaders" runat="server" CssClass="comboGrupo" DataSourceID="sqlDSReaders" DataTextField="AnonimisedName" DataValueField="AnonimisedName" AutoPostBack="True"></asp:DropDownList>
                        <br />
                        <br />
                    <asp:Panel ID="pnlAnnotationDetails" runat="server">
                        <asp:SqlDataSource ID="sqlDSReaders" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="
                            select ' select a reader' as AnonimisedName union all SELECT        dbo.tblEventParticipants.AnonimisedName
FROM            dbo.vw_Annotations INNER JOIN
                         dbo.tblEventParticipants ON dbo.vw_Annotations.tblEventID = dbo.tblEventParticipants.tblEventID AND dbo.vw_Annotations.SmartUser = dbo.tblEventParticipants.SmartUser
WHERE        (dbo.vw_Annotations.tblSmartImageID = @tblSmartImageID) AND (dbo.vw_Annotations.IsFixed = 0) AND (dbo.tblEventParticipants.Number IS NOT NULL)
GROUP BY  dbo.tblEventParticipants.AnonimisedName
order by AnonimisedName">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="" Name="tblSmartImageID" QueryStringField="SmartImageID" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </asp:Panel>
                </td>
               </tr>
           </table>
          
               
               
               <br />
               
               <br />
               
           <br />
        </div>
    </form>
    <!--#include file="footer.html"-->

</body>
</html>
