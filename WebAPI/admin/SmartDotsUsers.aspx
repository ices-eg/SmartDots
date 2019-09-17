<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmartDotsUsers.aspx.cs" Inherits="WebInterface.admin.SmartDotsUsers" %>

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
    <center>
        <br />        
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Admin </a> &gt; Manage smart dots users</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </center>
    <form id="form1" runat="server">
        <div style="width: 986px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
     <br />
     <br />
     <br />
          Add a new user:
          <br />
          <asp:TextBox ID="txtNameUser" runat="server"></asp:TextBox><asp:DropDownList ID="ddlListCountries" runat="server" DataSourceID="sqlCountries" DataTextField="Country" DataValueField="tblCodeID"></asp:DropDownList>  * please only add one user at the time using their email.
          <asp:SqlDataSource ID="sqlCountries" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [tblCodeID], [Code], left(Description,1) + lower(right(Description,len(description)-1)) as Country  FROM [tblCode] WHERE ([tblCodeGroupID] = @tblCodeGroupID)">
            <SelectParameters>
              <asp:Parameter DefaultValue="1" Name="tblCodeGroupID" Type="Int32" />
            </SelectParameters>
          </asp:SqlDataSource>
         <br />
           
          <asp:Button ID="bttAdduser" runat="server" Text="Add user" OnClick="bttAdduser_Click" />

     <br />
     <br />
          <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
     <br />
     <br />
          <asp:LinkButton ID="lnkUpdateEmails" runat="server" OnClick="lnkUpdateEmails_Click">Update the email of Smart Users</asp:LinkButton>  
     <br />
     <br />
     <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SQLAllSmartDotsUsers" AllowSorting="True" DataKeyNames="SmartUser" ena  GridLines="None" Width="985px">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                  <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Edit user name" ToolTip="Edit user name" ID="ImgEdit"  CommandName="Edit" runat="server" ImageUrl="~/icons/table_edit.png"  CausesValidation="False" Width="16px" Height="16px"/>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:ImageButton AlternateText="Update Record" ToolTip="Update Record" ID="ImgEdit"  CommandName="Update" runat="server" ImageUrl="~/icons/accept.png"  CausesValidation="False" Width="16px" Height="16px"/>
                            <asp:ImageButton AlternateText="Cancel Record Edit" ToolTip="Cancel Record Edit" ID="ImgCancel"  CommandName="Cancel" runat="server" ImageUrl="~/icons/cancel.png"  CausesValidation="False" Width="16px" Height="16px"/>
                        </EditItemTemplate> 
                    </asp:TemplateField> 
                    <asp:BoundField DataField="SmartUser" HeaderText="SmartUser" ReadOnly="True" SortExpression="SmartUser" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <asp:TemplateField HeaderText="Country" SortExpression="Country" HeaderStyle-Width="100px">
                                                <EditItemTemplate>
                                                        <asp:DropDownList ID="ddlCountry"  CssClass="comboGrupo" runat="server"  SelectedValue='<%# Bind("tblCodeID_Country") %>'  DataSourceID="sqlCountries" DataTextField="Country" DataValueField="tblCodeID">
                                                        </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCountry" runat="server" Text='<%# Bind("Country") %>' ></asp:Label>
                                                </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="True" />
                    <asp:BoundField DataField="isCountryCoordinator" HeaderText="Country Coordinator" SortExpression="isCountryCoordinator"  />
                    <asp:BoundField DataField="LastAccess" HeaderText="Last Access" SortExpression="LastAccess" ItemStyle-HorizontalAlign="Right" ReadOnly="True"/>
                    <asp:BoundField DataField="lastWebAPIAccess" HeaderText="Last WebAPI Access" SortExpression="lastWebAPIAccess" ItemStyle-HorizontalAlign="Right" ReadOnly="True"/>
                    <asp:BoundField DataField="NumLogins" HeaderText="No Logins" SortExpression="NumLogins" ItemStyle-HorizontalAlign="Right" ReadOnly="True"/>
                    <asp:BoundField DataField="NumWebAPILogins" HeaderText="No WebAPI Logins" SortExpression="NumWebAPILogins" ItemStyle-HorizontalAlign="Right" ReadOnly="True"/>
                </Columns>
                <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" Height="30px" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            </asp:GridView>
            <asp:SqlDataSource ID="SQLAllSmartDotsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT        dbo.tblDoYouHaveAccess.SmartUser, dbo.tblDoYouHaveAccess.Name, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, cast(dbo.tblDoYouHaveAccess.NumLogins as int) NumLogins, 
                         CONVERT(char(10), dbo.tblDoYouHaveAccess.LastAccess,126) as LastAccess, dbo.tblDoYouHaveAccess.isCountryCoordinator, dbo.tblDoYouHaveAccess.isSMARTAdministrator, CONVERT(char(10), dbo.tblDoYouHaveAccess.lastWebAPIAccess,126) lastWebAPIAccess, 
                         cast(dbo.tblDoYouHaveAccess.NumWebAPILogins as int) NumWebAPILogins, Country.Description as Country
FROM            dbo.tblDoYouHaveAccess INNER JOIN
                         dbo.tblCode AS Country ON dbo.tblDoYouHaveAccess.tblCodeID_Country = Country.tblCodeID"
              UpdateCommand="update tblDoYouHaveAccess set isCountryCoordinator = @isCountryCoordinator, tblCodeID_Country = @tblCodeID_Country WHERE smartuser = @smartuser">
            <UpdateParameters>
                <asp:Parameter Name="tblCodeID_Country" />
                <asp:Parameter Name="isCountryCoordinator" />
                <asp:Parameter Name="SmartUser" />
            </UpdateParameters>
            </asp:SqlDataSource>
        </div>
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
