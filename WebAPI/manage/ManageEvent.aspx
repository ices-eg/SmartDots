<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageEvent.aspx.cs" Inherits="Webinterface.manage.ManageEvent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots - Manage the events</title>
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
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; List of Events</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
 <br />

               List of the current events for the user:
            <br />
            <br />

            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="tblEventID" DataSourceID="sqlEvents" CellPadding="4" GridLines="None" Width="985px" AllowSorting="True" OnRowDataBound="GridView1_RowDataBound" >
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                        <asp:TemplateField HeaderText="View event">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlnkViewEvent" runat="server" NavigateUrl='<%# Eval("tblEventID", "ViewEvent.aspx?tblEventID={0}")  %>'
                                    ImageUrl="~/Icons/report.png" ToolTip="View the details for this event"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>                    
                        <asp:TemplateField HeaderText="Edit event">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlnkEditEvent" runat="server" NavigateUrl='<%# Eval("tblEventID", "EditEvent.aspx?tblEventID={0}")  %>'
                                    ImageUrl="~/Icons/report_edit.png" ToolTip="Edit the details for this event"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>                    

                    <asp:BoundField DataField="tblEventID" HeaderText="Event ID" InsertVisible="False" ReadOnly="True" SortExpression="tblEventID" ItemStyle-Width="35px" ItemStyle-Height="25px" />
                    <asp:BoundField DataField="NameOfEvent" HeaderText="Name Of Event" SortExpression="NameOfEvent" />
                    <asp:BoundField DataField="Purpose" HeaderText="Purpose" SortExpression="Purpose" />
                    <asp:BoundField DataField="Species" HeaderText="Species" SortExpression="Species" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate_date" ItemStyle-Width="75px" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate_date" ItemStyle-Width="75px" />
                    <asp:BoundField DataField="OrganizerEmail" HeaderText="Organizer Email" SortExpression="OrganizerEmail" />
                    <asp:BoundField DataField="EventType" HeaderText="Event Type" ReadOnly="True" SortExpression="EventType" ItemStyle-Width="100px" />
                </Columns>
                <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            </asp:GridView>

            <asp:SqlDataSource ID="sqlEvents" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="
                select *
                ,[dbo].[GetIfEventManagerByEmail](@Email,tblEventID) as EventCoordinator
                 from [dbo].[vw_ListEvents]
              where [dbo].[GetIfUserCanViewEvent](tblEventID,[dbo].[GetSmartUserFromEmail](@Email)) > 0
              order by tblEventID desc">
                <SelectParameters>
                    <asp:SessionParameter Name="Email" SessionField="email" />
                </SelectParameters>
            </asp:SqlDataSource>
            
        </div>
    </form>
    </center>
    <!--#include file="../footer.html"-->

</body>
</html>
