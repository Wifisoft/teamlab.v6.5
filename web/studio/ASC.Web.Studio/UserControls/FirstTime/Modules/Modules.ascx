<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Modules.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.Modules" %>
<div><%= Resources.Resource.WizardModulesTitle %></div>
<div id="modules" class="clearFix">
    <asp:Repeater runat="server" ID="_itemsRepeater">
        <ItemTemplate>
            <%# ((Container.ItemIndex)%2==0?"<div class=\"clearFix\">":"")%>
            <div class="item headerBaseMedium">
                <div id="itm" class="clearFix" name="<%#Eval("ID")%>">
                    <div style="float: left">
                        <input style="margin-right: 15px;" id="studio_poptDisabled_<%#Eval("ID")%>" value="<%#((bool)Eval("Disabled"))?"1" : "0"%>"
                            <%#((bool)Eval("Disabled"))?"" : "checked=checked"%> type="checkbox" onchange="WebItemsSettingsManager.ClickOnProduct('<%#Eval("ID")%>');" />
                    </div>
                    <div class="logo">
                        <img src="<%# Eval("LogoPath").ToString() %>" style="display:<%# String.IsNullOrEmpty(Eval("LogoPath").ToString())?"none":Eval("LogoPath").ToString() %>" />
                    </div>
                    <div class="named">
                        <label for="studio_poptDisabled_<%#Eval("ID")%>">
                            <%#HttpUtility.HtmlEncode((string)Eval("Name"))%></label>
                        <div class="description">
                            <%#HttpUtility.HtmlEncode((string)Eval("Desciption"))%></div>
                    </div>
                </div>
            </div>
            <%# ((Container.ItemIndex)%2!=0?"</div>":"")%>
        </ItemTemplate>
        <FooterTemplate>
        <%# (_itemsRepeater.Items.Count % 2 != 0 ? "<div class=\"item\"></div></div>" : "")%>
        </FooterTemplate>
    </asp:Repeater>
</div>
