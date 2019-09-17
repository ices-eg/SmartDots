<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEvent.aspx.cs" Inherits="WebInterface.manage.ViewEvent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" /></head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
        <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Manage events and users</a>&gt; <a href="ManageEvent.aspx">List of Events</a> &gt; View an event</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        <br />
        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;font-size:14px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
            <br />
            <br />
            <br />
            <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
            <b>This page can be used to get an overview of the samples in your event and to track the progress of the participants </b>
            <br /> <br />
            <table  cellpadding="0" cellspacing="0" style="width: 986px;">
                <tr>
                <td style="width: 220px;background-color:#F15D2A;color:white">Quarter</td>
                <td style="width: 220px;background-color:#F15D2A;color:white">Area</td>
                <td style="width: 220px;background-color:#F15D2A;color:white"></td>
                <td style="width: 220px;background-color:#F15D2A;color:white"></td>
                <td style="width: 106px;background-color:#F15D2A;color:white"><asp:Label ID="lblMode" runat="server" Text="Modal age" Visible="false"></asp:Label> </td>
                    </tr>
                <tr><td colspan="5">&nbsp;</td></tr>
                <tr>
                <td style="width: 220px;background-color:#FFFFFF"> <asp:DropDownList ID="ddlListQuarter" runat="server" AutoPostBack="true" DataSourceID="sqlQuarter" DataTextField="Quarter" DataValueField="Quarter" CssClass="comboGrupo"></asp:DropDownList></td>
                <td style="width: 220px;background-color:#FFFFFF"><asp:DropDownList ID="ddlListArea" runat="server" AutoPostBack="true" DataSourceID="sqlDSListArea" DataTextField="Code" DataValueField="tblCodeID_Area" CssClass="comboGrupo"></asp:DropDownList></td>
                <td style="width: 220px;"><asp:DropDownList ID="ddlListReaders" runat="server" DataSourceID="sqlListReaders" AutoPostBack="true" DataTextField="SmartUser" DataValueField="SmartUser" CssClass="comboGrupo" Visible="false"></asp:DropDownList></td>
                <td style="width: 220px;background-color:#FFFFFF"><asp:DropDownList ID="ddlListExperties" runat="server" AutoPostBack="true" DataSourceID="sqlExpertiseLevel" DataTextField="Expertise" DataValueField="ExpertiseLevel" CssClass="comboGrupo" Visible="false"></asp:DropDownList></td>
                <td style="width: 106px;background-color:#FFFFFF"><asp:DropDownList ID="ddlMode" AutoPostBack="True" CssClass="comboGrupo" DataTextField="Mode" DataValueField="intMode" runat="server" DataSourceID="sqlMode" Visible="false"></asp:DropDownList>
                    <asp:SqlDataSource ID="sqlMode" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select -1 as intMode, 'All' as Mode UNION ALL select Mode AS intMode, Mode  from ( SELECT        dbo.getMode(tblSmartImageID) AS intMode, cast(dbo.getMode(tblSmartImageID) as varchar) AS Mode  FROM dbo.vw_SmartImages  where (tblEventID = @tblEventID)  GROUP BY dbo.getMode(tblSmartImageID) ) as M where Mode is not null order by intmode">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    </td>
                    </tr>
            </table>

            
                    <asp:SqlDataSource ID="sqlListReaders" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select -1 as tblEventID , 'All' as SmartUser 
union all 
SELECT    dbo.vw_PermissionForAnnotations.tblEventID,    dbo.tblAnnotations.SmartUser
                                    FROM            dbo.vw_PermissionForAnnotations INNER JOIN dbo.tblAnnotations ON dbo.vw_PermissionForAnnotations.tblAnnotationID = dbo.tblAnnotations.tblAnnotationID
                                    WHERE        (dbo.vw_PermissionForAnnotations.SmartUser = @User) AND (dbo.vw_PermissionForAnnotations.tblEventID = @tblEventID) AND (dbo.tblAnnotations.IsFixed = 0)
                                    GROUP BY dbo.vw_PermissionForAnnotations.tblEventID, dbo.tblAnnotations.SmartUser">
                        <SelectParameters>
                            <asp:SessionParameter Name="user" SessionField="user" />
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sqlQuarter" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select 'All' as Quarter union all SELECT      cast(DATEPART(QUARTER, CatchDate) as varchar) AS Quarter FROM            dbo.tblSamples where (tblEventID = @tblEventID) GROUP BY DATEPART(QUARTER, CatchDate)">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            
                    <asp:SqlDataSource ID="sqlDSListArea" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select 'All' as Area, 'All' as Code,  0 as tblCodeID_Area
                                union all 
                                SELECT        dbo.tblCode.Description AS Area, dbo.tblCode.Code, dbo.tblCode.tblCodeID as tblCodeID_Area
                                FROM            dbo.tblSamples INNER JOIN
                                                         dbo.tblCode ON dbo.tblSamples.tblCodeID_AreaCode = dbo.tblCode.tblCodeID
                                WHERE        (dbo.tblSamples.tblEventID = @tblEventID)
                                GROUP BY dbo.tblCode.Description, dbo.tblCode.Code, dbo.tblCode.tblCodeID">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sqlExpertiseLevel" runat="server"  ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="
                        select 'All' as Expertise, -1 as ExpertiseLevel union all 
                        SELECT CASE ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE CASE ExpertiseLevel WHEN 0 THEN 'Basic' ELSE '' END END AS Expertise, dbo.tblAgeReadersSkills.ExpertiseLevel
                        FROM            dbo.tblAgeReadersSkills INNER JOIN
                                                 dbo.tblCode AS PreparationMethod ON dbo.tblAgeReadersSkills.tblCodeID_PreparationMethod = PreparationMethod.tblCodeID INNER JOIN
                                                 dbo.tblAnnotations ON dbo.tblAgeReadersSkills.SmartUser = dbo.tblAnnotations.SmartUser INNER JOIN
                                                 dbo.vw_PermissionForAnnotations ON dbo.tblAnnotations.tblAnnotationID = dbo.vw_PermissionForAnnotations.tblAnnotationID
                        WHERE        (dbo.tblAgeReadersSkills.Species =
                                                     (SELECT        Species
                                                       FROM            dbo.tblEvent
                                                       WHERE        (tblEventID = @tblEventID))) AND (dbo.tblAnnotations.tblEventID = @tblEventID) AND (dbo.vw_PermissionForAnnotations.SmartUser = @user)
                        GROUP BY  dbo.tblAgeReadersSkills.ExpertiseLevel">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                            <asp:SessionParameter Name="user" SessionField="user" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            &nbsp;</td>
            </tr>           
            </table>
            <br />
            <table cellpadding="0" cellspacing="0" style="width: 986px;">
            
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>EventID:</b> </td> <td> <asp:Label ID="lblEventID" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Name of the event:</b> </td> <td> <asp:Label ID="lblNameOfEvent" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Event type:</b> </td> <td> <asp:Label ID="lblEventType" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Start date:</b> </td> <td> <asp:Label ID="lblStartDate" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>End Date:</b> </td> <td> <asp:Label ID="lblEndDate" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Email of the Organizer:</b> </td> <td> <asp:Label ID="lblEmailOrganizer" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Species:</b> </td> <td> <asp:Label ID="lblSpecies" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Purpose:</b> </td> <td> <asp:Label ID="lblPurpose" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Closed:</b> </td> <td> <asp:Label ID="lblClosed" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>No. of Samples:</b> </td> <td> <asp:Label ID="lblNumberSamples" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>No. of Fish:</b> </td> <td> <asp:Label ID="lblNumberFish" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            </table>

            <br />
            <br />
           <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2"><asp:HyperLink ID="hplnkViewSummary" runat="server" Visible="false">Summary Annotations Per Age Reader</asp:HyperLink> </td>
            </tr>
            <tr>
                <td colspan="2"><asp:HyperLink ID="hplnkAnonimisedNameForSmartUsers" runat="server" Visible="false">Anonymized names to Smartuser</asp:HyperLink> </td>
            </tr>
            <tr>
                <td colspan="2"><asp:LinkButton ID="hplnkOpenAndCloseEvent" runat="server" Visible="false" OnClick="hplnkOpenEvent_Click">Close event</asp:LinkButton> </td>
            </tr>
            <tr>
                <td colspan="2"><asp:HyperLink ID="hplnkEditEvent" runat="server" Visible="false">Edit the event</asp:HyperLink> </td>
            </tr>
            <tr>
                <td colspan="2"><asp:HyperLink ID="hplnkDownloadData" runat="server"  Text="Download data" Visible="false"  ></asp:HyperLink> </td>
            </tr>
            <tr>
                <td colspan="2">
                  <asp:LinkButton ID="hplnkBttDownloadReport" runat="server" Visible="false" OnClick="hplnkBttDownloadReport_Click">Download report</asp:LinkButton> <asp:Label ID="lblDownloadReportLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2"><asp:HyperLink ID="hplnkViewReportLog" runat="server"  Text="Report log" Visible="false"  ></asp:HyperLink> </td>
            </tr>
            </table>
            <br />
            <br />
           <table id = "tableContent" border = "1" runat = "server" cellpadding="5" cellspacing="5" style="width: 986px;" ></table>
            <br />
         </div>
         <br />
         <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    </center>
        
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
