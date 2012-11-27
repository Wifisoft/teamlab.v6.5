<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionDetails.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Messages.DiscussionDetails" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Notify.Recipients" %>
<%@ Import Namespace="ASC.Projects.Engine" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Configuration" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<link href="<%= PathProvider.GetFileStaticRelativePath("discussiondetails.css") %>"
    type="text/css" rel="stylesheet" />

<script src="<%= PathProvider.GetFileStaticRelativePath("discussiondetails.js") %>"
    type="text/javascript"></script>

<%if(CanReadFiles){%>

<script type="text/javascript">
        //------ Attachments control init-------//
    jq(document).ready(function() {
        ASC.Controls.AnchorController.bind( /task_files/ , function() { ASC.Projects.TaskDescroptionPage.showFiles(); });
        var id = jq.getURLParam("id");
        //----Attachments------//
        var filesCount = parseInt(jq('#discussionTabs li[container=discussionFilesContainer]').attr('count'));

        Attachments.bind("addFile", function(ev, file) {
            Teamlab.addPrjEntityFiles(null, id, "message", [file.id], function() { });
            filesCount++;
            updateTabTitle('files', filesCount);
        });

        Attachments.bind("deleteFile", function(ev, fileId) {
            Teamlab.removePrjEntityFiles({}, id, "message", fileId, function() { });
            Attachments.deleteFileFromLayout(fileId);
            filesCount--;
            updateTabTitle('files', filesCount);
        });
        //------end Attachments----//
    });
</script>
<% } %>

<%if(!CanEdit){%>
<script type="text/javascript">
    jq(document).ready(function() {
        Attachments.banOnEditing();
    });
</script>
<% } %>
<script>
    var text = "<%= GetDiscussionTitle() %>";
    document.title = Encoder.htmlDecode(text) + " " + document.title;
    if (text.length > 40) {
        text = text.slice(0, 37) + "...";
    }
    jq(".mainContainerClass >.containerHeaderBlock table td > div:first-child span:last-child").html(text);
</script>

<div id="pageHeader">
    <span class="discussionHeader" title="<%= Discussion.Title.HtmlEncode() %>">
        <%= GetDiscussionTitle() %></span>
</div>
<div style="clear: both">
</div>

<div id="discussionActionsContainer">
    <% if (CanEdit) { %>
    <span id="updateDiscussionButton">
        <a href="<%= GetUpdateDiscussionUrl() %>" class="dottedLink">
        <%= MessageResource.EditMessage %>
        </a>
    </span>
    <span id="deleteDiscussionButton">
        <a class="dottedLink" discussionid="<%= Discussion.ID %>">
        <%= MessageResource.DeleteMessage %>
        </a>
    </span>
    <span id="manageParticipantsButton">
        <a class="dottedLink">
        <%= ProjectsCommonResource.AddParticipants %>
        </a>
    </span>
    <% } %>
    <%if(ProjectSecurity.CanCreateMessage(RequestContext.GetCurrentProject())){ %>
    <span id="newDiscussionButton">
        <a class="dottedLink" href="<%= GetNewDiscussionUrl() %>">
        <%= MessageResource.NewMessage %>
        </a>
    </span>
    <%} %>
    <span id="changeSubscribeButton"><a class="dottedLink" subscribed="<%= IsSubscribed() ? "1": "0" %>">
        <%= IsSubscribed() ? ProjectsCommonResource.UnSubscribeOnNewComment : ProjectsCommonResource.SubscribeOnNewComment %>
    </a></span>
</div>
<div style="clear: both">
</div>

<div id="discussionDescriptionContainer">
    <div class="discussionAuthorAvatar">
        <img src="<%= GetDiscussionAuthorAvatarURL() %>" alt="<%= GetDiscussionAuthorName() %>" />
    </div>
    <div class="discussionBodyContainer">
        <div>
            <a class="discussionAuthor" href="<%= GetDiscussionAuthorUrl() %>">
                <%= GetDiscussionAuthorName() %></a> <span class="discussionCreatedDate">
                    <%= Discussion.CreateOn.ToString(CultureInfo.InvariantCulture) %>
                </span>
        </div>
        <div class="discussionAuthorTitle">
            <%= GetDiscussionAuthorTitle() %>
        </div>
        <div class="discussionText">
            <%= GetDiscussionText() %>
        </div>
    </div>
</div>
<% var filesCount = GetDiscussionFilesCount(); %>
<ul id="discussionTabs">
    <li class="current" container="discussionCommentsContainer">
        <%= GetCommentsTabTitle() %></li>
    <%if(CanReadFiles)
      {%>
    <li container="discussionFilesContainer" count="<%= filesCount %>">
        <%= ProjectsCommonResource.DocsModuleTitle %>
        <% if (filesCount > 0)
           { %>
        (<%= filesCount %>)
        <% } %>
    </li>
    <% } %>
    <li container="discussionParticipantsContainer">
        <%= GetParticipantsTabTitle() %></li>
</ul>
<div id="discussionTabsContent">
    <div id="discussionCommentsContainer">
        <div id="emptyCommentsContainer">
            <asp:PlaceHolder runat="server" ID="emptyCommentsPlaceHolder" />
        </div>
        <div id="commentsContainer">
            <ascw:CommentsList ID="discussionComments" runat="server" BehaviorID="discussionComments">
            </ascw:CommentsList>
        </div>
    </div>
    <div id="discussionFilesContainer">
        <asp:PlaceHolder runat="server" ID="discussionFilesPlaceHolder" />
    </div>
    <div id="discussionParticipantsContainer">
        <asp:Repeater ID="discussionParticipantRepeater" runat="server">
            <ItemTemplate>
                <div class="discussionParticipant">
                    <%# GetDiscussionParticipantLink(((IRecipient)(Container.DataItem)).ID) %>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="discussionParticipant" id="currentLink">
            <%= GetCurrentParticipantLink() %>
        </div>
        <div style="clear: both;">
        </div>
        <% if (CanEdit) {%>
        <asp:PlaceHolder ID="discussionParticipantsSelectorHolder" runat="server"></asp:PlaceHolder>
        <% } %>
    </div>
</div>
<input id="hiddenProductId" type="hidden" value="<%= ProductEntryPoint.ID.ToString() %>" />
<div id="questionWindow" style="display: none">
    <ascw:Container ID="_hintPopup" runat="server">
        <header><%= MessageResource.DeleteMessage %></header>
        <body>
            <p>
                <%= MessageResource.DeleteDiscussionPopup %>
            </p>
            <p>
                <%= ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="popupButtonContainer">
                <a class="baseLinkButton remove">
                    <%= MessageResource.DeleteMessage %></a> <span class="button-splitter"></span>
                <a class="grayLinkButton cancel">
                    <%= ProjectsCommonResource.Cancel %></a>
            </div>
        </body>
    </ascw:Container>
</div>
