<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharingSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.SharingSettings" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls.Users" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<script id="sharingListTemplate" type="text/x-jquery-tmpl">
    
    {{each(i, item) items}}     
                    
            <div id="sharing_item_${item.id}" class="sharingItem borderBase clearFix {{if i%2 == 0}}tintMedium{{/if}}">        
            
            {{if item.isGroup}}
                <div class="name" title="${item.name}">
                    ${item.name}
                </div>
            {{else}}
                <div class="name">
                    <span class="userLink" title="${item.name}">${item.name}</span>   
                </div>             
            {{/if}}
            
            <div class="remove">
                {{if item.canEdit}}
                    <img class="removeItem" data="${item.id}" border="0" align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("trash.png")%>" alt="<%=Resources.Resource.DeleteButton%>"/>
                {{else}}
                    &nbsp;
                {{/if}}
            </div>
            
            <div class="action">
                {{if item.canEdit}}
                    <select data="${item.id}" id="select_${item.id}">
                        {{each(j, action) actions}}  
                            {{if action.id == item.selectedAction.id}}
                                <option value="${action.id}" selected="selected">${action.name}</option>
                             {{else}}
                                <option value="${action.id}">${action.name}</option>
                             {{/if}}                                
                        {{/each}}     
                    </select>
                {{else}}
                    ${item.selectedAction.name}
                {{/if}}
            </div>
            
        </div>
    {{/each}}
    
</script>



<div id="studio_sharingSettingsDialog" style="display:none;">
    <ascwc:Container ID="_sharingDialogContainer" runat="server">
        <Header>
            <%=Resources.Resource.SharingSettingsTitle%>
        </Header>
        <Body>
            <div class="header headerBase borderBase">
                <%=bodyCaption%>
            </div>

            <div id="sharingSettingsItems"></div>

            <div class="addToSharingLinks borderBase clearFix">                

                <ascwc:AdvancedUserSelector runat="server" id="shareUserSelector"></ascwc:AdvancedUserSelector>
                
                <asp:PlaceHolder ID="_groupSelectorHolder" runat="server"></asp:PlaceHolder>
                
            </div>
            <div class="btnBox clearFix">
                <a id="sharingSettingsSaveButton" class="baseLinkButton"><%=Resources.Resource.SaveButton %></a>
                <a id="sharingSettingsCancelButton" class="grayLinkButton"><%=Resources.Resource.CancelButton %></a>
            </div>
        </Body>
    </ascwc:Container>
</div>