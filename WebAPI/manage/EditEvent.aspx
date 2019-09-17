<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEvent.aspx.cs" Inherits="Webinterface.manage.EditEvent" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SmartDots</title>
    <link href="../css/Default.css" rel="stylesheet" />
    <link href="../css/ribbon.css" rel="stylesheet" />
    <link rel="stylesheet" href="http://www.ices.dk/assets/style_sheets/ices_styles.css" type="text/css" />
    <style type="text/css">
      .auto-style1 {
        height: 14px;
      }
    </style>
</head>
<body>
    <!--#include file="../header.html"-->

    
    <form id="form1" runat="server">
    <script type="text/javascript" >
     function uploadComplete( sender) {
         __doPostBack("<%= Button1.UniqueID %>", "");
        }
    </script>
    <div>
    <center>
        <table cellpadding="0" cellspacing="0" style="width: 986px;">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" class="auto-style2"><a href="../index.aspx">ICES SmartDots database</a> &gt; <a href="ListOperations.aspx">Manage events and users</a> &gt; <a href="ManageEvent.aspx">List of Events</a> &gt; Edit an event</td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </center>
             <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>

              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                <asp:PostBackTrigger ControlID="bttAddSamples" />
                <asp:PostBackTrigger ControlID="AddSamples" />
                </Triggers>
                <ContentTemplate>
                    <br />

                        <div style="width: 990px; margin-left: auto; margin-right: auto; margin-top: 0;font-size:14px">
                                    <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblEventName" runat="server" Text=""></asp:Label></div>
                                <br />
                                    <div style="font-weight:bold;font-size:18px"> <asp:Label ID="lblPurpose" runat="server" ForeColor="#009933" Text=""></asp:Label></div>
                                <br />
                                <br />
                                    <asp:LinkButton ID="lnkDeleteEvent" runat="server" Visible="false" ForeColor="Red" OnClick="lnkDeleteEvent_Click" >Delete the event</asp:LinkButton>
                                <br />
                                    <asp:HyperLink runat="server" ID="hpnkViewEvent" NavigateUrl="~/manage/ViewEvent.aspx?tblEventID=">View event</asp:HyperLink>
                                <br />
                                    <asp:LinkButton ID="lnkCloseEvent" runat="server" OnClick="lnkCloseEvent_Click">Close the event</asp:LinkButton>
                                <br />
                                    <asp:LinkButton ID="lnkPublishEvent" runat="server" OnClick="lnkPublishEvent_Click" Visible="false" >Publish the event</asp:LinkButton>
                                <br />
                                    <asp:LinkButton ID="lnkEditEventDetails" runat="server" OnClick="lnkEditEventDetails_Click">Edit Event Details</asp:LinkButton>
                                <br />
                                    <asp:HyperLink ID="hplnkDownloadData" runat="server"  Text="Download data" Visible="false"  ></asp:HyperLink>
                                <br />
                                    <asp:LinkButton ID="hplnkBttDownloadReport" runat="server" Visible="false" OnClick="hplnkBttDownloadReport_Click">Download report</asp:LinkButton> <asp:Label ID="lblDownloadReportLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
                                <br />
                                    <asp:HyperLink ID="hplnkViewReportLog" runat="server"  Text="Report log" Visible="false"  ></asp:HyperLink> 
                                <br /> 
                                <br />
                                <asp:Panel ID="pnlEditEventDetails" runat="server" Visible="false">
                                 <table style="width:990px">
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> &nbsp;&nbsp;</td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> </td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:18px"> &nbsp; </td></tr>
                                    <tr><td colspan="2" style="font-weight:bold;font-size:16px" class="auto-style1"> Set up the event details and upload the samples meta-data and images</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td class="auto-style1"> Type of event
                                        </td>
                                        <td class="auto-style1">
                                            <asp:Label ID="lblTypeOfEvent" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Enter the species (latin name)
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSpecies" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                     <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Event name 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEventName" runat="server" ></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> E-mail address 
                                        </td>
                                        <td>
                                            <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                        </td>
                
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> Startdate
                                        </td>
                                        <td>
                                             <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox> 
                                            <asp:Image ID="imgCalendar1" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" />
                                             <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy/MM/dd" TargetControlID="txtStartDate" PopupButtonID="imgCalendar1"></ajaxToolkit:CalendarExtender>
                                        </td>
                
                                    </tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr><td colspan="2"> &nbsp;</td></tr>
                                    <tr>
                                        <td> End date
                                        </td>
                                        <td>
                                             <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox> 
                                            <asp:Image ID="imgCalendar2" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" />
                                             <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" Format="yyyy/MM/dd" TargetControlID="txtEndDate" PopupButtonID="imgCalendar2"></ajaxToolkit:CalendarExtender>
                                        </td>
                
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td> <td>
                                            <asp:Label ID="lblEditEventLabel" runat="server" Text="" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td> <td>
                                        <asp:Button ID="bttEditEvent" runat="server" Text="Update Event" OnClick="bttEditEvent_Click"  />
                                        </td>
                                    </tr>

                                 </table>

                            <br />
                        </asp:Panel>
                            <br /><br />
                            <b>List of the event delegates (maximum of 6 delegates) </b>
                            <br />
                            Add a delegate to the event <asp:TextBox ID="txtDelegateName" runat="server" CssClass="comboGrupo"></asp:TextBox> <asp:Button ID="bttAddDelegate" runat="server" Text="Add" OnClick="bttAddDelegate_Click" />
                            <br />
                            <asp:Label ID="lblAddDelagateText" runat="server" Text="" ForeColor="Red"></asp:Label>
                            <br /><br />
                            <asp:GridView ID="gvEventDelegates" runat="server" AutoGenerateColumns="False" DataKeyNames="SmartUser,Email" DataSourceID="sqlEventDelegates" Width="500px" OnRowDataBound="gvEventDelegates_RowDataBound">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                              <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:ImageButton AlternateText="Delete user" ToolTip="Delete user" ID="imgDeleteUserDelegation"  CommandName="Delete" runat="server" ImageUrl="~/icons/user_delete.png" OnClientClick='return confirm("Do you really want to remove this age reader from managing event?");'  CausesValidation="False" Width="16px" Height="16px"/>
                                            </ItemTemplate>
                                            <HeaderStyle Width="20px" />
                                            <ControlStyle Width="15px"  CssClass="centerAlign"/>
                                        </asp:TemplateField>        

                                <asp:BoundField DataField="role" HeaderText="Role" ReadOnly="True" SortExpression="role" />
                                <asp:BoundField DataField="SmartUser" HeaderText="SmartUser" ReadOnly="True" SortExpression="SmartUser" />
                              </Columns>
                                    <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="sqlEventDelegates" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" SelectCommand="SELECT        t.tblEventID, t.Email, t.role, dbo.tblDoYouHaveAccess.SmartUser
FROM            (SELECT        tblEventID, OrganizerEmail AS Email, 'organizer' AS role
                          FROM            dbo.tblEvent
                          UNION ALL
                          SELECT        tblEventID, Value AS Value, 'delegate' AS role
                          FROM            dbo.tblEventData) AS t INNER JOIN
                         dbo.tblDoYouHaveAccess ON t.Email = dbo.tblDoYouHaveAccess.Email
WHERE        (t.tblEventID = @tblEventID)
"
                                      DeleteCommand="Delete FROM [tblEventData] WHERE [tblEventID] = @tblEventID and Value = @Email"  >
                                      <SelectParameters>
                                        <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                                      </SelectParameters>
                                        <DeleteParameters>
                                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                                            <asp:Parameter Name="Email" />
                                        </DeleteParameters>

                                    </asp:SqlDataSource>
                            <br />
                            <br />
                            <span style="font-weight:bold;color:#F15D2A">
                            <asp:CheckBox ID="chkIncludeAQ3Annotations" runat="server" AutoPostBack="True" OnCheckedChanged="chkIncludeAQ3Annotations_CheckedChanged" /> 
                              Include AQ3 annotations in the download and report.
                            </span>
                            <br />
                            <br />
                            Set the scale of images to <asp:TextBox ID="txtScale" runat="server" CssClass="comboGrupo"></asp:TextBox>  where sample contains: <asp:TextBox ID="txtSamplesScaleContains" runat="server" CssClass="comboGrupo"></asp:TextBox> (leave empty to apply the scale to all images)  <asp:Button ID="bttChangeScale" runat="server" Text="Apply" OnClick="bttChangeScale_Click" />
                            <br />
                            Delete samples that contain: <asp:TextBox ID="txtSamplesDelete" runat="server" CssClass="comboGrupo"></asp:TextBox> <asp:Button ID="bttDeleteSamples" runat="server" Text="Delete Samples" OnClick="bttDeleteSamples_Click" />
                            <br />
                            Delete images from samples that contain: <asp:TextBox ID="txtSampleDeleteImages" runat="server" CssClass="comboGrupo"></asp:TextBox> <asp:Button ID="bttDeleteImages" runat="server" Text="Delete Images" OnClick="bttDeleteImages_Click"  />
                            <br />
                            Update strata to <asp:TextBox ID="txtStrataValue" runat="server" CssClass="comboGrupo"></asp:TextBox> where sample contain: <asp:TextBox ID="txtSampleContainsToUpdateStrata" runat="server" CssClass="comboGrupo"></asp:TextBox> <asp:Button ID="bttUpdateStrataWhereSample" runat="server" Text="Update strata" OnClick="bttUpdateStrataWhereSample_Click" />
                            <br />
                            Update strata to be same as: <asp:DropDownList ID="ddlStrataFields" runat="server">
                              <asp:ListItem Text="Area" Value="tblCodeID_AreaCode"></asp:ListItem>
                              <asp:ListItem Text="Maturity Stage" Value="tblCodeID_MaturityStage"></asp:ListItem>
                              <asp:ListItem Text="Stock Code" Value="tblCodeID_StockCode"></asp:ListItem>
                              <asp:ListItem Text="Type Of Structure" Value="tblCodeID_TypeOfStructure"></asp:ListItem>                              
                              <asp:ListItem Text="Preparation Method" Value="tblCodeID_PreparationMethod"></asp:ListItem>
                                                         </asp:DropDownList> <asp:Button ID="bttUpdateStrataToAnoterField" runat="server" Text="Update Strata" OnClick="bttUpdateStrataToAnoterField_Click" />
                            <br />
                            <asp:Label ID="lblMessageScale" runat="server" Text="" ForeColor="Red"></asp:Label>
                            <br />
                            
                            <asp:GridView ID="gv_SamplesAndImages" runat="server" Width="985px" AutoGenerateColumns="False" DataKeyNames="tblSampleID" DataSourceID="SQLSampleData" 
                                
                                OnRowDataBound="gv_SamplesAndImages_RowDataBound"  >
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <Columns>                                            
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HyperLink ID="imgButtonConnectedWithSamples" runat="server" ImageUrl="~/icons/notConnected.png" AlternateText="Sample is connected with an image" ToolTip="Is the sample is connected with an image" Width="16px" Height="16px"></asp:HyperLink>
                                            </ItemTemplate>
                                            <HeaderStyle Width="25px" />
                                            <ControlStyle Width="15px"  CssClass="centerAlign"/>
                                        </asp:TemplateField>      
                                        <asp:BoundField DataField="SampleID" HeaderText="SampleID" ReadOnly="True" SortExpression="SampleID" />
                                        
                                        <asp:BoundField DataField="CountImages" HeaderText="No. Linked Images" ReadOnly="True" SortExpression="CountImages" />
                                        <asp:BoundField DataField="CatchDate" HeaderText="Catch Date" ReadOnly="True" SortExpression="CatchDate" />
                                        <asp:BoundField DataField="Area" HeaderText="Area" SortExpression="Area" />
                                        <asp:BoundField DataField="StatRec" HeaderText="Stat Rec" SortExpression="StatRec" />
                                        <asp:BoundField DataField="StockCode" HeaderText="Stock Code" SortExpression="StockCode" />
                                        <asp:BoundField DataField="SampleOrigin" HeaderText="Sample Origin" SortExpression="SampleOrigin" />
                                        <asp:BoundField DataField="SampleType" HeaderText="Sample Type" SortExpression="SampleType" />
                                        <asp:BoundField DataField="PreparationMethod" HeaderText="Preparation Method" SortExpression="PreparationMethod" />
                                        <asp:BoundField DataField="FishLength" HeaderText="Fish Length" SortExpression="FishLength" />
                                        <asp:BoundField DataField="FishWeight" HeaderText="Fish Weight" SortExpression="FishWeight" />
                                        <asp:BoundField DataField="Sex" HeaderText="Sex" SortExpression="Sex" />
                                        <asp:BoundField DataField="Strata" HeaderText="Strata" SortExpression="Strata" />
                                    </Columns>
                                    <HeaderStyle BackColor="#F15D2A" Font-Bold="True" ForeColor="White" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SQLSampleData" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" DeleteCommand="Delete [dbo].[tblSamples]  where tblSampleID = @tblSampleID" 
                                      SelectCommand="SELECT        tblEventID, tblSampleID, SampleID, CatchDate, StatRec, FishLength, FishWeight, Comments, MaturityStage, MaturityScale, SampleType, SampleOrigin, Area, StockCode, Sex, PreparationMethod, GUID_Sample, GUID_EventID, 
                         CountImages, strata
FROM            dbo.vw_EventSamples
where tblEventID = @tblEventID">
                                        <DeleteParameters>
                                            <asp:Parameter Name="tblSampleID" />
                                        </DeleteParameters>
                                        <SelectParameters>
                                            <asp:QueryStringParameter Name="tblEventID" QueryStringField="tblEventID" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                          
                            <br />
                            <br />
                            List current sample data:
                            <br />
                                
                                     <asp:Label ID="lblAddSampleSuccess" runat="server" Text="" ForeColor="Green" Visible="false"></asp:Label><br />
                                     <asp:Label ID="lblAddSamplesError" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label><br />
                                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Show me all the green ones!"  CssClass="hiddenButton"  />
                                <br />
                                <br />
                                    <asp:LinkButton ID="lnkAddMoreSamplesToEvent" runat="server" OnClick="lnkAddMoreSamplesToEvent_Click">Add More sample data to the events</asp:LinkButton>
                                <br />
                                <br />
                                <br />
                                <asp:Panel ID="pnlUploadMoreSamples" runat="server" Visible="false">
                                    <br />
                                     <table style="width:990px">
                                            <tr><td> &nbsp;</td><td></td></tr>
                                            <tr>
                                                <td> Upload more sample data(Maximum 50Mb's)</td> <td>
                                                    <asp:FileUpload id="UploadFile" runat="server" size="30"></asp:FileUpload></td>
                                            </tr>
                                                <tr><td colspan="2">                                   
                                                    <asp:Button ID="bttAddSamples" runat="server" Text="Upload samples" OnClick="bttAddSamples_Click" /> &nbsp;</td></tr>
                                            <tr>
                                                <td colspan="2" style="font-size:12px;color:red"> 
                                                    <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red" Visible="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2"> </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2"> &nbsp;</td>
                                            </tr>
                                            <tr><td colspan="2"> &nbsp;</td></tr>
                                            <tr><td colspan="2" > Convert from Excel to XML <a href="../uploads/SmartDotsSampleFile.xlsm">  download template</a> </td></tr>
                                            <tr><td colspan="2"> &nbsp;</td></tr>
                                     </table>
                                <br /><br /><asp:Label ID="lblMessageSamples" runat="server" Text=""  ForeColor="Red" ></asp:Label>
                                <br /><br /><asp:HyperLink ID="hlnkResult" runat="server">View file screening</asp:HyperLink>
                            <br />
                            </asp:Panel>
                            <br /> Add sample images to the list of samples (allowed file types: jpg,gif,png):
                            <br />
                            <ajaxToolkit:AjaxFileUpload ID="AddSamples" runat="server"  MaximumNumberOfFiles="299" Width="800px"  AllowedFileTypes="png,jpeg,gif,jpg" 
                                    OnUploadComplete="AddSamples_UploadComplete" OnClientUploadCompleteAll="uploadComplete"  />
                                <br />
                            <br /> List of the age readers listed for this exercise:
                                    <br />
                                    <br />
                                    <asp:GridView ID="gv_ageReadersListedExercise" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="sqlDSAgeReadersInTheExercise"  GridLines="None" Width="985px" DataKeyNames="SmartUser,tblEventID" OnRowDeleted="gv_ageReadersListedExercise_RowDeleted">
                                        <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:ImageButton AlternateText="Delete user" ToolTip="Delete user" ID="ImageButton2"  CommandName="Delete" runat="server" ImageUrl="~/icons/user_delete.png" OnClientClick='return confirm("Do you really want to remove this age reader from the event?");'  CausesValidation="False" Width="16px" Height="16px"/>
                                            </ItemTemplate>
                                            <HeaderStyle Width="20px" />
                                            <ControlStyle Width="15px"  CssClass="centerAlign"/>
                                        </asp:TemplateField>        

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:ImageButton AlternateText="Edit user name" ToolTip="Edit user details" ID="ImgEdit"  CommandName="Edit" runat="server" ImageUrl="~/icons/user_edit.png"  CausesValidation="False" Width="16px" Height="16px"/>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                    <asp:ImageButton AlternateText="Update Record" ToolTip="Update Record" ID="ImgEdit"  CommandName="Update" runat="server" ImageUrl="~/icons/accept.png"  CausesValidation="False" Width="16px" Height="16px"/>
                                                    <asp:ImageButton AlternateText="Cancel Record Edit" ToolTip="Cancel Record Edit" ID="ImgCancel"  CommandName="Cancel" runat="server" ImageUrl="~/icons/cancel.png"  CausesValidation="False" Width="16px" Height="16px"/>
                                                </EditItemTemplate> 
                                            <HeaderStyle Width="20px" />
                                        </asp:TemplateField>        

                                            <asp:BoundField DataField="tblEventID" HeaderText="tblEventID" SortExpression="tblEventID" Visible="false"/>
                                             <asp:TemplateField HeaderText="Country" SortExpression="Country">
                                                <EditItemTemplate>
                                                    <asp:Label ID="LabelCountryabc1" runat="server" Text='<%# Bind("Country") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LabelCountryabcd1" runat="server" Text='<%# Bind("Country") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="SmartUser" SortExpression="SmartUser">
                                                <EditItemTemplate>
                                                    <asp:Label ID="Label11" runat="server" Text='<%# Bind("SmartUser") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("SmartUser") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                                <EditItemTemplate>
                                                    <asp:Label ID="LabelEmail11" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LabelEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="Number" HeaderText="No." SortExpression="Number" />


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
                                            <asp:CheckBoxField DataField="ProvidesDataForAssessment" HeaderText="ProvidesDataForAssessment" SortExpression="ProvidesDataForAssessment" ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                        <HeaderStyle BackColor="#71B5BC" Font-Bold="True" ForeColor="White" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    </asp:GridView>
                                    <br /> <asp:Button ID="buttonSuggestNo" runat="server" Text="Generate the no. of age readers" OnClick="buttonSuggestNo_Click" CssClass="comboGrupo" />
                                    &nbsp;
                                    <asp:Button ID="bttSendEmailToParticipants" runat="server" OnClick="bttSendEmailToParticipants_Click" Text="Send an email to age readers" CssClass="comboGrupo"/>
                                    <br />
                                    <asp:SqlDataSource ID="sqlDSAgeReadersInTheExercise" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                                        SelectCommand="SELECT        EP.SmartUser,EP.number, EP.Role, EP.ExpertiseLevel, EP.ProvidesDataForAssessment, CASE ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE 'Basic' END AS ExpertiseLevelText, EP.tblEventID, A.Email, Country.Description as Country FROM            dbo.tblEventParticipants AS EP INNER JOIN   dbo.tblDoYouHaveAccess AS A ON EP.SmartUser = A.SmartUser INNER JOIN dbo.tblCode AS Country ON A.tblCodeID_Country = Country.tblCodeID WHERE (EP.[tblEventID] = @tblEventID)"
                                        DeleteCommand="Delete FROM [tblEventParticipants] WHERE [tblEventID] = @tblEventID and SmartUser = @SmartUser"
                                        UpdateCommand="Update [tblEventParticipants]  set ProvidesDataForAssessment = @ProvidesDataForAssessment , ExpertiseLevel = @ExpertiseLevel , Number = @number WHERE [tblEventID] = @tblEventID and SmartUser = @SmartUser">
                                        <SelectParameters>
                                            <asp:QueryStringParameter DefaultValue="0" Name="tblEventID" QueryStringField="tblEventID" Type="Int32" />
                                        </SelectParameters>
                                        <DeleteParameters>
                                            <asp:Parameter Name="SmartUser"  Type="String" />
                                            <asp:Parameter Name="tblEventID"  Type="String" />
                                        </DeleteParameters>
                                        <UpdateParameters>
                                            <asp:Parameter Name="ProvidesDataForAssessment" />
                                            <asp:Parameter Name="ExpertiseLevelText" />                                            
                                            <asp:Parameter Name="Number" />                                            
                                            <asp:Parameter Name="SmartUser"  Type="String" />
                                            <asp:Parameter Name="tblEventID"  Type="String"  />
                                        </UpdateParameters>

                                    </asp:SqlDataSource>
                                    <asp:Label ID="lblNoAgeReadersIntheExercise" runat="server" Text="There are currently no age readers participating in this event!" Visible="False" Font-Size="Large" ForeColor="#71B5BC"></asp:Label>
                                    <br />
                            <br /> 
                            <br />Click below to add age readers to this exercise:
                             <br />
                            <br />
                                   <asp:LinkButton ID="lnkViewListOfAgeReaders" runat="server" OnClick="lnkViewListOfAgeReaders_Click" >View the list of  the age readers</asp:LinkButton>
                            <asp:Panel ID="pnlListAgeReaders" runat="server" Visible="true">
                                <asp:GridView ID="gv_ListUsersExperties" runat="server" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDSListAgeReadersExperties"  GridLines="None" Width="985px" AllowSorting="True" OnRowCommand="gv_ListUsersExperties_RowCommand" OnSelectedIndexChanged="gv_ListUsersExperties_SelectedIndexChanged">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton AlternateText="Add age reader to the event" ToolTip="Add age reader to the event" ID="ImageButton2"  CommandName="AddUser" runat="server" ImageUrl="~/icons/user_add.png" CausesValidation="False" Width="16px" Height="16px"
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="25px" />
                                        <ControlStyle Width="15px"  CssClass="centerAlign"/>
                                    </asp:TemplateField>      
                                <asp:BoundField DataField="Country" HeaderText="Country" ReadOnly="True" SortExpression="Country"  />
                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                <asp:BoundField DataField="Species" HeaderText="Species" SortExpression="Species" />
                                <asp:BoundField DataField="ExpertiseLevel" HeaderText="ExpertiseLevel" SortExpression="ExpertiseLevel" HeaderStyle-Width="140px" />
                                <asp:BoundField DataField="isCountryCoordinator" HeaderText="Country Coordinator" SortExpression="isCountryCoordinator" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                            <HeaderStyle BackColor="#0072c6" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        </asp:GridView>
        <asp:SqlDataSource ID="SqlDSListAgeReadersExperties" runat="server" ConnectionString="<%$ ConnectionStrings:SmartDotsCS %>" 
                SelectCommand="
                    SELECT     dbo.tblAgeReadersSkills.SmartUser, dbo.tblAgeReadersSkills.Species, 
                      CASE ExpertiseLevel WHEN 1 THEN 'Advanced' ELSE CASE ExpertiseLevel WHEN 0 THEN 'Basic' ELSE '' END END AS ExpertiseLevel, 
                      dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, UPPER(LEFT(Country.Description, 1)) + LOWER(RIGHT(Country.Description, 
                      LEN(Country.Description) - 1)) AS Country, 
                      CASE dbo.tblDoYouHaveAccess.isCountryCoordinator WHEN 1 THEN 'Yes' ELSE 'No' END AS isCountryCoordinator
                    FROM         dbo.tblAgeReadersSkills INNER JOIN
                                          dbo.tblCode AS PreparationMethod ON dbo.tblAgeReadersSkills.tblCodeID_PreparationMethod = PreparationMethod.tblCodeID RIGHT OUTER JOIN
                                          dbo.tblDoYouHaveAccess ON dbo.tblAgeReadersSkills.SmartUser = dbo.tblDoYouHaveAccess.SmartUser LEFT OUTER JOIN
                                          dbo.tblCode AS Country ON dbo.tblDoYouHaveAccess.tblCodeID_Country = Country.tblCodeID
                    where (dbo.tblAgeReadersSkills.Species =
                                              (SELECT     Species
                                                FROM          dbo.tblEvent
                                                WHERE      (tblEventID = @tblEventID)))
            and dbo.tblAgeReadersSkills.SmartUser not in (select smartuser from tblEventParticipants where tblEventID = @tblEventID)
                    GROUP BY dbo.tblAgeReadersSkills.SmartUser, dbo.tblAgeReadersSkills.Species, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, 
                                          UPPER(LEFT(Country.Description, 1)) + LOWER(RIGHT(Country.Description, LEN(Country.Description) - 1)),ExpertiseLevel, isCountryCoordinator" 
                CancelSelectOnNullParameter="False" EnableViewState="False"
                    InsertCommand="INSERT INTO [dbo].[tblEventParticipants]
                                    ([tblEventID],[SmartUser],[Role],[ExpertiseLevel],[ProvidesDataForAssessment])
                                    VALUES(@tblEventID,@SmartUser,0,0,0)">
                
            <SelectParameters>
                    <asp:QueryStringParameter DefaultValue="0" Name="tblEventID" QueryStringField="tblEventID" />
            </SelectParameters>  
            
        </asp:SqlDataSource>
                            </asp:Panel>
                              <br />
                                   <asp:Label ID="lblAddUsersMessage" runat="server" Text=""  ForeColor="Red" ></asp:Label>
       
                                <br />
                                <asp:TextBox ID="txtUsers" runat="server" style="margin-bottom: 0px" Height="100px" Width="463px" TextMode="MultiLine"></asp:TextBox>
                            <br /><asp:Button ID="bttAddUsers" runat="server" Text="Add Users" OnClick="bttAddUsers_Click"   />
                               <br />
                                Please fill in the SmartUser or the email to give access to SmartDots event. 
                                 <br />Note: Use comma's between names.                             
                            <br />
                       </div>
                    <br />
            </ContentTemplate>
         </asp:UpdatePanel>
    
    </div>
    </form>
    <!--#include file="../footer.html"-->
</body>
</html>