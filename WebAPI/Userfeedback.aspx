<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Userfeedback.aspx.cs" Inherits="WebInterface.Userfeedback" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smartdots - Users expertise</title>
    <link href="css/Default.css" rel="stylesheet" />
    <link href="css/ribbon.css" rel="stylesheet" />
</head>
<body>
    <!--#include file="header.html"-->
    <form id="form1" runat="server">
    <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
        <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" style="font-size:22px">User feedback</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td >Name:</td>
                <td style="color:red">
                <asp:TextBox ID="txtName" runat="server" ></asp:TextBox>
                &nbsp;<asp:Label ID="lblFillInName" runat="server" Text="*please fill in your name " Visible="false" ForeColor="Red"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td >Email:</td>
                <td>
                  <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox> &nbsp;<asp:Label ID="lblFillInEmail" ForeColor="Red" runat="server" Text="*please fill in your email " Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td >Role:</td><td >
                <asp:DropDownList ID="ddlRole" runat="server">
                  <asp:ListItem>Event coordinator</asp:ListItem>
                  <asp:ListItem>National coordinator</asp:ListItem>
                  <asp:ListItem>Age reader</asp:ListItem>
                  <asp:ListItem>External</asp:ListItem>
                  <asp:ListItem>Maturity stager</asp:ListItem>
                  <asp:ListItem>SmartDots administrator</asp:ListItem>
                  <asp:ListItem>ICES Expert group</asp:ListItem>
                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td >Issue:</td><td >
                <asp:DropDownList ID="ddlIssue" runat="server">
                  <asp:ListItem>Suggestion</asp:ListItem>
                  <asp:ListItem>Improvement</asp:ListItem>
                  <asp:ListItem>Bug</asp:ListItem>
                  <asp:ListItem>Manual</asp:ListItem>
                  <asp:ListItem>Help wanted</asp:ListItem>
                  <asp:ListItem>Documentation</asp:ListItem>
                  <asp:ListItem>Reporting</asp:ListItem>
                  <asp:ListItem>Event management</asp:ListItem>
                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td style="vertical-align:top" >Comment:</td><td >
                <asp:TextBox ID="txtFeedback" runat="server" Height="93px" TextMode="MultiLine" Width="400px"></asp:TextBox>
                 &nbsp;<asp:Label ID="lblFillInFeedback" runat="server" Text="*please fill in comment field" Visible="false" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td style="vertical-align:top" >Image:</td><td >
                <asp:FileUpload ID="imgUploadImage" runat="server"></asp:FileUpload>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td><td style="color:red">** Please attach an image, file of screendump for clarification purposes if you think it is necessary or helpful.</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td><td><asp:LinkButton ID="lnkSendFeedback" runat="server" OnClick="lnkSendFeedback_Click">Send feedback</asp:LinkButton></td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </center>    
    </form>
    <!--#include file="footer.html"-->
</body>
</html>
