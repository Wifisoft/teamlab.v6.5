<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Masters/StudioTemplate.master"
    CodeBehind="Dashboard.aspx.cs" Inherits="ASC.Web.Studio.Dashboard" %>

<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Core.Users" %>
<asp:Content ID="DefaultPageContent" ContentPlaceHolderID="StudioPageContent" runat="server">

    <script>

        /*THIS IS JUST EXAMPLE*/
        jq(function() {
            var undefined = 0,
            created = 1,
            updated = 2,
            deleted = 4,
            commented = 8,
            periodic = 16,
            reply = 32,
            liked = 64,
            closed = 128,
            opened = 256,
            shared = 512;

            function getActions(actionFlag) {
                var actions = [];
                if ((actionFlag & created) === created)
                    actions.push('created');
                if ((actionFlag & updated) === updated)
                    actions.push('updated');
                if ((actionFlag & deleted) === deleted)
                    actions.push('deleted');
                if ((actionFlag & commented) === commented)
                    actions.push('commented');
                if ((actionFlag & periodic) === periodic)
                    actions.push('periodic');
                if ((actionFlag & reply) === reply)
                    actions.push('reply');
                if ((actionFlag & liked) === liked)
                    actions.push('liked');
                if ((actionFlag & closed) === closed)
                    actions.push('closed');
                if ((actionFlag & opened) === opened)
                    actions.push('opened');
                if ((actionFlag & shared) === shared)
                    actions.push('shared');
                return actions;
            }

            jq.getJSON('/api/1.0/feed.json?days=100', function(data) {
                if (data.response) {
                    var container = jq('#container');
                    for (var source in data.response) {
                        if (source) {
                            //Iterate over source
                            var itemsTypes = data.response[source];
                            for (var itemType in itemsTypes) {
                                var items = itemsTypes[itemType];
                                for (var itemIndex = 0; itemIndex < items.length; itemIndex++) {
                                    //Get template by key
                                    var item = items[itemIndex];
                                    var tmplKey = source + '-' + itemType;
                                    var tmplData = { item: item.item, when: item.when, isNew: item.isNew, createdBy: item.createdBy, actions: getActions(item.action), priority: item.priority, related: item.relativeTo };
                                    try {
                                        jq("#feed-" + tmplKey)
                                        .tmpl(tmplData)
                                        .prependTo(container);
                                    } catch(e) {

                                    } 
                                }
                            }
                        }
                    }
                }
            });
        });

    </script>

    <div id="container" class="feed">
    </div>

    <script id="feed-blogs-post" type="text/x-jquery-tmpl">
        <div class="blog post">
            <strong>blog post ${actions[0]}: <a href="/products/community/modules/blogs/viewblog.aspx?blogid=${item.id}">${item.title}</a></strong>
            <p>{{html item.preview}}</p>
            <small>Created: ${when} by ${createdBy.displayName}</small>
        </div>
        <hr/>
    </script>

    <script id="feed-blogs-comment" type="text/x-jquery-tmpl">
        <div class="blog comment">
            <strong><a href="/products/community/modules/blogs/viewblog.aspx?blogid=${related}">blog</a> comment ${actions[0]}</strong>
            
            <p>{{html item.text}}</p>
            <small>Created: ${when} by ${createdBy.displayName}</small>
        </div>
        <hr/>
    </script>

</asp:Content>
