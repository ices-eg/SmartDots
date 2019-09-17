<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublishEvent.aspx.cs" Inherits="WebInterface.manage.PublishEvent" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots - Manage the events</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" />
    </head>
<body>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
        <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; View an event</td>
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
            <b>This page will make the event avalilable in the home page, the event details are:</b>
            <br /> 
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
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Select a report file:</b></td>
                <td> <asp:FileUpload ID="flUpReport" runat="server"></asp:FileUpload></td>
              </tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr>
                <td style="width:150px;font-size:14px;font-weight:bold"><b>Select a summary report file:</b></td>
                <td> <asp:FileUpload ID="flupSummaryReport" runat="server"></asp:FileUpload></td>
              </tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2">&nbsp;&nbsp;</td></tr>
              <tr><td colspan="2">Select one of the options option:</td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2" style="vertical-align:top">
                <asp:HyperLink ID="Original" ImageUrl="../images/Example.jpg" NavigateUrl="../images/Example.jpg" Target="_blank" ImageWidth="100px" runat="server"></asp:HyperLink> 

                <asp:RadioButton ID="rdbPicturesFreelyAvailable" GroupName="ImagesReporting" runat="server" Text=" Pictures on-line will use their original resolution" Checked="True"></asp:RadioButton>
              </td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2">
                <asp:HyperLink ID="HyperLink1" ImageUrl="../images/ExampleLowResolution.jpg" NavigateUrl="../images/ExampleLowResolution.jpg" Target="_blank" ImageWidth="100px" runat="server"></asp:HyperLink>
                <asp:RadioButton ID="rdbPicturesLowResolution" runat="server" GroupName="ImagesReporting" Text=" Pictures on-line will use low resolution (640 x 480)"></asp:RadioButton>
                

                  </td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2"><asp:HyperLink ID="HyperLink2" ImageUrl="../images/ExampleWaterMark.png" NavigateUrl="../images/ExampleWaterMark.png" Target="_blank" ImageWidth="100px" runat="server"></asp:HyperLink> <asp:RadioButton ID="rdbPicturesLowResolutionWaterMark" GroupName="ImagesReporting" runat="server" Text=" Pictures on-line will use low resolution (640 x 480) with SmartDots watermark"></asp:RadioButton>
              

                  </td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2"><asp:CheckBox ID="chkMakePublicCopy" runat="server" Text="Make an empty copy of the event for training" Visible="false" Checked="true" Font-Size="16px" ForeColor="#666666"></asp:CheckBox></td></tr>
              <tr><td colspan="2">&nbsp;</td></tr>
              <tr><td colspan="2">&nbsp;<asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label></td></tr>
              <tr><td colspan="2">
                <asp:Button ID="bttPublishEvent" runat="server" OnClick="bttPublishEvent_Click" Text="Publish event" />
                </td></tr>
              <tr><td colspan="2">
                <asp:Button ID="fillInImageSize" runat="server"  Text="Update Images for all events" Visible="false" OnClick="fillInImageSize_Click" />
                </td></tr>

              <tr><td colspan="2">&nbsp;</td></tr>
            </table>
         </div>
         <br />
    </center>
        
    </form>
    <!--#include file="../footer.html"-->

</body>
</html>
