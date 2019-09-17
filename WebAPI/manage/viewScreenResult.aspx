<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewScreenResult.aspx.cs" Inherits="Webinterface.manage.viewScreenResult" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smartdots - Check a file</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" /></head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
    <div>
<center>
    <br />
    <table style="width: 986px">
        <tr>
            <td  valign="top" class="label">&nbsp;</td>
            <td  valign="top" class="value">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2" style="font-size:16px;font-weight:bold" >
                Result from the ICES DATa Screening Utility program for the following data:
                </td>
        </tr>
        <tr>
            <td  valign="top" class="label">&nbsp;</td>
            <td  valign="top" class="value">&nbsp;</td>
        </tr>
        <tr>
            <td  valign="top" class="label">Institute:</td>
            <td  valign="top" class="value> <asp:Label ID="lblCountry" runat="server" Text="."></asp:Label></td>
        </tr>
        <tr>
            <td  valign="top" class="label">Dataset:</td>
            <td  valign="top" class="value> <asp:Label ID="lblDataset" runat="server" Text="."></asp:Label></td>
        </tr>
        <tr>
            <td  valign="top" class="label">FileName:</td>
            <td  valign="top" class="value> <asp:Label ID="lblFileName" runat="server" Text="."></asp:Label></td>
        </tr>
        <tr>
            <td  valign="top" class="label">Email:</td>
            <td  valign="top" class="value> <asp:Label ID="lblEmail" runat="server" Text="."></asp:Label></td>
        </tr>
        <tr>
            <td  valign="top" class="label">Year:</td>
            <td  valign="top" class="value> <asp:Label ID="lblMYear" runat="server" Text="."></asp:Label></td>
        </tr>
        <tr>
            <td  valign="top" class="label">Submiting date:</td>
            <td  valign="top" class="value> <asp:Label ID="lblSubmitingDate" runat="server" Text="."></asp:Label></td>
        </tr>
      
    </table>
    <br/>
        <table style="width: 986px"><tr><td>
                        <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" DataSourceID="SQLRowsSummary" Width="400px">
            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Record" HeaderText="Record" ReadOnly="True" SortExpression="Record" />
                                <asp:BoundField DataField="Rows" HeaderText="Rows" ReadOnly="True" SortExpression="Rows" />
                            </Columns>
                            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />

                        </asp:GridView>
                                        <asp:SqlDataSource ID="SQLRowsSummary" runat="server" ConnectionString="<%$ ConnectionStrings:DATSUCS %>" SelectCommand="SELECT     Record, Rows, SessionID FROM         (SELECT     'record: Header' AS Record, STR(COUNT(*)) + ' Row(s)' AS Rows, SessionID FROM dbo.tbl_129_21274_HR GROUP BY SessionID UNION ALL SELECT     'record: Samples' AS Record, STR(COUNT(*)) + ' Row(s)' AS Rows, SessionID FROM         dbo.tbl_129_21274_SR GROUP BY SessionID UNION ALL SELECT     'record: NE' AS Record, STR(COUNT(*)) + ' Row(s)' AS Rows, SessionID FROM         dbo.tbl_123_20037_NE GROUP BY SessionID UNION ALL SELECT     'record: NM' AS Record, STR(COUNT(*)) + ' Row(s)' AS Rows, SessionID FROM         dbo.tbl_123_20037_NM GROUP BY SessionID) AS t WHERE SessionID = @SessionID">
                                            <SelectParameters>
                                                <asp:QueryStringParameter Name="SessionID" QueryStringField="SessionID" />
                                            </SelectParameters>
                        </asp:SqlDataSource>
                                        </td></tr></table>

     <br />
     <br />
     <br />
        <table style="width: 986px"><tr><td>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="grid_corner" DataSourceID="SLQDataSummary" CellPadding="4" ForeColor="#333333" GridLines="None" Width="480px">
            <Columns>
                <asp:BoundField DataField="ErrField" HeaderText="Number of records per record type:" SortExpression="ErrField"  HeaderStyle-HorizontalAlign="Left" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
                <asp:BoundField DataField="ErrValue" HeaderText="" SortExpression="ErrValue" />
            </Columns>
            <AlternatingRowStyle BorderStyle="None" CssClass="Alternate" BackColor="White" />
            <RowStyle BackColor="#F0F0F0" />
            <HeaderStyle BackColor="#D2D2D2" Font-Bold="True" VerticalAlign="Bottom"  ForeColor="White"/>
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
            <RowStyle ForeColor="#000066" />
        </asp:GridView>
        <asp:SqlDataSource ID="SLQDataSummary" runat="server" ConnectionString="<%$ ConnectionStrings:DATSUCS %>"
            SelectCommand="SELECT tblSU_SessionID, CheckID, Line, ErrValue, ErrField FROM tblSU_ReportedErrors WHERE (Line IS NULL) AND (tblSU_SessionID = @sessionid) order by ErrField">
            <SelectParameters>
                <asp:QueryStringParameter Name="sessionid" QueryStringField="sessionid" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlHeaderDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:DATSUCS %>"
            SelectCommand="SELECT tblSU_Session.tblSU_SessionID, c1.Code AS Country, tblSU_Session.email, c.Description, tblSU_Session.sess_date, tblSU_Session.filename, tblSU_Session.records, tblSU_Session.usryear, tblSU_Session.errlimit, c.Code as DType  FROM tblSU_Session INNER JOIN tblSU_DatasetVersion ON tblSU_Session.datasetverID = tblSU_DatasetVersion.tblSU_DatasetVerID INNER JOIN tblCode c2 ON tblSU_DatasetVersion.DatVerID =  c2.tblCodeID INNER JOIN RECO..tblCode c ON c2.ForeignID = c.tblCodeID INNER JOIN tblCode c1 ON tblSU_Session.countryID = c1.tblCodeID WHERE tblSU_Session.tblSU_SessionID =  @sessionid">
            <SelectParameters>
                <asp:QueryStringParameter Name="sessionid" QueryStringField="sessionid" />
            </SelectParameters>
        </asp:SqlDataSource>

            <br />
    <asp:Panel ID="PnlErrors" runat="server" Width="100%">    
        <br />
        <asp:Label ID="lblMessage" runat="server" Text="" Visible="false" />
    <br />  
        <asp:HyperLink ID="hplnkViewReport" runat="server" NavigateUrl="http://admin.ices.dk/DatsuReports/DatsuErrorReport.aspx?id=" Visible="false" Target="_blank">View full report (can export to XLS or PDF)</asp:HyperLink>   
        <br />
        &nbsp;<br />  
               <asp:HyperLink ID="hplnCleanFilter" runat="server" NavigateUrl="ScreenResult.aspx" Visible="false" Target="_blank">Clean search </asp:HyperLink>   

        </td></tr></table>
<br />
       <asp:GridView ID="gdvw_summaryErrors" runat="server" Width="986px" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="tblSU_CheckID" DataSourceID="sqlSummaryGroup" ForeColor="#333333" GridLines="None">
           <AlternatingRowStyle BackColor="White" />
           <Columns>
               <asp:BoundField DataField="NumberOfErrors" HeaderText="Number of Errors" ReadOnly="True" SortExpression="NumberOfErrors" HeaderStyle-HorizontalAlign="Left" />
               <asp:BoundField DataField="Severity" HeaderText="Type" SortExpression="Severity"  HeaderStyle-HorizontalAlign="Left" />
               <asp:TemplateField HeaderText="Reported Errors" SortExpression="Description">
                   <ItemTemplate>
                       <asp:HyperLink ID="hplnkNext" runat="server" NavigateUrl='ScreenResult.aspx' Text='<%# Bind("Description") %>'></asp:HyperLink>
                   </ItemTemplate>
               </asp:TemplateField>
               <asp:BoundField DataField="ErrField" HeaderText="Error Fields" SortExpression="ErrField"  HeaderStyle-HorizontalAlign="Left" />
           </Columns>
            <AlternatingRowStyle BorderStyle="None" CssClass="Alternate" BackColor="White" />
            <RowStyle BackColor="#FCE1D8" />
            <HeaderStyle BackColor="#F15D2A" Font-Bold="True" VerticalAlign="Bottom"  ForeColor="White"/>
            <PagerStyle BackColor="White" ForeColor="#0000FF" HorizontalAlign="Left" />
            <RowStyle ForeColor="#000066" />
        </asp:GridView>                 
        <asp:SqlDataSource ID="sqlSummaryGroup" runat="server" ConnectionString="<%$ ConnectionStrings:DATSUCS %>" SelectCommand="
SELECT RE.tblSU_SessionID, CK.description AS Description, ER.description AS Severity, RE.ErrField, COUNT(RE.tblSU_ReportedErrorsID) AS NumberOfErrors, CK.tblSU_CheckID  
        FROM dbo.tblSU_ReportedErrors AS RE INNER JOIN 
        dbo.tblSU_Check AS CK ON RE.CheckID = CK.tblSU_CheckID INNER JOIN 
        dbo.tblSU_Error AS ER ON CK.tblSU_ErrorID = ER.tblSU_ErrorID 
        WHERE (RE.tblSU_SessionID = @sessionID) AND (RE.CheckID &lt;&gt; 296) AND (RE.CheckID &lt;&gt; 312) 
        GROUP BY CK.description, ER.description, RE.ErrField,CK.tblSU_CheckID , RE.tblSU_SessionID
        ORDER BY COUNT(RE.tblSU_ReportedErrorsID) desc">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="0" Name="sessionID" QueryStringField="sessionid" />
            </SelectParameters>
        </asp:SqlDataSource>
    <br />
        <br />              
    
        <br />
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CssClass="grid_corner" DataSourceID="SQLdtsCheckErrors"  AllowPaging="True" CellPadding="4" ForeColor="#333333" GridLines="None" Width="986px" AllowSorting="True" PageSize="100">
            <Columns>
                <asp:BoundField DataField="Line" HeaderText="Record Line" SortExpression="Line" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
                <asp:BoundField DataField="type" HeaderText="Type" SortExpression="type" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
                <asp:BoundField DataField="Description" HeaderText="Reported errors" SortExpression="ErrValue" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
                <asp:BoundField DataField="ErrValue" HeaderText="Error Values" SortExpression="ErrValue" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
                <asp:BoundField DataField="ErrField" HeaderText="Error fields" SortExpression="ErrField" HeaderStyle-Height="20px" HeaderStyle-VerticalAlign="Middle" HeaderStyle-Font-Size="Larger"/>
            </Columns>
            <AlternatingRowStyle BorderStyle="None" CssClass="Alternate" BackColor="White" />

           <HeaderStyle BackColor="#71B5BC" Font-Bold="True" ForeColor="White" />
           <RowStyle BackColor="#E9F4F5" />
           <PagerStyle BackColor="#71B5BC" ForeColor="White" HorizontalAlign="Center"  />

        </asp:GridView>
        <asp:SqlDataSource ID="SQLdtsCheckErrors" runat="server" ConnectionString="<%$ ConnectionStrings:DATSUCS %>"
            SelectCommand="SELECT  tblSU_ReportedErrors.tblSU_SessionID, tblSU_ReportedErrors.Line, tblSU_Error.description AS type, tblSU_ReportedErrors.ErrValue,  tblSU_ReportedErrors.ErrField, tblSU_Check.description, tblSU_Check.CriticalOnDome FROM         tblSU_ReportedErrors INNER JOIN                      tblSU_Check ON tblSU_ReportedErrors.CheckID = tblSU_Check.tblSU_CheckID INNER JOIN                    tblSU_Error ON tblSU_Check.tblSU_ErrorID = tblSU_Error.tblSU_ErrorID WHERE    ( ErrField not like  'Summaries of data: Timeout expired%' ) and (tblSU_ReportedErrors.Line IS NOT NULL) AND (tblSU_ReportedErrors.tblSU_SessionID = @sessionid)  ">
            <SelectParameters>
                <asp:QueryStringParameter Name="sessionid" QueryStringField="sessionid" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" Width="676px"></asp:Label></asp:Panel>
    &nbsp;&nbsp;<br />
    &nbsp;
    <br />
    &nbsp;
    <br />
    <br />
        </center>        
    </div>
    </form>
    <!--#include file="../footer.html"-->
</body>
</html>
