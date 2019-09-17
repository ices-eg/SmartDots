<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateToken.aspx.cs" Inherits="Webinterface.manage.CreateToken" %>

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
        <center>
        <br />        
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Create a new token</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </center>

    <form id="form1" runat="server">
<br />
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <center>
    <br />
    <br />
    &nbsp;&nbsp;&nbsp;
    
    
    <br />
    <table style="width:990px">
    <tr>
    <td style="width: 29px">
    </td>
    <td colspan="2" style="font-size:20px">
       Create a new token</td>
    </tr>
    <tr><td colspan="3">&nbsp;</td></tr>
    <tr>
    <td style="width: 29px">
    </td>
    <td colspan="2" style="font-size:12px">
        This page is to create a new token that the users can use as clients of the secure webservices. In this web services the user can do the same operations as when autenticated. 
        <br />This token can be requested for the period of 1,3 or 5, 10 or 30 days.
        <br />Requesting this token will disable any other tokens requested by the same user previuosly. A user can only have one token at the time.
        
    </td>
    </tr>
    <tr><td colspan="3">&nbsp;</td></tr>
    <tr><td colspan="3">&nbsp;</td></tr>
    <tr>
    <td style="width: 29px">
    </td>
    <td style="width: 49px">
        &nbsp;</td>
    <td style="width: 236px">
        &nbsp;</td>
    </tr>
    <tr>
    <td style="width: 29px">
        &nbsp;</td>
    <td style="width: 49px;font-weight:bold">
        <b>Username:</b></td>
    <td style="width: 236px">
        <asp:Label ID="lblUser" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr><td></td><td colspan="2"> &nbsp;</td></tr>

    <tr>
    <td style="width: 29px">
        &nbsp;</td>
    <td style="width: 49px;font-weight:bold">
        <b>Valid for </b></td>
    <td style="width: 236px">
        <asp:DropDownList ID="ddlNumberOfDays" runat="server">
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>30</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
        </asp:DropDownList> Days
        </td>
    </tr>
    <tr><td></td><td colspan="2"> &nbsp;</td></tr>
    <tr>
    <td class="auto-style3">
        </td>
    <td class="auto-style4">
        &nbsp;</td>
    <td class="auto-style5">
        <asp:Button ID="bttSubmit" runat="server" CssClass="buttonType" Text="Create a new token " OnClick="bttSubmit_Click"  />
        </td>
    </tr>
    <tr>
    <td style="width: 29px">
        &nbsp;</td>
    <td colspan="2"><asp:Label ID="lblError" runat="server" ForeColor="Red" Height="69px" Visible="False" Width="745px" Font-Size="Larger"></asp:Label></td>
    </tr>
    <tr><td></td><td colspan="2" style="font-size:14px;font-weight:bold">List of requested tokens:</td></tr>
    <tr><td></td><td colspan="2"> &nbsp;</td></tr>

    <tr><td></td><td colspan="2">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"  DataKeyNames="tblTokensID" DataSourceID="sqlDSTokens" ForeColor="Red" GridLines="None" Width="824px" AllowSorting="True" >
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="tblTokensID" HeaderText="Token" ReadOnly="True" SortExpression="tblTokensID" />
                <asp:BoundField DataField="SmartUser" HeaderText="User" SortExpression="SmartUser" />
                <asp:BoundField DataField="DateTime" HeaderText="Start Date Time" SortExpression="DateTime" />
                <asp:BoundField DataField="ValidFor" HeaderText="Valid For (Days)" SortExpression="ValidFor" />
                <asp:BoundField DataField="stillValid" HeaderText="Valid?" SortExpression="stillValid" />
            </Columns>
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:GridView>
        <asp:SqlDataSource ID="sqlDSTokens" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [tblTokensID]  , [SmartUser], [DateTime], [Created], [ValidFor], [stillValid] FROM [tblTokens] WHERE ([SmartUser] = @user) order by [DateTime] desc ">
            <SelectParameters>
                <asp:SessionParameter Name="user" SessionField="user" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        </td></tr>

    <tr><td></td><td colspan="2"> &nbsp;</td></tr>

    <tr>
    <td class="auto-style1">
        </td>
    <td class="auto-style2" colspan="2">
        If you have not been granted access please contact the ICES secretariat (professional secretary to your meeting or <a href="mailto:advice@ices.dk">advice@ices.dk </a></td>
    </tr>
    <tr>
    <td class="auto-style1">
        &nbsp;</td>
    <td class="auto-style2" colspan="2">
        If you have forgotten your sharepoint password, please check this <a href="http://news.ices.dk/GroupNetPass/default.aspx">link</a></td>
    </tr>
    </table>

<br />
</center><br />
<br />   

</form>
      <!--#include file="../footer.html"-->
</body>
</html>
