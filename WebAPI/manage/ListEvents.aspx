<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListEvents.aspx.cs" Inherits="Webinterface.manage.ListEvents" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smartdots - List Events</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" />
</head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <center>
        <div style="width: 986px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
     <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; List of Events</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
       
     <br />
        <asp:Label ID="lblYearEvents" runat="server" Text="List of event for the year of "></asp:Label>
            <asp:DropDownList ID="ddlYear" runat="server" DataSourceID="SQLYears" DataTextField="Year" DataValueField="intYear" AutoPostBack="True" CssClass="comboGrupo" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
            </asp:DropDownList>
            &nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="hplnkDownloadList" NavigateUrl="../download/downloadListEvents.ashx" runat="server">Download List Events</asp:HyperLink>



            <asp:SqlDataSource ID="SQLYears" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select 'all' as Year, 0 as intYear union all SELECT cast(year([StartDate]) as nvarchar) as Year , year([StartDate]) as intYear FROM [dbo].[tblEvent] Group by  year([StartDate]) ">
            </asp:SqlDataSource>
        <br />


        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDSEvents"  GridLines="None" Width="985px" AllowSorting="True" DataKeyNames="tblEventID" OnRowDataBound="GridView1_RowDataBound">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField HeaderText="View event">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlnkViewEvent" runat="server" NavigateUrl='<%# Eval("tblEventID", "ViewEvent.aspx?tblEventID={0}")  %>' Text='<%# Eval("tblEventID", "{0}")  %>' ToolTip="View the details for this event"></asp:HyperLink>
                            </ItemTemplate>
                </asp:TemplateField>  
                <asp:BoundField DataField="Purpose" HeaderText="Purpose" ReadOnly="True" SortExpression="Purpose" ItemStyle-Height="20px" />
                <asp:BoundField DataField="EventType" HeaderText="Event Type" ReadOnly="True" SortExpression="EventType" ItemStyle-Width="100px" ItemStyle-Height="20px" />
                <asp:BoundField DataField="NameOfEvent" HeaderText="Name Of Event" SortExpression="NameOfEvent" />
                <asp:BoundField DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate_date" ItemStyle-Width="100px" ItemStyle-Height="20px" />
                <asp:BoundField DataField="EndDate" HeaderText="EndDate" SortExpression="EndDate_date" ItemStyle-Width="100px" ItemStyle-Height="20px" />
                <asp:BoundField DataField="OrganizerEmail" HeaderText="Organizer Email" SortExpression="FirstOrganizerEmail" ItemStyle-Height="20px" />
            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSEvents" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                SelectCommand="SELECT tblEventID, Purpose, NameOfEvent,StartDate_date,EndDate_date, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, EndDate, Protocol, OrganizerEmail, Institute, SmartUser, EventType, TypeOfStructure, intYear
, [dbo].[GetIfUserCanViewEvent](tblEventID,@smartuser) as canUserViewEvent
 FROM dbo.vw_ListEvents WHERE (intYear = @Year) OR (@Year = 0) and NameOfEvent not like '%Public training%'" 
                CancelSelectOnNullParameter="False" EnableViewState="False">
            <SelectParameters>
                <asp:SessionParameter Name="smartuser" SessionField="user" />
                <asp:ControlParameter ControlID="ddlYear" Name="Year" PropertyName="SelectedValue" DefaultValue=""  />
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
    </form>
    </center>
    <!--#include file="../footer.html"-->
</body>
</html>
