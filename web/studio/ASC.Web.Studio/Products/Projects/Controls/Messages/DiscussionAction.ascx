<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionAction.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Messages.DiscussionAction" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<% if (IsMobile)
   { %>
<ascwc:JsHtmlDecoder id="JsHtmlDecoder" runat="Server">
</ascwc:JsHtmlDecoder>
<% } %>
<link href="<%=PathProvider.GetFileStaticRelativePath("discussionaction.css")%>"
    type="text/css" rel="stylesheet" />

<script src="<%= PathProvider.GetFileStaticRelativePath("discussionaction.js") %>"
    type="text/javascript"></script>

<script src="<%= VirtualPathUtility.ToAbsolute("~/js/ajaxupload.3.5.js") %>" type="text/javascript"></script>

<script type="text/javascript">
    discussionaction.init('<%= SecurityContext.CurrentAccount.ID %>', '<%= fckEditor.ClientID %>');
</script>
<% if (Discussion != null && Project != null)
   {%>
   <script>
       var text = "<%= Discussion.Title %>";
       if (text.length > 40) {
           text = text.slice(0, 37) + "...";
       }
       jq(".mainContainerClass >.containerHeaderBlock table td > div:first-child span:last-child").text(text);
       document.title = "<%= Discussion.Title %>" + " " + document.title;
   </script>
<% } %>
<% if (Discussion == null && Project == null)
   {%>
<div id="discussionProjectContainer" class="requiredField">
    <span class="requiredErrorText">
        <%= MessageResource.SelectProject %></span>
    <div class="headerPanel">
        <%= ProjectResource.Project %>
    </div>
    <select id="discussionProject" class="comboBox">
        <option value="-1">
            <%= ProjectsCommonResource.Select %></option>
    </select>
</div>

<script type="text/javascript">
    if (typeof (projects) != 'undefined') {
        projects = Teamlab.create('prj-projects', null, jq.parseJSON(jQuery.base64.decode(projects)).response);
        var projectId = jq.getURLParam('prjID');
        if (projectId) {
            jq('#discussionProject').empty();
        }
        for (var i = 0; i < projects.length; i++) {
            var option = document.createElement('option');
            option.setAttribute('value', projects[i].id);
            if (projectId && projectId == projects[i].id) {
                option.setAttribute('selected', 'selected');
            }
            option.appendChild(document.createTextNode(projects[i].title));
            if (projects[i].canCreateMessage) {
                jq('#discussionProject').append(option);
            }
        }
    }
</script>

<% } %>
<div id="discussionTitleContainer" class="requiredField">
    <span class="requiredErrorText">
        <%= ProjectsJSResource.EmptyMessageTitle %></span>
    <div class="headerPanel">
        <%= MessageResource.MessageTitle %>
    </div>
    <asp:TextBox ID="discussionTitle" Width="100%" runat="server" CssClass="textEdit"
        MaxLength="250" />
</div>
<div id="discussionTextContainer">
    <div class="headerPanel">
        <%= MessageResource.MessageContent %>
    </div>
    <% if (IsMobile)
       { %>
    <asp:TextBox ID="discussionContent" Width="100%" runat="server" CssClass="nonstretch"
        TextMode="MultiLine" Rows="12"></asp:TextBox>

    <script type="text/javascript">
        var node = jq('<div>' + jq('[id$=discussionContent]').val() + '</div>').get(0);
        jq('[id$=discussionContent]').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
    </script>

    <% }
       else
       { %>
    <FCKeditorV2:FCKeditor ID="fckEditor" Width="100%" Height="275px" runat="server">
    </FCKeditorV2:FCKeditor>
    <% } %>
</div>
<ul id="discussionTabs">
    <% if (Discussion != null)
       { %>
    <li container="discussionFilesContainer">
        <%= ProjectsCommonResource.DocsModuleTitle %></li>
    <% } %>
    <li class="current" container="discussionParticipantsContainer">
        <%= MessageResource.DiscussionParticipants %></li>
</ul>
<div id="discussionTabsContent">
    <div id="discussionParticipantsContainer">
        <div class="inviteMessage">
            <%= MessageResource.SubscribePeopleInfoComment %></div>
        <asp:Repeater ID="discussionParticipantRepeater" runat="server">
            <ItemTemplate>
                <div class="discussionParticipant">
                    <span class="userLink" guid="<%# ((Participant)(Container.DataItem)).ID %>">
                        <%# HttpUtility.HtmlEncode(((Participant)(Container.DataItem)).UserInfo.FirstName) + " " + HttpUtility.HtmlEncode(((Participant)(Container.DataItem)).UserInfo.LastName)%>
                    </span>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div style="clear: both;">
        </div>
        <span id="addDiscussionParticipantButton"><a class="dottedLink">
            <%= ProjectsCommonResource.AddParticipants %></a> </span>
        <asp:PlaceHolder ID="discussionParticipantsSelectorHolder" runat="server"></asp:PlaceHolder>
    </div>
    <div id="discussionFilesContainer">
        <asp:PlaceHolder runat="server" ID="discussionFilesPlaceHolder" />
    </div>
</div>
<div id="discussionButtonsContainer">
    <%= GetDiscussionAction()%>
</div>
<div id='discussionActionsInfoContainer' class='pm-ajax-info-block'>
    <span class="textMediumDescribe">
        <%= ProjectResource.LoadingWait %></span><br />
    <img src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"
        alt="<%= ProjectResource.ProjectAdded %>" />
</div>
<div id="discussionPreviewContainer">
    <a id="hideDiscussionPreviewButton" class="baseLinkButton">
        <%= ProjectsCommonResource.Collapse %></a>
</div>

<script id="discussionTemplate" type="text/x-jquery-tmpl">
    <div class="discussionContainer">
        <div class="discussionHeaderContainer">
            <span class="discussionHeader">${title}</span>
        </div>
        <div class="discussionAuthorAvatar">
            <img src="${authorAvatarUrl}" alt="${authorName}"/>
        </div>
        <div class="discussionDescriptionContainer">
            <div>
                <a class="discussionAuthor" href="javascript:void(0)">
                    ${authorName}
                </a>
                <span class="discussionCreatedDate">
                    ${createOn}
                </span>
            </div>
            <div class="discussionAuthorTitle">
                ${authorTitle}
            </div>
            <div class="discussionText">
                {{html content}}
            </div>
            <div style="clear: both;"></div>
        </div>
    </div>
</script>

