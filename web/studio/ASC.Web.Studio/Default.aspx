<%@ Page Language="C#" MasterPageFile="~/Masters/StudioTemplate.master" AutoEventWireup="true" EnableViewState="false"
    CodeBehind="Default.aspx.cs" Inherits="ASC.Web.Studio._Default" Title="Untitled Page" %> 
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>   
<%@ Import Namespace="ASC.Web.Studio.Utility" %> 
<%@ Import Namespace="ASC.Web.Core" %> 
<%@ Import Namespace="ASC.Core" %> 
<%@ Import Namespace="ASC.Core.Users" %> 
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
 

 <asp:Content runat="server" ContentPlaceHolderID="StudioTitleContent">
     <asp:PlaceHolder ID="fullSearch" runat="server">
         <div class="fullSearchHolder">
             <div class="clearFix searchBox">
                 <div style="float:left;" class="dv">
                     <input type="text" id="studio_search" class="textEditMainSearch" maxlength="255" />
                 </div>
                 <div class="mainSearchButton" onclick="Searcher.Search();">
                     <input type="hidden" value="" />
                 </div>
             </div>
         </div>
     </asp:PlaceHolder>
 </asp:Content>
<asp:Content ID="DefaultPageContent" ContentPlaceHolderID="StudioPageContent" runat="server"> 

  <div id="GreetingBlock">
    
    <div style="padding-top:10px;">
    <asp:PlaceHolder ID="navPanelHolder" runat="server">
         
    </asp:PlaceHolder>
    </div>
    <div class="clearFix" style="padding-top:20px;">
    
    <%if(_showGettingStarted){ %>
    <div style="float:left; display:none;"> <%--left block is hidden--%>
        <div class="videoBox borderBase tintLight">         
            <div class="borderBase headerBase gsHeader">
                <%=Resources.Resource.GettingStartingHeader%>
            </div>
            
            <div class="getstartMenuItem">            
             <a class="linkHeaderGetstart" href="#" onclick="javascript:ImportUsersManager.ShowImportControl(); return false;"><img alt="" align="absmiddle" src=<%=WebImageSupplier.GetAbsoluteWebPath("btn_invitepeople.png") %> /></a>
             <a class="linkHeaderGetstart" href="#" onclick="javascript:ImportUsersManager.ShowImportControl(); return false;"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddEmployeesButton").HtmlEncode()%></a>             
            </div><br clear="all" />
            
            <%if(IsProjectsEnabled) { %>
            <div class="getstartMenuItem">
             <a class="linkHeaderGetstart" href="<%=VirtualPathUtility.ToAbsolute("~/products/projects/settings.aspx")%>"><img alt="" align="absmiddle" src=<%=WebImageSupplier.GetAbsoluteWebPath("btn_importbasecamp.png") %> /></a>
             <a class="linkHeaderGetstart" href="<%=VirtualPathUtility.ToAbsolute("~/products/projects/settings.aspx")%>"><%=Resources.Resource.ImportFromBaseCamp%></a>
            </div><br clear="all" />
            <%} %>
            
            <div class="getstartMenuItem">
             <a class="linkHeaderGetstart" href="#" onclick="StudioManager.ShowGettingStartedVideo(); return false;"><img alt="" align="absmiddle" src=<%=WebImageSupplier.GetAbsoluteWebPath("btn_video.png") %> /></a>
             <a class="linkHeaderGetstart" href="#" onclick="StudioManager.ShowGettingStartedVideo(); return false;"><%=Resources.Resource.WatchVideoButton%></a>
            </div><br clear="all" />
            
            <div style="margin:23px 0 10px;" class="clearFix">
                <input id="studio_gettingStartedState" onclick="StudioManager.SaveGettingStartedState();" type="checkbox" style="height: 15px; margin-top: 0px; margin-bottom: 0px; float: left;"/>
                <label for="studio_gettingStartedState" style="float: left; margin-left: 3px;"><%=Resources.Resource.DontShowOnStartupText%></label>
            </div>
            
           
        </div>
        
        <%--video dialog--%>
        <div id="studio_GettingStartedVideoDialog" style="display:none;">
             <ascwc:Container runat="server" ID="_gettingStartedVideoContainer">
                <Header>
                    <%=Resources.Resource.GettingStartingHeader%>
                </Header>
                <Body>
                 <div>
                    <object width="640" height="385"><param name="movie" value="https://www.youtube.com/v/CU5qm_xiNck&hl=en_US&fs=1&rel=0"></param><param name="allowFullScreen" value="true"></param><param name="allowscriptaccess" value="always"></param><embed src="https://www.youtube.com/v/CU5qm_xiNck&hl=en_US&fs=1&rel=0" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" width="640" height="385"></embed></object>
                 </div>
                 <div class="clearFix" style="margin-top:16px;">
                 <a class="grayLinkButton" style="float:left;" href="javascript:jq.unblockUI()"><%=Resources.Resource.CancelButton %></a>
                 </div>
                </Body>
             </ascwc:Container>
        </div>
        
        <div id="fb-root"></div><script src="https://connect.facebook.net/en_US/all.js#xfbml=1"></script><fb:like href="http://www.facebook.com/TeamLab" send="false" width="240" show_faces="false" font=""></fb:like>

    </div>
    <%} %>

    <div class="GreatingModulesBlock borderBase"> <%--style="float:left;  <%if(_showGettingStarted){ %>width:670px;<%} %>">--%>

        <div class="headerBaseBig" style="padding:15px 15px 5px;">
        <%=String.Format(Resources.Resource.WelcomeUserMessage, CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).DisplayUserName(true))%>
        </div>
        
        <asp:Repeater runat="server" ID="_productRepeater">
            <HeaderTemplate>
                <div class="clearFix">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="product clearFix">
                    <img alt="" src="<%#(Container.DataItem as IWebItem).GetLargeIconAbsoluteURL()%>" />
                    <h2 class="title">
                        <a class="linkHeaderLightBig" href="<%#VirtualPathUtility.ToAbsolute((string)Eval("StartURL"))%>"><%#HttpUtility.HtmlEncode((string)Eval("Name"))%></a>
                    </h2>
                    <span class="description"><%#(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin() && (Container.DataItem is Product))? (string)Eval("ExtendedDescription") : (string)Eval("Description")%></span>
                </div>
                <%#Container.ItemIndex%2!=0?"</div><div class='clearFix'>":""%>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
        <% if (!ASC.Core.CoreContext.Configuration.Standalone)
           {%>
      <div class="SocialLinks" >
           <ul class="ListSocLink">
             <li><a class="faceBook" onmouseup="PageTrack('GoTo_facebook');" href="http://www.facebook.com/pages/TeamLabcom/118996991455138" target="_blank">&nbsp;</a></li>
             <li> <a class="youtube" href="https://www.youtube.com/user/TeamLabdotcom" onmouseup="PageTrack('GoTo_youtube');" target="_blank" rel="nofollow">&nbsp;</a></li>
             <li>  <a class="linkedin" href="https://www.linkedin.com/groups?home=&gid=3159387&trk=anet_ug_hm" onmouseup="PageTrack('GoTo_linkedin');" target="_blank" rel="nofollow">&nbsp;</a></li>
             <li style="width:110px; margin-top:2px;"><a href="https://twitter.com/share" class="twitter-share-button" data-url="http://www.teamlab.com/" data-via="TeamLabdotcom" data-related="TeamLabdotcom">Tweet</a></li>
             <li class="GooglePlus"> <span><g:plusone href="http://www.teamlab.com/" size="medium"></g:plusone></span></li>
             <li><iframe src="https://www.facebook.com/plugins/like.php?href=http%3A%2F%2Fwww.facebook.com%2FTeamLab&amp;send=false&amp;local=en_US&amp;layout=standart&amp;width=330&amp;show_faces=true&amp;action=like&amp;colorscheme=light&amp;font&amp;height=24" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:500px; height:28px;" allowTransparency="true"></iframe></li>
           </ul>
           <script>!function(d, s, id) { var js, fjs = d.getElementsByTagName(s)[0]; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = "//platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); } } (document, "script", "twitter-wjs");</script>
           <script type="text/javascript">

               (function() {
                   var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
                   po.src = 'https://apis.google.com/js/plusone.js';
                   var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
               })();
           </script>
       </div> 
   <%} %>
         
    </div>
    </div>    
 </div>
 

 <asp:PlaceHolder runat="server" ID="_afterRegistryWelcomePopupBoxHolder">
 <script language="javascript" type="text/javascript">
    jq(document).ready(function(){
       try {

			jq.blockUI({ message: jq("#studio_welcomeMessageBox"),
				css: {
					opacity: '1',
					border: 'none',
					padding: '0px',
					width: '400px',
					cursor: 'default',
					textAlign: 'left',
					'background-color': 'Transparent',
					'margin-left': '-200px',
					'top': '30%'
				},

				overlayCSS: {
					backgroundColor: '#aaaaaa',
					cursor: 'default',
					opacity: '0.3'
				},
				focusInput: false,
				fadeIn: 0,
				fadeOut: 0
			});
		}
		catch (e) { };
         
    });
 </script>
 
 
 <div id="studio_welcomeMessageBox" style="display:none;">
     <ascwc:Container runat="server" ID="_welcomeBoxContainer">
        <Header>
            <%=Resources.Resource.AfterRegistryMessagePopupTitle%>
        </Header>
        <Body>
         <div>
            <%=Resources.Resource.AfterRegistryMessagePopupText%>
         </div>
         <div class="clearFix" style="margin-top:16px;">
         <a class="baseLinkButton" style="float:left;" href="javascript:jq.unblockUI()"><%=Resources.Resource.ContinueButton %></a>
         </div>
        </Body>
     </ascwc:Container>
</div>
</asp:PlaceHolder>

</asp:Content>
