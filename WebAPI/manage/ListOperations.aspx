<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOperations.aspx.cs" Inherits="Webinterface.manage.ListOperations" %>

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
    <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> > Manage events and users</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" ><b>Below you can see which data are available in the current database. You can choose to: </b>
                    <br />
                    <br />
                          - <a href="AddUsers.aspx"> Add new users</a>
                    <br />
                    <br />
                          - <a href="UsersExpertise.aspx"> Setup users expertise (for your country)</a>
                    <br />
                    <br />
                          - <a href="ListAgeReadersExpertise.aspx"> List of age readers expertise</a>
                    <br />
                    <br />
                          - <a href="EventForm.aspx"> Propose a new event </a>
                    <br />
                    <br />
                          - <a href="ManageEvent.aspx"> Manage current events</a>
                    <br />
                    <br />       
                         - <a href="ListEvents.aspx"> List of events </a>
                    <br />
                    <br />       
                         - <a href="CheckSmartDotsFile.aspx"> Verify if a sample file is according to the format </a>
                    <br />
                    <br />
                         - <a href="CreateToken.aspx"> Creates a new token to work with the web services and the SmartDots software </a>
                    <br />
                    <br />
                         - <a href="Logout.aspx"> Logout </a>
                    <br />
                    <br />
                </td>
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
        </form>
    </center>
    <!--#include file="../footer.html"-->
</body>
</html>
