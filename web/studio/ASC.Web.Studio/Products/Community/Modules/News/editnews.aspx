<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/News/news.Master"
    AutoEventWireup="true" CodeBehind="editnews.aspx.cs" Inherits="ASC.Web.Community.News.EditNews" %>

<%@ Register Namespace="ASC.Web.Community.News.Controls" Assembly=" ASC.Web.Community.News"
    TagPrefix="ucc" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register TagPrefix="uc" Namespace="ASC.Web.Controls" Assembly="ASC.Web.Controls" %>
<%@ Import Namespace="ASC.Web.Community.News.Resources" %>
<asp:Content ContentPlaceHolderID="HeadContent" runat="Server">

    <uc:JsHtmlDecoder id="JsHtmlDecoder" runat="Server"></uc:JsHtmlDecoder>
    
    <script type="text/javascript">
        var isMobile = <%=_mobileVer?"true":"false" %>;
        
        function FeedPrevShow() {
            jq('#feedPrevDiv_Caption').val(jq('#<%=feedName.ClientID %>').val());
            
            if (isMobile) {
                var text = jq('#<%=SimpleText.ClientID%>').val();
                jq('#feedPrevDiv_Body').html(ASC.Controls.HtmlHelper.Text2EncodedHtml(text));
            }
            else
                jq('#feedPrevDiv_Body').html(FCKeditorAPI.GetInstance('<%=HTML_FCKEditor.ClientID %>').GetXHTML(true));
                
            jq('#feedPrevDiv').show();
            
            jq.scrollTo(jq('#feedPrevDiv').position().top, {speed:500});
        }
        
        function HidePreview()
        {
            jq('#feedPrevDiv').hide();
            jq.scrollTo(jq('#newsCaption').position().top, {speed:500});
        }
        
    </script>
    
    <script type="text/javascript">

    function FCKConfig_OnLoad(config)
    {
        config.RedirectUrlToUpload('<%=RedirectUpload()%>');
        config.MaxImageWidth = 650;
    }

    function PreSaveMobile() {
        if (isMobile) {
        
            var text = jq('#<%=SimpleText.ClientID%>').val();
            jq('#mobiletext').val(ASC.Controls.HtmlHelper.Text2EncodedHtml(text));
        }
    }

    </script>

</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="NewsContents" runat="server">
   <div class="requiredField" style="padding-bottom: 20px;">
    <span class="requiredErrorText"><%=NewsResource.RequaredFieldValidatorCaption%></span>
    <div class="headerPanel" id="newsCaption">
        <%=NewsResource.NewsCaption%></div>
        <asp:TextBox runat="server" ID="feedName" class="textEdit" Style="width: 100%" />
   </div>
    <div class="headerPanel">
        <%=NewsResource.NewsType%></div>
    <div class="headerPanel">
        <div style="padding-bottom: 20px;">
            <asp:DropDownList runat="server" ID="feedType" class="comboBox" DataTextField="TypeName" DataValueField="id" />
        </div>
        <%=NewsResource.NewsBody%></div>
       
    <% if (_mobileVer){%>
    
    <asp:TextBox runat="server" ID="SimpleText" TextMode="MultiLine" style="width:100%; height:200px;"></asp:TextBox>
    <textarea id="mobiletext" name="mobiletext" style="display:none;"><%=_text%></textarea>
        <script type="text/javascript">
            jq(function() {

            var node = jq('<div>' + jq('#mobiletext').val() + '</div>').get(0);
                jq('#<%=SimpleText.ClientID%>').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
            })
            
        </script>
       
    <%}else{ %>
    <FCKeditorV2:FCKeditor runat="server" ID="HTML_FCKEditor" Width="100%" Height="400px" />
    <%} %>
    
    <div class="clearFix" id="panel_buttons" style="padding-top: 15px;" align="left">
        <%--<asp:LinkButton ID="lbSave" OnClientClick="javascript:PreSaveMobile(); NewsBlockButtons();" CssClass="baseLinkButton"
            OnClick="SaveFeed" CausesValidation="true" runat="server" Style="margin-right: 8px;"><%=NewsResource.PostButton%></asp:LinkButton>--%>
        <a href="javascript:void(0);" id="lbSave" class="baseLinkButton" onclick="PreSaveMobile(); NewsBlockButtons(); CheckDataNews();"
         style="margin-right: 8px;"><%=NewsResource.PostButton%></a>
        <a href="javascript:void(0);" onclick="FeedPrevShow(); return false;"
            class="baseLinkButton" style="margin-right: 8px;">
            <%=NewsResource.Preview%></a>
            
            <asp:LinkButton ID="lbCancel" CssClass="grayLinkButton cancelFckEditorChangesButtonMarker"
            OnClick="CancelFeed" CausesValidation="true" OnClientClick="javascript:NewsBlockButtons();" runat="server"><%=NewsResource.CancelButton%></asp:LinkButton>
             
    </div>
    <div style="padding-top: 15px; display: none;" id="action_loader" class="clearFix">        
        <div class="textMediumDescribe">
            <%=NewsResource.PleaseWaitMessage%>
		</div>
		<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
    </div>
    <div id="feedPrevDiv" style="display: none; padding-top: 20px">
        <div class="headerPanel">
            <%=NewsResource.FeedPrevCaption%>
        </div>
        <input id="feedPrevDiv_Caption" class="feedPrevCaption" />
        <div id="feedPrevDiv_Body" class="feedPrevBody longWordsBreak">
        </div><%--
        <div class="feedPrevCredits">
            <%=NewsResource.PostedBy%><span id="feedPrevDiv_PostedBy" style="padding-right: 12px;
                padding-left: 8px;"></span><span id="feedPrevDiv_PostedOn"></span></div>--%>
        <div style='margin-top:10px;'><a class="baseLinkButton" href='javascript:void(0);' onclick='HidePreview(); return false;'><%= NewsResource.HideButton%></a></div>
    </div>
</asp:Content>
