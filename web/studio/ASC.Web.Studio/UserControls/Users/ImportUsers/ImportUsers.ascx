<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportUsers.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.ImportUsers" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>


<!--[if IE]>
<style>
#wizard_users .checkall
{
    border:0;
    margin-top:13px;
}
</style> 
<![endif]-->

<!--[if lte IE 8]>
<style>
  .fistable
  {
   width:540px;
   padding: 0 12% 0 22%;
   cellpadding:20%;
   display:block;
  }
  #wizard_users #userList .userItem .name {
    width: 436px;
    padding-left:6px;
}

.fistable .desc
{
width:450px;
}

#wizard_users
{
    width:750px;
    padding-left:6px;
}

#wizard_users #userList .userItem .check
{
    padding:0 0 0 2px;
}


  </style>
<![endif]-->

<!--[if IE 9]>
<style>
#wizard_users #userList .userItem .check input
{
    margin:0 3px 0 2px;
}

#wizard_users #userList .userItem .name
{
width:438px;
}

#wizard_users #userList .userItem .name .firstname, #wizard_users #userList .userItem .name .lastname
{
    float:left;
    padding-right:14px;
    vertical-align:top;
}
  </style>
<![endif]-->




<div id="importUsers">

<div class="blockUI blockMsg blockElement" id="upload"><img/></div>
            <div class="desc">
                <%= ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("ImportContactsDescription").HtmlEncode()%></div>
                <div class="smallDesc"><span style="color:red;">*</span> <%= Resources.Resource.ImportContactsSmallDescription %></div>
            <div class="clearFix importUsers" id="panel">
                <div class="frame <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)?"framePad":"" %>">
                    <iframe src="<%= SetupInfo.GetImportServiceUrl() %>" style="border: none;
                        width: 575px;height:50px;overflow:hidden;filter: alpha(opacity=100);" frameBorder="0" id="ifr"></iframe>
                </div>
                 <div class="file" onclick="ImportUsersManager.ChangeVisionFileSelector();" title="From File" style="display:<%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)?"none":"block" %>;">
                    <%= Resources.Resource.ImportContactsFromFile %><div class="innerBox" style="float: right">
                    </div>
                    <div class="fileSelector" onclick="ImportUsersManager.ChangeVisionFileSelector();">
                        <ul>
                            <li id="import_flatUploader"><a href="javascript:void(0);"><%= Resources.Resource.ImportContactsFromFileCSV %></a></li>
                            <li id="import_msUploader"><a href="javascript:void(0);"><%= Resources.Resource.ImportContactsFromFileMS %></a></li>
                        </ul>
                    </div>
                </div>
                
            </div>
    <div id="wizard_users">
        <div class="clearFix <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)?"mob":"" %>" id="addUserBlock">
            <div class="checkall">
                <input type="checkbox" id="checkAll" onclick="ImportUsersManager.ChangeAll()" />
            </div>
            <div class="nameBox">
                <div class="error" id="fnError">
                    <%= Resources.Resource.ImportContactsErrorFirstName %>
                </div>
                <input type="text" id="firstName" class="textEdit" maxlength="64" />
            </div>
            <div class="lastnameBox">
            <div class="error" id="lnError">
                   <%= Resources.Resource.ImportContactsErrorLastName %>
                </div>
                <input type="text" id="lastName" class="textEdit" maxlength="64" />
            </div>
            <div class="emailBox">
            <div class="error" id="eaError">
                    <%= Resources.Resource.ImportContactsErrorEmail %>
                </div>
                <input type="text" id="email" class="textEdit" maxlength="64" />
            </div>
            <div class="<%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)?"":"btn" %>">
                <div class="btnBox">
                    <a class="grayLinkButton" id="saveSettingsBtn" href="javascript:void(0);" onclick="ImportUsersManager.AddUser();">
                        <%= Resources.Resource.ImportContactsAddButton %></a>
                </div>
            </div>
        </div>
        <div class="restr  <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)?"mob":"" %>">
            <table id="userList">
            </table>
        </div>
    </div>
      <div class="buttons">
        <a class="baseLinkButton" style="float: left;" href="#" onclick="ImportUsersManager.ImportList();">
                        <%=Resources.Resource.ImportContactsSaveButton%></a> 
                        <a class="grayLinkButton" style="float: left;
                            margin-left: 8px;" href="#" onclick="ImportUsersManager.DeleteSelected();">
                            <%= Resources.Resource.ImportContactsDeleteButton %>
                           </a>
                            
                        <a class="grayLinkButton" style="float: left;
                            margin-left: 8px;" href="#" onclick="ImportUsersManager.HideImportWindow();">
                            <%= Resources.Resource.ImportContactsCancelButton %></a>
      </div>
</div>

<table id="donor" style="display:none;">
    <tr>
        <td class="fistable">
        <div class="desc">
            <%= Resources.Resource.ImportContactsFirstable %>
            </div>
        </td>
    </tr>
</table>

<ascwc:Container ID="icon" runat="server">
    <Header>
     <%= Resources.Resource.ImportContactsErrorHeader %>
        </Header>
    <Body>
    <div>
    <%= String.Format(Resources.Resource.ImportContactsFromFileError,"<br />") %>
    </div>
    <div class="clearFix" style="width:100%;text-align:center;padding-top:30px;">
        <a class="baseLinkButton" href="#" onclick="ImportUsersManager.HideInfoWindow('okcss');">
             <%= Resources.Resource.ImportContactsOkButton %></a>
            </div>
    </Body>
</ascwc:Container>
<div class="blockUpload" id="blockProcess" style="display:none"></div>

 <script type="text/javascript">
     ImportUsersManager.FName = "<%= Resources.Resource.ImportContactsFirstName %>";
     ImportUsersManager.EmptyFName = "<%= Resources.Resource.ImportContactsEmptyFirstName %>";
     ImportUsersManager.LName = "<%= Resources.Resource.ImportContactsLastName %>";
     ImportUsersManager.EmptyLName = "<%= Resources.Resource.ImportContactsEmptyLastName %>";
     ImportUsersManager.Email = "<%= Resources.Resource.ImportContactsEmail %>";
     ImportUsersManager._mobile = <%= ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context).ToString().ToLower()%>;

     jq(document).click(function(event) {
        jq.dropdownToggle().registerAutoHide(event, ".file", ".fileSelector");
        jq('#upload img').attr('src',SkinManager.GetImage('mini_loader.gif'));
     });
     
 </script>

