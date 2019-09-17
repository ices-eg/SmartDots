<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnonimisedNameForSmartUsers.aspx.cs" Inherits="WebInterface.manage.AnonimisedNameForSmartUsers" %>

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
        <div style="width: 986px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
     <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; <a href="ViewEvent.aspx?tblEventID=<%=Request.QueryString["tblEventID"].ToString() %>">View event</a> &gt; Summary of the annotations per user</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
       
     <br />
         <div style="font-size:20px;color:grey"> Translation of the smartuser name to anonimized name:</div>
        <br />


        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDSEvents"  GridLines="None" Width="985px" AllowSorting="True" DataKeyNames="tblEventID,SmartUser">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="tblEventID" HeaderText="EventID" SortExpression="tblEventID" ReadOnly="True" />
                <asp:BoundField DataField="Rank" HeaderText="Rank" SortExpression="Rank" />
                <asp:BoundField DataField="SmartUser" HeaderText="SmartUser" SortExpression="SmartUser" ReadOnly="True" />
                <asp:BoundField DataField="AnonimisedName" HeaderText="Anonimised Name" SortExpression="AnonimisedName" />
                <asp:BoundField DataField="ExpertiseLevel" HeaderText="Expertise Level" SortExpression="ExpertiseLevel" />
            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSEvents" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                SelectCommand="SELECT        tblEventID, SmartUser, CASE ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE 'Basic' END AS ExpertiseLevel, Number as [Rank], AnonimisedName
FROM            dbo.tblEventParticipants
WHERE        (tblEventID = @tblEventID)
ORDER BY Number
" 
                CancelSelectOnNullParameter="False" EnableViewState="False">
            <SelectParameters>
                <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" Type="Int32" />
            </SelectParameters>

        </asp:SqlDataSource>
        <br />
     <br />
           <asp:Label ID="lblMessage" runat="server" Text=""  ForeColor="Red" ></asp:Label>
      <br />
       <br />
        <br />                
        <br />


            
            </div>
  
    </center>
  </form>
    <!--#include file="../footer.html"-->
</body>
</html>
