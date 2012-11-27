<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionsList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Messages.DiscussionsList" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<link href="<%= PathProvider.GetFileStaticRelativePath("discussions.css") %>" type="text/css"
    rel="stylesheet" />

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("jquery.tmpl.min.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("discussions.js") %>"></script>

<script type="text/javascript" src="<%= PathProvider.GetFileStaticRelativePath("common_filter_projects.js") %>"></script>

<script type="text/javascript">
    jq(document).ready(function() {
        discussions.init('<%= SecurityContext.CurrentAccount.ID %>');
    }); 
</script>

<% if (RequestContext.IsInConcreteProject())
   { %>

<script type="text/javascript">
    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
	var lastWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
	lastWeek.setDate(lastWeek.getDate() - 7);
    
    function createAdvansedFilter() {
        jq('#AdvansedFilter').advansedFilter(
        {
            store: true,
            anykey: true,
            colcount: 2,
            anykeytimeout: 1000,
            filters:
            [
                { 
                    type: "person",
                    id: "me-author",
                    title: "<%= ProjectsFilterResource.MyDiscussions %>",
                    filtertitle: "<%= ProjectsFilterResource.Author%>:",
                    group: "<%= ProjectsFilterResource.ByParticipant %>",
                    hashmask: "person/{0}",
                    groupby: "userid",
                    bydefault: {id : "<%= SecurityContext.CurrentAccount.ID %>"}
                },
                {
                    type: "person",
                    id: "author",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.Author %>:",
                    group: "<%= ProjectsFilterResource.ByParticipant %>",
                    hashmask: "person/{0}",
                    groupby: "userid"
                },
                { 
                    type: "daterange",
                    id: "today2",
                    title: "<%= ProjectsFilterResource.Today %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created",
                    bydefault: {from : today.getTime(), to : today.getTime()}
                },
                {
                    type: "daterange",
                    id: "recent",
                    title: "<%= ProjectsFilterResource.Recent %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created",
                    bydefault: {from : lastWeek.getTime(), to : today.getTime()}
                },
                {
                    type: "daterange",
                    id: "created",
                    title: "<%= ProjectsFilterResource.CustomPeriod %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created"
                },
                // Other
                { 
                	type: "flag",
                	id: "followed",
                	title: "<%= ProjectsFilterResource.FollowDiscussions %>",
                	group: "<%= ProjectsFilterResource.Other%>",
                	hashmask: "followed"
                }
            ],
            sorters:
            [
                { id: "create_on", title: "<%= ProjectsFilterResource.ByCreateDate %>", sortOrder: "descending", def: true },
                { id: "title", title: "<%= ProjectsFilterResource.ByTitle %>", sortOrder: "ascending" }
                
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

      ASC.Projects.ProjectsAdvansedFilter.init = true;
    }
</script>

<% }
   else
   { %>

<script type="text/javascript">
    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
	var lastWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
	lastWeek.setDate(lastWeek.getDate() - 7);
    
    function createAdvansedFilter(tags, projects) {
        jq('#AdvansedFilter').advansedFilter(
        {
            store: true,
            anykey: true,
            colcount: 2,
            anykeytimeout: 1000,
            filters: 
            [
                { 
                    type: "person",
                    id: "me_author",
                    title: "<%= ProjectsFilterResource.MyDiscussions %>",
                    filtertitle: "<%= ProjectsFilterResource.Author %>:",
                    group: "<%= ProjectsFilterResource.ByParticipant %>",
                    hashmask: "my",
                    groupby: "userid",
                    bydefault: {id : "<%= SecurityContext.CurrentAccount.ID %>"}
                },
                { 
                    type: "person",
                    id: "author",
                    title: "<%= ProjectsFilterResource.OtherUsers %>",
                    filtertitle: "<%= ProjectsFilterResource.Author %>:",
                    group: "<%= ProjectsFilterResource.ByParticipant %>",
                    hashmask: "person/{0}",
                    groupby: "userid"
                },
                {
                    type: "flag",
                    id: "myprojects",
                    title: "<%= ProjectsFilterResource.MyProjects %>",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    hashmask: "myprojects",
                    groupby: "projects"
                },
                {
                    type: "combobox",
                    id: "project",
                    title: "<%= ProjectsFilterResource.OtherProjects %>",
                    filtertitle: "<%= ProjectsFilterResource.ByProject %>:",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    options: projects,
                    groupby: "projects",
                    defaulttitle: "<%= ProjectsFilterResource.Select%>"
                },
                {
                    type: "combobox",
                    id: "tag",
                    title: "<%= ProjectsFilterResource.ByTag %>",
                    filtertitle: "<%=ProjectsFilterResource.Tag %>:",
                    group: "<%= ProjectsFilterResource.ByProject %>",
                    options: tags,
                    groupby: "projects",
                    defaulttitle: "<%= ProjectsFilterResource.Select%>"
                },
                { 
                    type: "daterange",
                    id: "today2",
                    title: "<%= ProjectsFilterResource.Today %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created",
                    bydefault: {from : today.getTime(), to : today.getTime()}
                },
                {
                    type: "daterange",
                    id: "recent",
                    title: "<%= ProjectsFilterResource.Recent %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created",
                    bydefault: {from : lastWeek.getTime(), to : today.getTime()}
                },
                {
                    type: "daterange",
                    id: "created",
                    title: "<%= ProjectsFilterResource.CustomPeriod %>",
                    filtertitle: " ",
                    group: "<%= ProjectsFilterResource.ByCreateDate %>",
                    hashmask: "created/{0}/{1}",
                    groupby: "created"
                },
                // Other
                {
                    type: "flag",
                    id: "followed",
                    title: "<%= ProjectsFilterResource.FollowDiscussions %>",
                    group: "<%= ProjectsFilterResource.Other%>",
                    hashmask: "followed"
                }
            ],
            sorters:
            [
                { id: "create_on", title: "<%= ProjectsFilterResource.ByCreateDate %>", sortOrder: "descending", def: true },
                { id: "title", title: "<%= ProjectsFilterResource.ByTitle %>", sortOrder: "ascending" }
                
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

      ASC.Projects.ProjectsAdvansedFilter.init = true;
    }
</script>

<% } %>    
<% if (CanCreateDiscussion()) { %>
<span id="newDiscussionButton">
    <a class="dottedLink"><%= MessageResource.NewMessage %></a> 
</span>
<% } %>
<asp:PlaceHolder runat="server" ID="contentPlaceHolder"></asp:PlaceHolder>
<div class="presetContainer">
    <a id="preset_my" class="baseLinkAction">
        <%=ProjectsFilterResource.MyDiscussions %></a> <span>,</span> <a id="preset_follow"
            class="baseLinkAction">
            <%=ProjectsFilterResource.DiscussionFollow%></a> <span>,</span> <a id="preset_latest"
                class="baseLinkAction">
                <%=ProjectsFilterResource.LatestDiscussions%></a>
</div>
<div id="discussionsList">
</div>
<div id="EmptyListDiscussion" class="noContent">
</div>
<div id="EmptyListForFilter" class="noContent">
</div>
<asp:PlaceHolder runat="server" ID="emptyScreenHolder"></asp:PlaceHolder>
<span id="showNextDiscussionsButton">
    <%= MessageResource.ShowNextDiscussions %></span>

<script id="discussionTemplate" type="text/x-jquery-tmpl">
    <div class="discussionContainer">
        <div class="discussionAuthorAvatar">
            <img src="${authorAvatar}" alt="${authorName}"/>
        </div>
        <div class="discussionDescriptionContainer">
            <div class="discussionCreatedDate">
                ${createdDate}
            </div>
            <a href=${projectUrl} class="discussionProjectTitle">
                ${projectTitle}       
            </a>
            <div style="clear: right;"></div>
            <div class="discussionAuthor">
                <a>${authorName}</a>
                <span class="discussionAuthorTitle">
                    ${authorPost}
                </span>
                <div style="clear: left;"></div>
            </div>
            <div class="discussionTitle">
                <a href="${discussionUrl}">
                    ${title}
                </a>
            </div>
            <div class="discussionText">
                {{html text}}
            </div>
            <div class="discussionComments">
                 <a href="${commentsUrl}"><%=MessageResource.Comments%>: ${commentsCount}</a>
            </div>
        </div>
    </div>
</script>

