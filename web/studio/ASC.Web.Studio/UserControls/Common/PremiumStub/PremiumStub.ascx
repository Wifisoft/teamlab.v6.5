
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PremiumStub.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.PremiumStub" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>

<div id="studio_premiumStubDialog" style="display:none;">
<ascwc:Container ID="_stubContainer" runat="server">
    <Header>
        <%=Resources.Resource.PremiumStubTitle%>
    </Header>
    <Body>
        <div id="premiumStubContant">           
            
         <div id="premiumStubText" class="headerBase">
                <%=_text%>                         
         </div>
         <div class="description">
            <%=Resources.Resource.PremiumFeatureDescription%>
         </div>
         
         <ul>
            <li>
                <div class="title">
                    <%=Resources.Resource.PrivateProjectsFeature%>
                </div>
                <%=Resources.Resource.PrivateProjectsFeatureDesc%>
            </li>
            <li>
                 <div class="title">
                    <%=Resources.Resource.FileSharingFeature%>
                </div>
                <%=Resources.Resource.FileSharingFeatureDesc%>
            </li>
            <li>
                 <div class="title">
                    <%=Resources.Resource.PrioritySupportFeature%>
                </div>
                <%=Resources.Resource.PrioritySupportFeatureDesc%>
            </li>
             <li>
                 <div class="title">
                    <%=Resources.Resource.AdvancedFilesUploadFeature%>
                </div>
                <%=Resources.Resource.AdvancedFilesUploadFeatureDesc%>
            </li>
             <li>
                 <div class="title">
                    <%=Resources.Resource.VastStorageCapacityFeature%>
                </div>
                <%=Resources.Resource.VastStorageCapacityFeatureDesc%>
            </li>
            
         </ul>
        
           
        <div id="premiumStubBtn" class="clearFix">
            <a target="_blank" id="buyStubBtn" href="<%=ShoppingCardUrl()%>" class="premiumButton">
                <div class="left">
                    <div class="content"><%=Resources.Resource.PremiumStubButton%></div>
                </div>
                <div class="right">&nbsp;</div>
            </a>
            
            <a class="learnMore" target="_blank" href="<%=GoPremiumUrl()%>">
                <%=Resources.Resource.LearnMoreLink%>
            </a>
         </div>
               
           
        </div>
        
        <input id="premiumStubType" type="hidden" value="0" />
        <input id="premiumStubText_0" type="hidden" value="<%=HttpUtility.HtmlEncode(_projText)%>"/>
        <input id="premiumStubText_1" type="hidden" value="<%=HttpUtility.HtmlEncode(_accessRightsText)%>"/>
        <input id="premiumStubText_2" type="hidden" value="<%=HttpUtility.HtmlEncode(_sharedText)%>"/>
                       
    </Body>
</ascwc:Container>
</div>