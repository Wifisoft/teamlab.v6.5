<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminRepeater.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AdminRepeater" %>
 <asp:Repeater ID="_adminsRepeater" runat="server">
        <HeaderTemplate>
         <table cellpadding="3" cellspacing="0" style="width:100%;">
            <tr class="headerColumn">
                <td class="borderAdminHeaderRow" style="width:34px;">&nbsp;</td>
                <td class="borderAdminHeaderRow"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Employees").HtmlEncode() %></td>
                <td class="borderAdminHeaderRow"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Department").HtmlEncode() %></td>
                <td class="borderAdminHeaderRow"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("UserPost").HtmlEncode()%></td>
            </tr>
         </HeaderTemplate>
        <ItemTemplate>                        
             <tr class="<%#Container.ItemIndex%2==0?"tintMedium" : "tintLight" %>">
                <td class="borderAdminRow"><img alt="" class="userMiniPhoto" src="<%#Eval("PhotoUrl") %>"/></td>
                <td class="borderAdminRow">
                    <div class="clearFix"><a href="<%#Eval("UserUrl") %>" class="linkHeaderLightMedium" title="<%#Eval("UserName")%>"><%#Eval("UserName")%></a></div>                    
                </td>
                <%#String.IsNullOrEmpty((string)Eval("DepUrl"))?"<td class=\"borderAdminRow\"><div>&nbsp;</div></td>":
                                       "<td class=\"borderAdminRow\"><div><a href=\"" + Eval("DepUrl") + "\" title=\"" + Eval("DepName") + "\">" + Eval("DepName") + "</a></div></td>"
                %>
                <td class="borderAdminRow"><div title="<%#Eval("UserPost")%>"><%#Eval("UserPost")%></div></td>
              </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>