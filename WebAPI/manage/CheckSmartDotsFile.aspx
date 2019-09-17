<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckSmartDotsFile.aspx.cs" Inherits="Webinterface.manage.CheckSmartDotsFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />

</head>
<body>
  
  <!--#include file="../header.html"-->
    <form id="form1" runat="server">
        <center>
     <div style="width: 986px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
     <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Check a sample file</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br /> 

        <br />            
            <asp:FileUpload ID="fileUpload" runat="server" />
            <br /><br /><asp:Button ID="bttCheckFile" runat="server" Text="Check sample file" OnClick="bttCheckFile_Click"  />
            <br /><br /><asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
            <br /><br /><asp:HyperLink ID="hlnkResult" runat="server">View file screening</asp:HyperLink>
        <table>
            <tr>
                <td colspan="2"> </td>
            </tr>
            <tr>
                <td colspan="2"> &nbsp;</td>
            </tr>
            <tr><td colspan="2"> &nbsp;</td></tr>
            <tr><td colspan="2" > Convert from Excel to XML <a href="../uploads/SmartDotsSampleFile.xlsm">  download template</a> </td></tr>
            <tr><td colspan="2"> &nbsp;</td></tr>
        </table>
        </div>
            </center>
    </form>
        <!--#include file="../footer.html"-->
</body>
</html>
