<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkinSettingsContent.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.SkinSettingsContent" %>
   <div class="clearFix">
        <div style="float:left;">
            <asp:Repeater runat="server" ID="skinRepeater">            
                <ItemTemplate>
                <div class="clearFix" style="padding: 3px 0px;">
                
                    <label for="skin_<%#(String)Eval("Id")%>" class="sampleSkinBox borderBase" style="background-image:url('<%#ASC.Data.Storage.WebPath.GetPath("/usercontrols/common/topnavigationpanel/css/"+(String)Eval("Folder")+"/img/topheader_bg.gif")%>');">&nbsp;</label>
                    <div style="float: left">
                        <input class="studio_skin" name="studio_skin" type="radio" <%#(bool)Eval("Checked")?"checked=\"checked\"":""%> 
                            id="skin_<%#(String)Eval("Id")%>" value="<%#(String)Eval("Id")%>" />
                    </div>
                    <div style="float: left; margin-top: 3px; margin-left: 10px;">
                         <label for="skin_<%#(String)Eval("Id")%>" style="cursor:pointer;">
                            <%#HttpUtility.HtmlEncode((String)Eval("Name"))%></label>
                            
                         <img id="skin_prev_<%#(String)Eval("Id")%>" src="<%#(String)Eval("Path")%>"  style="display:none;"/>
                    </div>
                </div>
                </ItemTemplate>
             </asp:Repeater>
             
            
        </div>
        
        <img id="studio_skinPreview" alt="<%=Resources.Resource.SkinSettings%>" src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("skins/"+_currentSkin.ID+".png")%>" />

    </div>