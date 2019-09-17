<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEvent.aspx.cs" Inherits="WebInterface.ViewEvent" %>
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
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ViewListEvents.aspx">View list events</a> &gt; View an event</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        <br />
        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;font-size:14px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
            <table cellpadding="0" cellspacing="0" style="width: 986px;">
            
            <tr>
                <td colspan="2">
            <table  cellpadding="0" cellspacing="0" style="width: 986px;">
                <tr>
                <td style="width: 220px;background-color:#F15D2A;color:white"></td>
                <td style="width: 220px;background-color:#F15D2A;color:white"></td>
                <td style="width: 106px;background-color:#F15D2A;color:white"></td>
                    </tr>
                
                <tr><td colspan="5">&nbsp;</td></tr>
                <tr>
                <td style="width: 220px;background-color:#FFFFFF"> <asp:DropDownList ID="ddlListQuarter" runat="server" AutoPostBack="true" DataSourceID="sqlQuarter" DataTextField="Quarter" DataValueField="Quarter" CssClass="comboGrupo" Visible="false"></asp:DropDownList></td>
                <td style="width: 220px;background-color:#FFFFFF"><asp:DropDownList ID="ddlListArea" runat="server" AutoPostBack="true" DataSourceID="sqlDSListArea" DataTextField="Code" DataValueField="tblCodeID_Area" CssClass="comboGrupo" Visible="false"></asp:DropDownList></td>
                <td style="width: 106px;background-color:#FFFFFF"><asp:DropDownList ID="ddlMode" AutoPostBack="True" CssClass="comboGrupo" DataTextField="Mode" DataValueField="intMode" runat="server" DataSourceID="sqlMode" Visible="false" OnSelectedIndexChanged="ddlMode_SelectedIndexChanged"></asp:DropDownList>
                    <asp:SqlDataSource ID="sqlMode" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select -1 as intMode, 'All' as Mode UNION ALL select Mode AS intMode, Mode  from ( SELECT        dbo.getMode(tblSmartImageID) AS intMode, cast(dbo.getMode(tblSmartImageID) as varchar) AS Mode  FROM dbo.vw_SmartImages  where (tblEventID = @tblEventID)  GROUP BY dbo.getMode(tblSmartImageID) ) as M where Mode is not null order by intmode">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="key" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    </td>
                    </tr>
            </table>

            
                    <asp:SqlDataSource ID="sqlQuarter" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="Select 'All' as Quarter union all SELECT      cast(DATEPART(QUARTER, CatchDate) as varchar) AS Quarter FROM            dbo.tblSamples where (tblEventID = @tblEventID) GROUP BY DATEPART(QUARTER, CatchDate)">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="key" />
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
                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="key" />
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
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Species:</b> </td> <td> <asp:Label ID="lblSpecies" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Purpose:</b> </td> <td> <asp:Label ID="lblPurpose" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Closed:</b> </td> <td> <asp:Label ID="lblClosed" runat="server" Text="" Font-Size="14px"></asp:Label></td>
            </tr>
               <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Email of the Organizer:</b> </td> <td> <asp:Label ID="lblEmailOrganizer" runat="server" Text=""  Font-Size="14px"></asp:Label>  </td>
            </tr>
              <tr>
                <td colspan="2">
                  
                </td>
              </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Report:</b> </td> <td> <asp:HyperLink ID="hplnkReport" runat="server" Text="Download Report" Target="_blank" Font-Size="14px"></asp:HyperLink></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Report summary:</b> </td> <td> <asp:HyperLink ID="hplnkReportSummary" runat="server" Text="Download Report summary" Target="_blank" Font-Size="14px"></asp:HyperLink></td>
            </tr>
            <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Data:</b> </td> <td> <asp:HyperLink ID="hplnkDownloadData" runat="server"  Text="Download data"  ></asp:HyperLink></td>
            </tr>
            </table>
            <br />
            <br />
          <div style="text-align:left">
            <table style="width:980px">
              <tr>
                <td> Sort event by:</td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=FishWeight&order=asc">Fish Weight Asc </a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=Fishlength&order=asc">Fish Length Asc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=mode&order=asc">Mode Asc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=Area&order=asc">Area Asc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=FishID&order=asc">Fish ID Asc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=NoTotalAnnotations&order=asc">No. Annotations Asc </a></td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=FishWeight&order=desc">Fish Weight Desc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=FishWeight&order=desc">Fish Length Desc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=mode&order=desc">Mode Desc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=Area&order=desc">Area Desc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=FishID&order=desc">Fish ID Desc</a></td>
                <td><a href="ViewEvent.aspx?key=<%= Request.QueryString["key"].ToString() %>&field=NoTotalAnnotations&order=desc">No. Annotations Desc</a></td>
              </tr>
            </table>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
          </div>
            <br />
            <br />
            <table id = "tableContent" border = "1" runat = "server" cellpadding="5" cellspacing="5" style="width: 986px;" ></table>
            <br />
            <br />
         </div>
         <br />
         <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    </center>       
    </form>
      <!--#include file="footer.html"-->

</body>
</html>
