<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductQuotes.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Statistics.ProductQuotes" %>

<%-- Quotes --%>
<div class="quotesBlock">
  <div class="information">
    <table class="information">
      <tr>
        <td class="name"><%=Resources.Resource.TenantCreated%>:</td>
        <td class="value"><%=RenderCreatedDate()%></td>
      </tr>
      <tr>
        <td class="name"><%=Resources.Resource.TenantUsersTotal%>:</td>
        <td class="value"><%=RenderUsersTotal()%></td>
      </tr>      
    </table>
  </div>
  
  <div class="headerBase storeUsage">
    <%=Resources.Resource.TenantUsedSpace%>: <span class="diskUsage"><%=RenderUsedSpace()%></span>
 
   <%if (!ASC.Core.CoreContext.Configuration.Standalone && _tariff.State != ASC.Core.Billing.TariffState.Trial)
   { %>
        <div class="description">
            <%=String.Format(Resources.Resource.TenantUsedSpacePremiumDescription, GetMaxTotalSpace())%>
        </div>
         </div>
               
   <%}
else
{ 
         if (!ASC.Core.CoreContext.Configuration.Standalone) { %>
        <div class="description">
            <%=String.Format(Resources.Resource.TenantUsedSpaceFreeDescription, GetMaxTotalSpace())%>
        </div>    
        <% } %>
         </div>  
         <div style="margin-top:10px; margin-left:20px;">  
            <asp:PlaceHolder runat="server" ID="_buyNowHolder"></asp:PlaceHolder>
        </div>
<%} %>

  
  <asp:Repeater ID="_itemsRepeater" runat="server" >
        <ItemTemplate>       
            <div class="headerBase borderBase header">
                 <img align="absmiddle" src="<%#Eval("Icon")%>" alt="" /> <%#HttpUtility.HtmlEncode((string)Eval("Name"))%>
             </div>                
            <div class="contentBlock">
                <asp:Repeater ID="_usageSpaceRepeater" runat="server">
                  <HeaderTemplate>
                    <table class="quotes">
                    <tr class="describeText"><td colspan="2"><%=Resources.Resource.SourceForDiskUsage%></td><td align="right"><%=Resources.Resource.DiskUsageTitle%></td>
                    </tr>
                  </HeaderTemplate>
                      <ItemTemplate>
                        <tr class="borderBase<%#Container.ItemIndex % 2 == 0 ? " even" : " odd"%>"<%#Container.ItemIndex>=10?"style=\"display:none;\"":"" %>>
                          <td class="icon">
                            <%#String.IsNullOrEmpty((string)Eval("Icon"))?"": "<img src=\""+Eval("Icon")+"\" alt=\"\" />"%>
                            
                          </td>
                          <td class="name">
                            <div>
                                <%#String.IsNullOrEmpty((string)Eval("Url"))?
                                    "<span class=\"headerBaseSmall\" title=\"" + HttpUtility.HtmlEncode((string)Eval("Name")) + "\">" + HttpUtility.HtmlEncode((string)Eval("Name")) + "</span>" :
                                    "<a href=\""+Eval("Url")+"\" class=\"linkHeaderSmall\" title=\"" + HttpUtility.HtmlEncode((string)Eval("Name")) + "\">"+HttpUtility.HtmlEncode((string)Eval("Name"))+"</a>"
                                %>
                            </div>
                          </td>
                          <td class="value">
                            <%#Eval("Size")%>
                          </td>
                        </tr>
                      </ItemTemplate>
                  <FooterTemplate>
                  </table>
                        
                    
                  </FooterTemplate>
                </asp:Repeater>
                
                <asp:PlaceHolder ID="_showMorePanel" runat="server">
                <div class="moreBox">
                    <%=Resources.Resource.Top10Title %>
                    <a class="linkDescribeMedium baseLinkAction" style="margin-left: 15px;"><%=Resources.Resource.ShowAllButton%></a>
                </div>
                </asp:PlaceHolder>
                
                 <asp:PlaceHolder ID="_emptyUsageSpace" runat="server">
                <div class="emptySpace">
                    <%=Resources.Resource.EmptyUsageSpace%>                                
                </div>
                </asp:PlaceHolder>
                
            </div>            
        </ItemTemplate>
    </asp:Repeater>
    
</div>

<script language="javascript">
    jq(function() {
        jq(".moreBox a.linkDescribeMedium").click(function() {
            jq(jq(this).parent().prev()).find('tr').show();
            jq(this).parent().hide();
        });
    })
</script>
