<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewMaturitySampleAnnotation.aspx.cs" Inherits="WebInterface.manage.viewMaturitySampleAnnotation" %>

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
       <br />
       <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;">
           <br />
           <br />
                <table cellpadding="0" cellspacing="0" style="width: 986px;">
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2"><a href="../index.aspx">SmartDots</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; <a href="ViewMaturityEvent.aspx?tblEventID=<%=Request.QueryString["tblEventID"].ToString() %>"> View event</a> &gt; View sample images</td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                </table>
             <br />
            
             <br />
            <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
          <br />
          <table style="width:984px">
               <tr>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>FishID:</span><asp:Label ID="Label1" runat="server" Text=""></asp:Label> </td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Catch date:</span> <asp:Label ID="lblDate" runat="server" Text=""></asp:Label></td>
                   <td class="auto-style1"><span style='font-weight:bold;font-size:12px;'>Fish Weight:</span> <asp:Label ID="lblWeight" runat="server" Text=""></asp:Label></td>
               </tr>
               <tr>
                   <td><span style='font-weight:bold;font-size:12px;'>Fish Lenght:</span> <asp:Label ID="lblLenght" runat="server" Text=""></asp:Label></td>
                   <td><span style='font-weight:bold;font-size:12px;'>Area:</span> <asp:Label ID="lblArea" runat="server" Text=""></asp:Label></td>
                   <td>&nbsp;</td>
               </tr>
           </table>
            <br />
           <br />
            <asp:Panel ID="pnlImages" runat="server">

            </asp:Panel>
         <br />
         <br />
          Sex: <asp:DropDownList ID="ddlSex" runat="server" CssClass="comboGrupo" DataSourceID="sqlSexCodes" DataTextField="Description" DataValueField="tblCodeID">
           </asp:DropDownList>
           <asp:SqlDataSource ID="sqlSexCodes" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT        dbo.tblCode.tblCodeID, dbo.tblCode.Code, dbo.tblCode.Description
FROM            dbo.tblAnnotationsMaturity INNER JOIN
                         dbo.tblCode ON dbo.tblAnnotationsMaturity.tblCodeID_Sex = dbo.tblCode.tblCodeID
WHERE        (dbo.tblAnnotationsMaturity.tblEventID = @tblEventID) AND (dbo.tblAnnotationsMaturity.FishID IN
                             (SELECT        FishID
                               FROM            dbo.tblSamples
                               WHERE        (tblSampleID = @sampleID)) and SmartUser = @user)
union all
select -1 , ' ',' '
union all
select dbo.tblCode.tblCodeID, dbo.tblCode.Code, dbo.tblCode.Description from tblcode where tblCodeGroupID = 10 and tblCodeID not in 
(SELECT        tblCodeID_Sex 
FROM            dbo.tblAnnotationsMaturity
WHERE        (dbo.tblAnnotationsMaturity.tblEventID = @tblEventID) AND (dbo.tblAnnotationsMaturity.FishID IN
                             (SELECT        FishID
                               FROM            dbo.tblSamples
                               WHERE        (tblSampleID = @sampleID))) and SmartUser = @user)">
             <SelectParameters>
               <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
               <asp:QueryStringParameter DefaultValue="sampleID" Name="sampleID" QueryStringField="sampleID" />
               <asp:SessionParameter DefaultValue="." Name="user" SessionField="user" />
             </SelectParameters>
           </asp:SqlDataSource>
         <br />         
         <br />
         
           Maturity: <asp:DropDownList ID="ddlMaturity" runat="server" CssClass="comboGrupo" DataSourceID="sqlMaturityStages" DataTextField="Description" DataValueField="tblCodeID">
           </asp:DropDownList>
           <asp:SqlDataSource ID="sqlMaturityStages" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="
SELECT        dbo.tblCode.tblCodeID, dbo.tblCode.Code,'(' + dbo.tblCode.Code + ')  ' +  dbo.tblCode.Description as Description
FROM            dbo.tblAnnotationsMaturity INNER JOIN
                         dbo.tblCode ON dbo.tblAnnotationsMaturity.tblCodeID_Maturity = dbo.tblCode.tblCodeID
WHERE        (dbo.tblAnnotationsMaturity.tblEventID = @tblEventID) AND (dbo.tblAnnotationsMaturity.FishID IN
                             (SELECT        FishID
                               FROM            dbo.tblSamples
                               WHERE        (tblSampleID = @sampleID))) and SmartUser = @user
union all
select -1 , ' ',' '
union all
select dbo.tblCode.tblCodeID, dbo.tblCode.Code, '(' + dbo.tblCode.Code + ')  ' + dbo.tblCode.Description  as Description from tblcode where tblCodeGroupID in (select value from tblEventData where tblEventID = @tblEventID and tblTypeID = 3) and tblCodeID not in 
(SELECT        tblCodeID_Maturity 
FROM            dbo.tblAnnotationsMaturity
WHERE        (dbo.tblAnnotationsMaturity.tblEventID = @tblEventID) AND (dbo.tblAnnotationsMaturity.FishID IN
                             (SELECT        FishID
                               FROM            dbo.tblSamples
                               WHERE        (tblSampleID = @sampleID))) and SmartUser = @user)">
             <SelectParameters>
               <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
               <asp:QueryStringParameter Name="sampleID" QueryStringField="sampleID" />
               <asp:SessionParameter DefaultValue="." Name="user" SessionField="user" />
             </SelectParameters>
           </asp:SqlDataSource>
         <br />         
         <br />
         Comments:<br />
         <asp:TextBox ID="txtComments" runat="server" Height="138px" Width="485px" TextMode="MultiLine"></asp:TextBox>
         <br />         
         <br />
         <asp:Button ID="bttUpdateAnnotationDatabase" runat="server" Text="Add annotation" OnClick="bttUpdateAnnotationDatabase_Click" CausesValidation="False" />
         <br />         
         <br />
         <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>        
         <br /> 
         <br />
        </div>
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>