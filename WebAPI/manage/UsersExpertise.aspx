<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsersExpertise.aspx.cs" Inherits="Webinterface.manage.UsersExpertise" %>
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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <center>
        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
     <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Edit users experties</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
       
     <br />
        <asp:Label ID="lblCountryName" runat="server" Text="Please select an age reader from Denmark to edit the age reader expertise "></asp:Label>
            <asp:DropDownList ID="ddlListUsers" runat="server" DataSourceID="SQLUsersList" DataTextField="Email" DataValueField="SmartUser" AutoPostBack="True" OnSelectedIndexChanged="ddlListUsers_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
            <br />
            <asp:SqlDataSource ID="SQLUsersList" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [Email], [SmartUser], [isCountryCoordinator] FROM [tblDoYouHaveAccess]"></asp:SqlDataSource>
            <br /><asp:Label ID="lblNoSkillsForThisUser" runat="server" Text="This user has no expertise" Visible="false"></asp:Label>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDSListSkils" ForeColor="Red" GridLines="None" Width="969px" AllowSorting="True" DataKeyNames="SmartUser,Species,tblCodeID_StockorArea,tblCodeID_PreparationMethod">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Delete age reader expertise" ToolTip="Delete age reader expertise" ID="ImageButton2"  CommandName="Delete" runat="server" ImageUrl="~/icons/delete.png" OnClientClick='return confirm("Do you really want to delete this record?");'  CausesValidation="False" Width="16px" Height="16px"/>
                    </ItemTemplate>
                    <HeaderStyle Width="25px" />
                    <ControlStyle Width="15px"  CssClass="centerAlign"/>
                </asp:TemplateField>      
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Edit expertise level" ToolTip="Edit expertise level" ID="ImgEdit"  CommandName="Edit" runat="server" ImageUrl="~/icons/table_edit.png"  CausesValidation="False" Width="16px" Height="16px"/>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:ImageButton AlternateText="Update Record" ToolTip="Update Record" ID="ImgEdit"  CommandName="Update" runat="server" ImageUrl="~/icons/accept.png"  CausesValidation="False" Width="16px" Height="16px"/>
                            <asp:ImageButton AlternateText="Cancel Record Edit" ToolTip="Cancel Record Edit" ID="ImgCancel"  CommandName="Cancel" runat="server" ImageUrl="~/icons/cancel.png"  CausesValidation="False" Width="16px" Height="16px"/>
                        </EditItemTemplate> 
                    <HeaderStyle Width="25px" />
                </asp:TemplateField>        
                
                  
                <asp:BoundField DataField="SmartUser" HeaderText="SmartUser" SortExpression="SmartUser" ReadOnly="True" HeaderStyle-Width="80px"  />
                <asp:BoundField DataField="Species" HeaderText="Species" SortExpression="Species" ReadOnly="True" HeaderStyle-Width="170px"  />
                <asp:BoundField DataField="PreparationMethod" HeaderText="Preparation Method" ReadOnly="True" SortExpression="PreparationMethod" HeaderStyle-Width="120px"  />
                <asp:TemplateField HeaderText="Expertise Level" SortExpression="ExpertiseLevelText" HeaderStyle-Width="100px">
                    <EditItemTemplate>
                            <asp:DropDownList ID="ddlExpertiseLevel"  CssClass="comboGrupo" runat="server"  SelectedValue='<%# Bind("ExpertiseLevel") %>'>
                                    <asp:ListItem Value="0">Basic</asp:ListItem>
                                    <asp:ListItem Value="1">Advanced</asp:ListItem>
                            </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblExpertiseLevel" runat="server" Text='<%# Bind("ExpertiseLevelText") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StockOrArea" HeaderText="Stock description or Area code" ReadOnly="True" SortExpression="StockOrArea" />

            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSListSkils" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                        SelectCommand="SELECT     Skils.SmartUser, Skils.Species, Skils.tblCodeID_StockorArea, CASE Skils.ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE 'Basic' END AS ExpertiseLevelText,  Skils.ExpertiseLevel, Area.Code + '(' + Area.Description + ')' AS StockOrArea, PreparationMethod.Code as PreparationMethod, Skils.tblCodeID_PreparationMethod FROM         dbo.tblAgeReadersSkills AS Skils INNER JOIN dbo.tblCode AS Area ON Skils.tblCodeID_StockorArea = Area.tblCodeID INNER JOIN dbo.tblCode AS PreparationMethod ON Skils.tblCodeID_PreparationMethod = PreparationMethod.tblCodeID
                         where Smartuser = @SmartUser" 
                        DeleteCommand="delete from [dbo].[tblAgeReadersSkills] where SmartUser = @SmartUser and Species = @Species and tblCodeID_StockorArea = @tblCodeID_StockorArea and tblCodeID_PreparationMethod = @tblCodeID_PreparationMethod" 
                        UpdateCommand="UPDATE [tblAgeReadersSkills] set ExpertiseLevel = @ExpertiseLevel where SmartUser = @SmartUser and Species = @Species and tblCodeID_StockorArea = @tblCodeID_StockorArea and tblCodeID_PreparationMethod = @tblCodeID_PreparationMethod"
                        CancelSelectOnNullParameter="False" EnableViewState="False">
            <DeleteParameters>
                <asp:Parameter Name="tblCodeID_StockorArea"  Type="String" />
                <asp:Parameter Name="tblCodeID_PreparationMethod"  Type="String" />
                <asp:Parameter Name="Species"  Type="String" />
                <asp:Parameter Name="SmartUser"  Type="String" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlListUsers" Name="SmartUser" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>                
                <asp:Parameter Name="ExpertiseLevelText" />
                <asp:Parameter Name="tblCodeID_PreparationMethod"  Type="String" />
                <asp:Parameter Name="tblCodeID_StockorArea"  Type="String" />
                <asp:Parameter Name="Species"  Type="String" />
                <asp:Parameter Name="SmartUser"  Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <br />
        <br />
            <img src="../icons/add.png" /> <asp:LinkButton ID="lnkAddNew" runat="server" Text="Add a new expertise to this user. " OnClick="lnkAddNew_Click">Add a new expertise to this user. </asp:LinkButton>
     <br />
     <br />
        <asp:Panel ID="pnlAddNewSkills" runat="server"  Visible="false" Width="986px">
            <br />
            <br />
               Expertise Level: <asp:DropDownList ID="ddlExpertiseLevel"  CssClass="comboGrupo" runat="server">
                <asp:ListItem Value="0">Basic</asp:ListItem>
                <asp:ListItem Value="1">Advanced</asp:ListItem>
            </asp:DropDownList>
            <br />
            <br />
               Select Species: <asp:DropDownList ID="ddlSpeciesList" runat="server" DataSourceID="sqlListSpecies" DataTextField="Code" CssClass="comboGrupo" DataValueField="Code" AutoPostBack="True" OnSelectedIndexChanged="ddlSpeciesList_SelectedIndexChanged"></asp:DropDownList>
            <asp:SqlDataSource ID="sqlListSpecies" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT ' Please select a species' as [Code], ' Please select a species' as [Description]  union all  SELECT  [Code], [Description] FROM [tblCode] WHERE ([tblCodeGroupID] = @tblCodeGroupID) order by [Description]">
                <SelectParameters>
                    <asp:Parameter DefaultValue="3" Name="tblCodeGroupID" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <br />
                Sample Type (Preparation method): <asp:DropDownList ID="ddlPreparationMethod" runat="server" DataSourceID="SqlPreparationMethod" DataTextField="Description" DataValueField="tblCodeID"  CssClass="comboGrupo"></asp:DropDownList>

            <asp:SqlDataSource ID="SqlPreparationMethod" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [tblCodeID], 'Otolith (' + [Description] + ')' as Description FROM [tblCode] WHERE ([tblCodeGroupID] = @tblCodeGroupID) union all SELECT [tblCodeID], [Description] FROM [tblCode] WHERE ([tblCodeGroupID] = 9) and tblCodeID <> 529 ">
                <SelectParameters>
                    <asp:Parameter DefaultValue="5" Name="tblCodeGroupID" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <br /><asp:Panel ID="pnlStocksOrMediterranium" runat="server">
                       &nbsp;&nbsp;&nbsp; <asp:RadioButton ID="rdbStocks" GroupName="StocksOrMediterranean" Text=" Show list of ICES Stocks" runat="server" OnCheckedChanged="rdb_CheckedChanged" Checked="True" AutoPostBack="true"></asp:RadioButton>
                       &nbsp;&nbsp;&nbsp; <asp:RadioButton ID="rdbMediterranium" GroupName="StocksOrMediterranean" Text=" Show GFCM GSA areas" runat="server" AutoPostBack="true" OnCheckedChanged="rdb_CheckedChanged"></asp:RadioButton>
                       &nbsp;&nbsp;&nbsp; <asp:RadioButton ID="rdbICESAreas" GroupName="StocksOrMediterranean" Text=" Show ICES areas" runat="server" AutoPostBack="true" OnCheckedChanged="rdb_CheckedChanged"></asp:RadioButton>
                </asp:Panel>
            <br />
                <asp:Label ID="lblListOfStockOrListAreas" runat="server" Text="List Stocks:"></asp:Label>
               
                <br />
                <br />
                  <asp:LinkButton ID="lnkSelectAll" runat="server" OnClick="lnkSelectAll_Click">Select all</asp:LinkButton>              
                <br />
                <br />
                   <asp:Panel ID="pnlListAreas" runat="server" Width="986px"></asp:Panel>
            <br />
            <asp:Button ID="bttAddSkils" runat="server" Text="Add selected expertise" OnClick="bttAddSkils_Click"></asp:Button>
            <br />
                  <asp:Label ID="lblMessage" runat="server" Text=""  ForeColor="Red" ></asp:Label>
        </asp:Panel>
      <br />            
               
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
    </form>
    </center>
    <!--#include file="../footer.html"-->
</body>
</html>
