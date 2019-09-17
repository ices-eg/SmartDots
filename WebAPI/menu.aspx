<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu.aspx.cs" Inherits="WebInterface.menu" %>

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
    <!--#include file="header.html"-->
    <form id="form1" runat="server">
    <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> > List of operations </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" ><b>Below you can see which data are available in the current database. You can choose to: </b>
                    <br />
                    <br />
                          - <a href="ViewUsers.aspx"> View list of users</a>
                    <br />
                    <br />
                          - <a href="ViewListEvents.aspx"> View List Events</a>
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
    </center>
        </form>
    <!--#include file="footer.html"-->

</body>
</html>
