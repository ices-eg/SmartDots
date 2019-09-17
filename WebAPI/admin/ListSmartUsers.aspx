<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListSmartUsers.aspx.cs" Inherits="WebInterface.admin.ListSmartUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="author" content="Carlos Pinto, Julie Davies, Wim Allegaert, Rui Catarino, Line Pinna,Karen Bekaert , Neil Holdsworth, Els Torreele" />
    <meta name="description" content="Introduction to ICES" />
    <meta name="keywords" content="ICES, International, Council, Exploration, International Council for the Exploration of the Sea, fisheries, stock assessment graphs, fish stocks" />
    <title>SmartDots</title>
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link href="../css/Default.css" rel="stylesheet" />
    
</head>
<body>
    <div class="corner-ribbon top-right sticky red shadow">BETA</div>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
    <div>
    <br />
    <center>
    <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Administrate SmartDots</a> &gt; Check SmartDots Users</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        <br />
    <table style="width:990px">
    <tr>
    <td style="width: 29px">
    </td>
    <td colspan="2">
        <asp:GridView ID="gv_Users" runat="server"></asp:GridView>
    </td>
    </tr>
    <tr>
    <td style="width: 29px">
    </td>
    <td style="width: 49px">
        &nbsp;</td>
    <td style="width: 236px">
        &nbsp;</td>
    </tr>
   
    </table>
        </center>
        </div>
    </form>
    <!--#include file="../footer.html"-->
</body>
</html>
