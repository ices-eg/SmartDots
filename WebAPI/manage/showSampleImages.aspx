<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="showSampleImages.aspx.cs" Inherits="Webinterface.manage.showSampleImages" %>

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
                        <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Edit event</td>
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
            <asp:Panel ID="pnlImages" runat="server">

            </asp:Panel>
        </div>
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
