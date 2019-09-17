<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateEvent.aspx.cs" Inherits="WebInterface.admin.CreateEvent" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots</title>
     <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" />
</head>
<body>
    <div class="corner-ribbon top-right sticky red shadow">BETA</div>
    <!--#include file="../header.html"-->
    <form id="form1" runat="server">
    <div>
    <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; Propose a new event</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </center>
      <center>
             
              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="bttCreaveEvent"/>
                     </Triggers>
                <ContentTemplate>

                    <br />

                        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;font-size:14px">
                                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                                <br />
                                <br />
                    
                                <br />
                                 <table style="width:990px">
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> &nbsp;&nbsp;</td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> Propose a new event</td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> &nbsp; </td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:16px" class="auto-style1"> Set up the event details and upload the samples meta-data and images</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Purpose of the event
                                        </td>
                                        <td>
                                           <asp:DropDownList ID="ddlPurpose" runat="server">
                                               <asp:ListItem Value="Age reading" Text="Age reading"></asp:ListItem>
                                               <asp:ListItem Value="Maturity determination" Text="Maturity determination"></asp:ListItem>
                                           </asp:DropDownList>
                                            
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Type of event
                                        </td>
                                        <td>
                                           <asp:DropDownList ID="ddlTypeEvent" runat="server" DataSourceID="sqlDSTypeEvent" DataTextField="Code" DataValueField="tblCodeID"></asp:DropDownList>
                                            <asp:SqlDataSource ID="sqlDSTypeEvent" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [tblCodeID], [Code] FROM [tblCode] WHERE ([tblCodeGroupID] = @tblCodeGroupID)">
                                                <SelectParameters>
                                                    <asp:Parameter DefaultValue="7" Name="tblCodeGroupID" Type="Int32" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the species (latin name)
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSpecies" runat="server" DataSourceID="sqlSpecies" DataTextField="Code" DataValueField="tblCodeID"></asp:DropDownList>
                                            <asp:SqlDataSource ID="sqlSpecies" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT [tblCodeID], [Code], [Description], [tblCodeGroupID] FROM [tblCode] WHERE ([tblCodeGroupID] = @tblCodeGroupID) order by Code">
                                                <SelectParameters>
                                                    <asp:Parameter DefaultValue="3" Name="tblCodeGroupID" Type="Int32" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                     <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the event name 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEventName" runat="server" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEventName" ErrorMessage="Please fill in the event name" ForeColor="Red"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the e-mail address 
                                        </td>
                                        <td>
                                            <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                        </td>
                
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Delegate the event (write the email)
                                        </td>
                                        <td>
                                           <asp:TextBox ID="txtDelegator1" runat="server" ></asp:TextBox>
                                            <br />
                                           <asp:TextBox ID="txtDelegator2" runat="server" Visible="false" ></asp:TextBox>
                                            
                                        </td>
                                    </tr>

                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the startdate
                                        </td>
                                        <td>
                                             <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox> 
                                            <asp:Image ID="imgCalendar1" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" />
                                             <ajaxtoolkit:calendarextender ID="CalendarExtender1" runat="server" Format="yyyy/MM/dd" TargetControlID="txtStartDate" PopupButtonID="imgCalendar1"></ajaxToolkit:CalendarExtender>
                                             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Please fill in the start date" ForeColor="Red"></asp:RequiredFieldValidator>
                                        </td>
                
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the end date
                                        </td>
                                        <td>
                                             <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox> 
                                            <asp:Image ID="imgCalendar2" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" />
                                             <ajaxtoolkit:calendarextender ID="CalendarExtender2" runat="server" Format="yyyy/MM/dd" TargetControlID="txtEndDate" PopupButtonID="imgCalendar2"></ajaxToolkit:CalendarExtender>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEndDate" ErrorMessage="Please fill in the end date" ForeColor="Red"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compareDates" runat="server" controltovalidate="txtStartDate" controltocompare="txtEndDate"  operator="LessThan" type="Date" ErrorMessage="The end date needs to be after the begining date"  ForeColor="Red"></asp:CompareValidator>
                                        </td>
                
                                    </tr>


                                    <tr><td> &nbsp;</td><td></td></tr>
                                    <tr>
                                        <td> Select the Samples File to validate (Maximum 50Mb's)</td> <td><asp:FileUpload id="UploadFile" runat="server" size="60"></asp:FileUpload></td>
                                    </tr>
                                        <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td>&nbsp;</td> <td>
                                        <asp:Button ID="bttCreaveEvent" runat="server" Text=" Create Event " OnClick="bttCreateEvent_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="font-size:12px;color:red"> 
                                            <asp:Label ID="lblMessage" runat="server" Text="Message" ForeColor="Red" Visible="False"></asp:Label>
                                             <br /><br /><asp:HyperLink ID="hlnkResult" runat="server">View file screening</asp:HyperLink>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2"> &nbsp;</td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2" > Convert from Excel to XML <a href="../uploads/SmartDots_Reporting_Format.xlsm">  download template</a> </td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>

                                     </table>
                            </div>
                    <br />
            </ContentTemplate>
         </asp:UpdatePanel>
    </center>
    </div>
    </form>
    <!--#include file="../footer.html"-->
</body>
</html>
