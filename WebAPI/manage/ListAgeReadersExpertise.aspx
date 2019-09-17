<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListAgeReadersExpertise.aspx.cs" Inherits="Webinterface.manage.ListAgeReadersExpertise" %>

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
        <div style="width: 986px; margin-left: auto; margin-right: auto; margin-top: 0;text-align:left">
     <br />
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; List of Age readers Expertise</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
     <br />
       
     <br />
        <asp:Label ID="lblCountryName" runat="server" Text="List of current users for :"></asp:Label>
            <asp:DropDownList ID="ddlCountries" runat="server" DataSourceID="SQLCountries" DataTextField="Description" DataValueField="tblCodeID" AutoPostBack="True" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SQLCountries" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="select 0 as tblCodeID, 'All' as code , 'All countries' as description union all SELECT     tblCodeID, Code, UPPER(LEFT(Description, 1)) + LOWER(RIGHT(Description, LEN(Description) - 1)) AS Description FROM         dbo.tblCode AS Country WHERE     (tblCodeGroupID = @tblCodeGroupID) order by description">
                <SelectParameters>
                    <asp:Parameter DefaultValue="1" Name="tblCodeGroupID" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            &nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="hplnkDownloadExpertise" NavigateUrl="../download/downloadListSmartusers.ashx" runat="server">Download List Expertise</asp:HyperLink>

        <br />


        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" Width="985px" DataSourceID="SqlDSListAgeReadersExperties"   AllowSorting="True">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>

                <asp:BoundField DataField="SmartUser" HeaderText="SmartUser" ReadOnly="True" SortExpression="SmartUser"  />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="Species" HeaderText="Species" SortExpression="Species" />
                <asp:BoundField DataField="ExpertiseLevel" HeaderText="Expertise Level" SortExpression="ExpertiseLevel" HeaderStyle-Width="140px" />
                <asp:BoundField DataField="Stock" HeaderText="Stock or Area" SortExpression="Stock" />
                <asp:BoundField DataField="PreparationMethod" HeaderText="Preparation Method" SortExpression="Preparationmethod" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="140px"  />
                <asp:BoundField DataField="isCountryCoordinator" HeaderText="Country Coordinator" SortExpression="isCountryCoordinator" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="Country" HeaderText="Country" ReadOnly="True" SortExpression="Country"  />
            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSListAgeReadersExperties" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                SelectCommand="
                    SELECT     dbo.tblDoYouHaveAccess.SmartUser, dbo.tblAgeReadersSkills.Species, 
                                          CASE ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE CASE ExpertiseLevel WHEN 0 THEN 'Basic' ELSE '' END END AS ExpertiseLevel, StockOrArea.Code AS Stock, 
                                          dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, UPPER(LEFT(Country.Description, 1)) + LOWER(RIGHT(Country.Description, 
                                          LEN(Country.Description) - 1)) AS Country, dbo.tblAgeReadersSkills.CreateDate, dbo.tblAgeReadersSkills.tblCodeID_PreparationMethod, 
                                          PreparationMethod.Description AS PreparationMethod, CASE dbo.tblDoYouHaveAccess.isCountryCoordinator WHEN 1 THEN 'Yes' ELSE 'No' END as isCountryCoordinator
                    FROM         dbo.tblAgeReadersSkills INNER JOIN
                                          dbo.tblCode AS PreparationMethod ON dbo.tblAgeReadersSkills.tblCodeID_PreparationMethod = PreparationMethod.tblCodeID RIGHT OUTER JOIN
                                          dbo.tblDoYouHaveAccess ON dbo.tblAgeReadersSkills.SmartUser = dbo.tblDoYouHaveAccess.SmartUser LEFT OUTER JOIN
                                          dbo.tblCode AS StockOrArea ON dbo.tblAgeReadersSkills.tblCodeID_StockorArea = StockOrArea.tblCodeID LEFT OUTER JOIN
                                          dbo.tblCode AS Country ON dbo.tblDoYouHaveAccess.tblCodeID_Country = Country.tblCodeID
                    where  dbo.tblDoYouHaveAccess.SmartUser not like 'Guest%' and (dbo.tblDoYouHaveAccess.tblCodeID_Country = @tblCodeID_Country or @tblCodeID_Country = 0) " 
                CancelSelectOnNullParameter="False" EnableViewState="False">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlCountries" Name="tblCodeID_Country" PropertyName="SelectedValue" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="Name" />
                <asp:Parameter Name="SmartUser"  Type="String"  />
            </UpdateParameters>

        </asp:SqlDataSource>
        <br />
     <br />
           <asp:Label ID="lblMessage" runat="server" Text=""  ForeColor="Red" ></asp:Label>
      <br />
       <br />
        <br />
       
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
