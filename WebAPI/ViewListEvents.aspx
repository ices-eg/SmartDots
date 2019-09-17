<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewListEvents.aspx.cs" Inherits="WebInterface.ViewListEvents" %>


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
                <td colspan="2"><a href="../index.aspx">SmartDots</a> > List of events </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" ><b>Below you can see the list of events: </b>
                  <br />
                  <br /> Workshops and Exchanges with a status ‘Published’ are available by clicking on the event links.
                  <br />
                  <br /> Events and exchanges with a status ‘Completed’ or ‘Ongoing’ are not yet available to view. 
                    <br />
                    <br />
                          <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="sqlListEvents" Width="986px" OnRowDataBound="GridView1_RowDataBound">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlnkReports"  ToolTip="Download the Report"  Target="_blank"  runat="server" NavigateUrl='<%# Eval("intYear", "sampleImages/{0}") + Eval("tblEventID", "/{0}") + Eval("Report", "/{0}") %>'
                                        ImageUrl="~/Icons/report.png"></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle Width="25px" />
                                <ControlStyle Width="25px"  CssClass="centerAlign"/>
                            </asp:TemplateField> 
                              <asp:BoundField DataField="tblEventID" HeaderText="ID" SortExpression="tblEventID" />

                              <asp:TemplateField HeaderText="Name of the event" SortExpression="tblEventID"   HeaderStyle-VerticalAlign="Bottom">
                                  <ItemTemplate>
                                      <asp:HyperLink ID="hplnkViewEvent" runat="server" NavigateUrl='<%# Eval("tblEventID", "ViewEvent.aspx?key={0}") %>' ToolTip="View Event"  ><%#Eval("NameOfEvent")%></asp:HyperLink>
                                  </ItemTemplate>
                              </asp:TemplateField>

                              <asp:BoundField DataField="Purpose" HeaderText="Purpose" SortExpression="Purpose" />
                              <asp:BoundField DataField="intYear" HeaderText="Year"  SortExpression="intYear" />
                              <asp:BoundField DataField="Species" HeaderText="Species" SortExpression="Species" />
                              <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" />
                              <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                              <asp:BoundField DataField="OrganizerEmail" HeaderText="Organizer Email" SortExpression="OrganizerEmail" />

                            </Columns>
                              <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
                              <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                  </asp:GridView>
                  <asp:SqlDataSource ID="sqlListEvents" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="exec up_WebInterface_getListEvents"></asp:SqlDataSource>
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

