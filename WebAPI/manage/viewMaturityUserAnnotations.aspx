<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewMaturityUserAnnotations.aspx.cs" Inherits="WebInterface.manage.viewMaturityUserAnnotations" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots</title>
     <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" />
</head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
       <br />
       <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;">
           <br />
           <br />
                <table cellpadding="0" cellspacing="0" style="width: 986px;">
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; <a href="ViewMaturityEvent.aspx?tblEventID=<%=Request.QueryString["tblEventID"].ToString() %>"> View event</a> &gt; View sample images</td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                </table>
             <br />
            
             <br />
            <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
          <br />
          <table style="width:984px">
               <tr>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>FishID:</span> <asp:Label ID="lblfishID" runat="server" Text=""></asp:Label></td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Catch date:</span> <asp:Label ID="lblDate" runat="server" Text=""></asp:Label></td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Fish Weight:</span> <asp:Label ID="lblWeight" runat="server" Text=""></asp:Label></td>
               </tr>
               <tr>
                   <td><span style='font-weight:bold;font-size:12px;'>Fish Lenght:</span> <asp:Label ID="lblLenght" runat="server" Text=""></asp:Label></td>
                   <td><span style='font-weight:bold;font-size:12px;'>Area:</span> <asp:Label ID="lblArea" runat="server" Text=""></asp:Label></td>
                   <td>&nbsp;</td>
               </tr>
           </table>
            <br />
           <br />
            <asp:Panel ID="pnlAnnotationDetails" runat="server">
            </asp:Panel>
         <br />
            <asp:Panel ID="pnlImages" runat="server">
            </asp:Panel>
         <br />
         <br />
        </div>
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
