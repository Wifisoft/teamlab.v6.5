<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSubscriptions.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserSubscriptions" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>


<script id="subscribtionObjectsTemplate" type="text/x-jquery-tmpl">
  {{if Objects.length > 0}}
    <div style="padding-top:15px;" id="studio_subscriptions_${ItemId}_${SubItemId}_${TypeId}">
        {{each(i, obj) Objects}} 
		    <div id="studio_subscribeItem_${ItemId}_${SubItemId}_${TypeId}_${obj.Id}" class="clearFix" style="margin-bottom:5px;">
		        <div style="float:left;">
		            <input id="studio_subscribeItemChecker_${ItemId}_${SubItemId}_${TypeId}_${obj.Id}" value="${obj.Id}" type="checkbox"/>
		        </div>
		        <div style="float:left; margin-left:15px; width:70%; overflow: hidden;">
                        {{if obj.Url  == ''}}
                            <a href="${obj.Url}" title="${obj.Name}">${obj.Name}</a>
                        {{else}}
                            ${obj.Name}
                        {{/if}}

	            </div>
		    </div>
	    {{/each}}

	    <div class="clearFix" style="margin-top:15px;">
	        <a class="baseLinkButton promoAction" href="javascript:CommonSubscriptionManager.UnsubscribeObjects('${ItemId}','${SubItemId}','${TypeId}');" style="float:left;"><%=Resources.Resource.UnsubscribeButton%></a>
	    </div>
	</div>
 {{/if}}
</script>


<script id="itemSubscriptionsTemplate" type="text/x-jquery-tmpl">

    {{each(i, item) Items}} 
        <div id="subscriptionProductItem_${item.Id}">
        <div id="studio_product_subscribeBox_${item.Id}" class="borderBase tintMedium clearFix" style="border-left:none; border-right:none; margin-top:-1px; padding:10px;">
                
           <div class="headerBase" style="float:left; cursor:pointer;" onclick="CommonSubscriptionManager.ToggleProductList('${item.Id}')">
				{{if item.LogoUrl != ''}}
					<img alt="" style="margin-right:5px;" align="absmiddle" src="${item.LogoUrl}"/>					
				{{/if}}
				
                ${item.Name}
                
                {{if !item.IsEmpty}}                
                    {{if item.IsOpen}}
                        <img alt="" align="absmiddle" id="studio_subscribeProductState_${item.Id}" style="margin-left:15px;"  src="<%=WebImageSupplier.GetAbsoluteWebPath("collapse_down_dark.png")%>"/>
                    {{else}}                        
                        <img alt="" align="absmiddle" id="studio_subscribeProductState_${item.Id}" style="margin-left:15px;"  src="<%=WebImageSupplier.GetAbsoluteWebPath("collapse_right_dark.png")%>"/>
                    {{/if}}
                {{/if}}
		   </div>

			<div style="float:right; text-align:right; width:110px;">
                {{if item.CanUnSubscribe}}
                    <a class="promoAction unsubscribe baseLinkAction" href="javascript:CommonSubscriptionManager.UnsubscribeProduct('${item.Id}');"><%=Resources.Resource.UnsubscribeButton%></a>                
                {{/if}}
                &nbsp;
            </div>
				
            <div style="float: right;">
		        <select id="NotifyByCombobox_${item.Id}" class="comboBox notify-by-combobox" style="display: none;" onchange="CommonSubscriptionManager.SetNotifyByMethod('${item.Id}', jq(this).val());">
			        {{if item.NotifyType == 0}}
			            <option class="optionItem" value="0" selected="selected">
			        {{else}}
			            <option class="optionItem" value="0">
			        {{/if}}
			        <%=Resources.Resource.NotifyByEmail%></option>
			        
			        {{if item.NotifyType == 1}}
			            <option class="optionItem" value="1" selected="selected">
			        {{else}}
			            <option class="optionItem" value="1">
			        {{/if}}
			        <%=Resources.Resource.NotifyByTMTalk%></option>
			        
			         {{if item.NotifyType == 2}}
			            <option class="optionItem" value="2" selected="selected">
			        {{else}}
			            <option class="optionItem" value="2">
			        {{/if}}
			        <%=Resources.Resource.NotifyByEmailAndTMTalk%></option>
		        </select>
			</div>
			
			</div>
			
			{{if item.IsOpen}}
                <div id="studio_product_subscriptions_${item.Id}" style="padding-left:40px;">
            {{else}}
                <div id="studio_product_subscriptions_${item.Id}" style="padding-left:40px; display:none;">
            {{/if}}          
                {{if item.Type == 0}}   
                    <div id="studio_module_subscriptions_${item.Id}_${item.Id}">	
                    {{each(k, type) item.Types}} 		                                            
                       
                        <div id="studio_subscribeType_${item.Id}_${item.Id}_${type.Id}" class="borderBase" style="border-top:none; border-left:none; border-right:none; padding:10px;">
                                 <div class="clearFix">
					                    {{if !type.Single}}					            
                                            <div style="float:left; cursor:pointer;" onclick="CommonSubscriptionManager.ToggleSubscriptionList('${item.Id}','${item.Id}','${type.Id}');">
						                        ${type.Name}
                                                <img alt="" align="absmiddle" id="studio_subscriptionsState_${item.Id}_${item.Id}_${type.Id}" style="margin-left:15px;"  src="<%=WebImageSupplier.GetAbsoluteWebPath("collapse_right_light.png")%>"/>
						                    </div>				        
    					        
					                    {{else}}
    					                					        
						                    <div style="float:left;">
						                        ${type.Name}
						                    </div>
    						                
						                    <div style="float:right; text-align:right; width:110px;">
						                    {{if type.IsSubscribed}}
                                                <a class="promoAction baseLinkAction" href="javascript:CommonSubscriptionManager.UnsubscribeType('${item.Id}','${item.Id}','${type.Id}');">
                                                    <%=Resources.Resource.UnsubscribeButton%>
                                                </a>                                            
                                             {{else}}
                                                <a class="promoAction baseLinkAction" href="javascript:CommonSubscriptionManager.SubscribeType('${item.Id}','${item.Id}','${type.Id}');">
                                                    <%=Resources.Resource.SubscribeButton%>
                                                </a>
                                             {{/if}}                                            
                                             </div>
						                {{/if}} 
					                </div>
                        </div>
                       {{/each}} 
                       </div>
                 
                
                {{else}}                
                   {{each(j, group) item.Groups}} 			    
                      <div id="studio_module_subscribeBox_${item.Id}_${group.Id}" class="borderBase clearFix" style="border-left:none; border-right:none; margin-top:-1px; padding:10px;">
                        <div class="headerBaseMedium" style="float:left; cursor:pointer;" onclick="CommonSubscriptionManager.ToggleModuleList('${item.Id}','${group.Id}');">

				            {{if group.ImageUrl!=''}}
					            <img alt="" style="margin-right:5px;" align="absmiddle" src="${group.ImageUrl}"/>
					        {{/if}}
				
				            ${group.Name}
                            <img alt="" align="absmiddle" id="studio_subscribeModuleState_${item.Id}_${group.Id}" style="margin-left:15px;"  src="<%=WebImageSupplier.GetAbsoluteWebPath("collapse_down_dark.png")%>"/>
				        </div>
				      </div>

                      <div id="studio_module_subscriptions_${item.Id}_${group.Id}">				
				        {{each(k, type) group.Types}} 			 
				            
				            <div id="studio_subscribeType_${item.Id}_${group.Id}_${type.Id}" class="tintMedium borderBase" style="border-top:none; border-left:none; border-right:none; padding:10px 10px 10px 30px;">
					            <div class="clearFix">
					                {{if !type.Single}}
					            
                                        <div style="float:left; cursor:pointer;" onclick="CommonSubscriptionManager.ToggleSubscriptionList('${item.Id}','${group.Id}','${type.Id}');">
						                    ${type.Name}
                                            <img alt="" align="absmiddle" id="studio_subscriptionsState_${item.Id}_${group.Id}_${type.Id}" style="margin-left:15px;"  src="<%=WebImageSupplier.GetAbsoluteWebPath("collapse_right_light.png")%>"/>
						                </div>
					        
					                {{else}}
					        
						                <div style="float:left;">
						                    ${type.Name}
						                </div>
						             {{/if}} 
						             
						             <div style="float:right; text-align:right; width:110px;">
						                {{if type.IsSubscribed}}
                                            <a class="promoAction baseLinkAction" href="javascript:CommonSubscriptionManager.UnsubscribeType('${item.Id}','${group.Id}','${type.Id}');">
                                                <%=Resources.Resource.UnsubscribeButton%>
                                            </a>                                            
                                         {{else}}
                                            <a class="promoAction baseLinkAction" href="javascript:CommonSubscriptionManager.SubscribeType('${item.Id}','${group.Id}','${type.Id}');">
                                                <%=Resources.Resource.SubscribeButton%>
                                            </a>
                                         {{/if}}                                            
                                     </div>
						            
					             </div>
					          </div>
					          
				          {{/each}}   
                       </div>
                      
			      {{/each}}                
                {{/if}}	          
			</div>	
	    </div>   
    {{/each}}
				
</script>

<script type="text/javascript">
    jq(function() {

        var header = jq('.mainContainerClass>.containerHeaderBlock table td div').last();
        header.css('float', 'left');
        header.after('<div class="HelpCenterSwitcher title big" id="QuestionSubscriptionsPortal" title="' + jq("#questionForHelp").text() + '"></div> ');
        jq('#QuestionSubscriptionsPortal').click(function() {
            jq(this).helper({ BlockHelperID: 'AnswerForHelpSubscriptionsPortal' });
        });
    });

</script>
<span id="questionForHelp" style="display:none;"><%=Resources.Resource.HelpQuestionSubscriptionsPortal%></span>

<a name="subscriptions"></a>


<div style="float: right; margin-top: -21px; margin-right: 125px;">
	<%=Resources.Resource.NotifyBy%>
</div>

<div class="borderBase clearFix" style="border-left:none; border-right:none; padding:10px;">
   <div class="headerBase" style="float:left;">
       <img alt="" style="margin-right:5px;" align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("lastadded_widget.png")%>"/>
       <%=Resources.Resource.WhatsNewSubscriptionName%>                
   </div>
    <div id="studio_newSubscriptionButton" style="float:right; text-align:right; padding-top:5px; width:110px">
        <%=RenderWhatsNewSubscriptionState()%>
    </div>
    
    <%--What's new Notify by Combobox--%>
    <div style="float: right;">
		<%=RenderWhatsNewNotifyByCombobox() %>
    </div>
</div>
            
<% if (IsAdmin())
   {%>
    <div class="borderBase clearFix" style="border-top:none; border-left:none; border-right:none; padding:10px;">
       <%--name & log for admin notify settings--%>
       <div class="headerBase" style="float:left;">
           <img alt="" style="margin-right:5px;" align="absmiddle" src="<%=WebImageSupplier.GetAbsoluteWebPath("btn_settings.png")%>"/>
           <%=Resources.Resource.AdministratorNotifySenderTypeName%>                
       </div>
        <%--unsubscribe link--%>    
        <div id="studio_adminSubscriptionButton" style="float:right; text-align:right; padding-top:5px; width:110px">
            <%=RenderAdminNotifySubscriptionState()%>
        </div>
        
        <%--Admin Notify Notify By Combobox--%>
        <div style="float: right;">
		    <%=RenderAdminNotifyNotifyByCombobox()%>
        </div>
    </div>
<% }%>

<div id="studio_notifySenders" class="clearFix">    
</div>
<%--popup window--%>

 <div class="popup_helper" id="AnswerForHelpSubscriptionsPortal">
     <p><%=String.Format(Resources.Resource.HelpAnswerSubscriptionsPortal, "<br />", "<b>", "</b>")%>
     <a href="http://www.teamlab.com/help/tipstricks/managing-subscriptions.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a></p>
</div>   