<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewMaturityEvent.aspx.cs" Inherits="WebInterface.manage.ViewMaturityEvent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" /></head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
        <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Manage events and users</a>&gt; <a href="ManageEvent.aspx">List of Events</a> &gt; View an event</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        <br />
        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;font-size:14px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
            <br />
          
            <br />
            <table cellpadding="0" cellspacing="0" style="width: 986px;">
            
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>EventID:</b> </td> <td> <asp:Label ID="lblEventID" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Name of the event:</b> </td> <td> <asp:Label ID="lblNameOfEvent" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Event type:</b> </td> <td> <asp:Label ID="lblEventType" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Start date:</b> </td> <td> <asp:Label ID="lblStartDate" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>End Date:</b> </td> <td> <asp:Label ID="lblEndDate" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Email of the Organizer:</b> </td> <td> <asp:Label ID="lblEmailOrganizer" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Species:</b> </td> <td> <asp:Label ID="lblSpecies" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Purpose:</b> </td> <td> <asp:Label ID="lblPurpose" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Closed:</b> </td> <td> <asp:Label ID="lblClosed" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>No. of Fish:</b> </td> <td> <asp:Label ID="lblNumberFish" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            </table>
 <br />
           <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            
            <tr>
                <td colspan="2"><asp:LinkButton ID="hplnkOpenAndCloseEvent" runat="server" Visible="false" OnClick="hplnkOpenEvent_Click">Close event</asp:LinkButton> </td>
            </tr>          
            </table>
            <br />
            <table id = "tableContent" border = "1" runat = "server" cellpadding="5" cellspacing="5" style="width: 986px;" ></table>
            <br />
            <br />
         </div>
         <br />
         <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    </center>
        
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
