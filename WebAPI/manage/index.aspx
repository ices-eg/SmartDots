<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Webinterface.manage.index1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <br />
    &nbsp;&nbsp;&nbsp;
    
    
    <br />
    <table style="width:990px">
    <tr>
    <td style="width: 29px">
    </td>
    <td colspan="2">
        If you have been granted access to SmartDots, you can login with your sharepoint password:</td>
    </tr>
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
    <td style="width: 49px">
        <b>Username:</b></td>
    <td style="width: 236px">
        ICES\<asp:TextBox ID="txtUser" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
    <td style="width: 29px">
        &nbsp;</td>
    <td style="width: 49px">
        <b>Password:</b></td>
    <td style="width: 236px">
        <asp:TextBox ID="txtPass" runat="server" TextMode="Password" 
            ToolTip="Insert your password here"></asp:TextBox>
        </td>
    </tr>
    <tr>
    <td class="auto-style3">
        </td>
    <td class="auto-style4">
        &nbsp;</td>
    <td class="auto-style5">
        <asp:Button ID="bttSubmit" runat="server" CssClass="buttonType" Text="Check " OnClick="bttSubmit_Click" />
        </td>
    </tr>
    <tr>
    <td style="width: 29px">
        &nbsp;</td>
    <td colspan="2"><asp:Label ID="lblError" runat="server" ForeColor="Red" Height="69px" Visible="False" Width="745px" Font-Size="Larger"></asp:Label></td>
    </tr>
    <tr>
    <td >
        </td>
    <td colspan="2">
        If you have not been granted access please contact the ICES secretariat (professional secretary to your meeting or <a href="mailto:advice@ices.dk">advice@ices.dk </a></td>
    </tr>
    <tr>
    <td >
        &nbsp;</td>
    <td  colspan="2">
        If you have forgotten your sharepoint password, please click on this <a href="http://news.ices.dk/GroupNetPass/default.aspx">link</a></td>
    </tr>
    </table>
</asp:Content>
