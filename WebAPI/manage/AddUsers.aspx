<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUsers.aspx.cs" Inherits="Webinterface.manage.AddUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smartdots - Users expertise</title>
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
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Add users</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
       
     <br />
        <asp:Label ID="lblCountryName" runat="server" Text="List of current users for :"></asp:Label>
        <br />


        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDSListUsers" ForeColor="Red" GridLines="None" Width="559px" AllowSorting="True" DataKeyNames="SmartUser">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Delete user" ToolTip="Delete user" ID="ImageButton2"  CommandName="Delete" runat="server" ImageUrl="~/icons/user_delete.png" OnClientClick='return confirm("Do you really want to delete this record?");'  CausesValidation="False" Width="16px" Height="16px"/>
                    </ItemTemplate>
                    <HeaderStyle Width="20px" />
                    <ControlStyle Width="15px"  CssClass="centerAlign"/>
                </asp:TemplateField>        

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Edit user name" ToolTip="Edit user name" ID="ImgEdit"  CommandName="Edit" runat="server" ImageUrl="~/icons/user_edit.png"  CausesValidation="False" Width="16px" Height="16px"/>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:ImageButton AlternateText="Update Record" ToolTip="Update Record" ID="ImgEdit"  CommandName="Update" runat="server" ImageUrl="~/icons/accept.png"  CausesValidation="False" Width="16px" Height="16px"/>
                            <asp:ImageButton AlternateText="Cancel Record Edit" ToolTip="Cancel Record Edit" ID="ImgCancel"  CommandName="Cancel" runat="server" ImageUrl="~/icons/cancel.png"  CausesValidation="False" Width="16px" Height="16px"/>
                        </EditItemTemplate> 
                    <HeaderStyle Width="20px" />
                </asp:TemplateField>        

                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:TemplateField HeaderText="SmartUser" SortExpression="SmartUser">
                    <EditItemTemplate>
                        <asp:Label ID="Label11" runat="server" Text='<%# Bind("SmartUser") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("SmartUser") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email" SortExpression="Email">
                    <EditItemTemplate>
                        <asp:Label ID="Label22" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Is Country Coordinator" HeaderText="Is Country Coordinator" SortExpression="Is Country Coordinator" ReadOnly="True" />
            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSListUsers" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS%>" 
                SelectCommand="SELECT Name, LOWER (SmartUser) as SmartUser, Email, CASE isCountryCoordinator WHEN 1 THEN 'Yes' else 'No' END  AS [Is Country Coordinator] FROM tblDoYouHaveAccess where tblCodeID_Country =  @tblCodeID_Country order by SmartUser" 
                UpdateCommand="UPDATE tblDoYouHaveAccess SET Name = @Name WHERE (SmartUser = @SmartUser)" 
                DeleteCommand="DELETE FROM tblDoYouHaveAccess WHERE (SmartUser = @SmartUser)" 
                CancelSelectOnNullParameter="False" EnableViewState="False">
            <DeleteParameters>
                <asp:Parameter Name="SmartUser"  Type="String" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="SmartUser"  Type="String"  />
            </UpdateParameters>
            <SelectParameters>
                <asp:SessionParameter Name="tblCodeID_Country" SessionField="tblCodeID_Country" Type="Int32" />
            </SelectParameters>

        </asp:SqlDataSource>
        <br />
     <br />
           <asp:Label ID="lblMessage" runat="server" Text=""  ForeColor="Red" ></asp:Label>
      <br />
       <br />
        Please fill in the share point username to give access to the system. 
         <br />Note: Use comma's between names. 
       
        <br />
        <br />
        <asp:TextBox ID="txtUsers" runat="server" style="margin-bottom: 0px" Height="100px" Width="463px" TextMode="MultiLine"></asp:TextBox>
       
        <br />
       
         <br />

        <asp:Button ID="bttAddUsers" runat="server" Text="Add Users" OnClick="bttAddUsers_Click"  />

        <br />                
        <br />


            
            </div>
    </form>
    </center>
    <!--#include file="../footer.html"-->
</body>
</html>
