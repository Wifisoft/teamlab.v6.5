<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Assembly Name="ASC.Blogs.Core" %>
<%@ Import Namespace="ASC.Blogs.Core.Resources" %>
<%@ Import Namespace="ASC.Web.Community.Blogs" %>


<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true"
    CodeBehind="EditBlog.aspx.cs" Inherits="ASC.Web.Community.Blogs.EditBlog" Title="Untitled Page" %>

<%@ Register Src="Controls/TagCloud.ascx" TagPrefix="ctrl" TagName="TagCloud" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagPrefix="ctrl" TagName="ActionContainer" %>
<%@ Register Src="Controls/TopList.ascx" TagPrefix="ctrl" TagName="TopList" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">

    <ascw:JsHtmlDecoder id="JsHtmlDecoder" runat="Server"></ascw:JsHtmlDecoder>
 
   <%=RenderScripts()%>
    <script type="text/javascript">
    function callback(result)
    {
        if(result.value != null)
        {
            document.getElementById("tag_" + result.value).innerHTML = "";
        }
    }    
    function FCKConfig_OnLoad(config)
    {
        config.RedirectUrlToUpload("<%=RenderRedirectUpload()%>");  
        config.MaxImageWidth = <%=GetBlogMaxImageWidth %>;
    }
    
    BlogsManager.IsMobile = <%=_mobileVer?"true":"false" %>;
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <ascw:container id="mainContainer" runat="server">
        <header>
        </header>
        <body>
            <asp:HiddenField ID="hidBlogID" runat="server" />
            <asp:HiddenField ID="hdnUserID" runat="server" />
            <a name="post"></a>
            <div id="postHeader" class="requiredField" style="padding-bottom: 20px;">
                <span class="requiredErrorText"><%=BlogsResource.BlogTitleEmptyMessage %></span>
                <asp:Panel ID="pnlHeader" runat="server">
                    <div class="headerPanel">
                        <%=BlogsResource.BlogTitleLabel%>                   
                    </div>
                    <asp:TextBox ID="txtTitle" MaxLength="255" CssClass="textEdit" runat="server" Width="100%"></asp:TextBox>
                </asp:Panel>
            </div>
            <div class="headerPanel">
                <%=BlogsResource.ContentTitle %>
            </div>
            <div style="padding-bottom: 20px;">
                <% if (_mobileVer){%>    
                <textarea ID="mobiletextEdit" style="width:100%; height:200px;"></textarea>
                <textarea id="mobiletext" name="mobiletext" style="display:none;"><%=_text%></textarea>
                    <script type="text/javascript">
                        jq(function() {

                            var node = jq('<div>' + jq('#mobiletext').val() + '</div>').get(0);
                            jq('#mobiletextEdit').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
                        })
                        
                    </script>
                   
                <%}else{ %>
                    <FCKeditorV2:FCKeditor ID="FCKeditor" Height="400px" runat="server">
                    </FCKeditorV2:FCKeditor>
                <%} %>
                
            </div>
            <div class="headerPanel">
                <%=BlogsResource.TagsTitle%></div>
            <asp:TextBox CssClass="textEdit" ID="txtTags" runat="server" Width="100%"
				autocomplete="off" onkeydown="return blogTagsAutocompleteInputOnKeyDown(event);"></asp:TextBox><br>
            <div class="textMediumDescribe" style="text-align: left;">
                <%=BlogsResource.EnterTagsMessage%></div>
            <div id="panel_buttons" style="padding-top: 15px;">
                    
                    <a style="width: 80px" class="baseLinkButton" href="javascript:void(0);"
						onclick="BlogsManager.PerformMobilePost(); BlogsManager.BlockButtons(); BlogsManager.CheckData();"><%=BlogsResource.PostButton%></a>
                    <a style="margin-left: 8px; width: 80px" class="baseLinkButton" href="javascript:void(0);"
						onclick="BlogsManager.ShowPreview('<%=FCKeditor.ClientID%>', '<%=txtTitle.ClientID%>'); return false;"><%=BlogsResource.PreviewButton%></a>
                    
                    <asp:LinkButton ID="lbCancel" OnClientClick="javascript:BlogsManager.BlockButtons();"
                        CssClass="grayLinkButton cancelFckEditorChangesButtonMarker" style="margin-left: 8px;" runat="server"
                        OnClick="lbCancel_Click"><%=BlogsResource.CancelButton %></asp:LinkButton>                                        
                    
            </div>
            <div style="padding-top: 15px; display: none;" id="action_loader" class="clearFix">
                <div class="textMediumDescribe">
                    <%=BlogsResource.PleaseWaitMessage%>
                </div>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">
            </div>
            <div id="previewHolder" style="display: none;">
            <asp:PlaceHolder ID="PlaceHolderPreview" runat="server"></asp:PlaceHolder>
            </div>
        </body>
    </ascw:container>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
    <ctrl:ActionContainer ID="actions" runat="server" />    
    <%--<ctrl:toplist ID="ctrlTopList" runat="server" />--%>
    <ascwc:SideRecentActivity id="sideRecentActivity" runat="server"></ascwc:SideRecentActivity>
    <ctrl:TagCloud ID="TagCloud" runat="server" />
</asp:Content>
