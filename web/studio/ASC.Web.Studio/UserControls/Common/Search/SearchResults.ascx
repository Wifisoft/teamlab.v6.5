<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.Search.SearchResults" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Search" %>
<%@ Register Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="asc" Assembly="ASC.Web.Studio" %>
<asp:Repeater ID="results" runat="server">
    <ItemTemplate>
        <div style="padding: 10px;" class="clearFix">
            <div style="vertical-align: middle; float: left;cursor:pointer;" class="headerBase" onclick="Search.Toggle('<%# Container.FindControl("resultItems").ClientID %>','<%# Container.FindControl("btnToggleNavigator").ClientID %>')">
                <img align="absmiddle" alt="<%# ((SearchResult)Container.DataItem).Name %>" style="margin-right: 5px" src="<%# ((SearchResult)Container.DataItem).LogoURL %>" />
                <%# ((SearchResult)Container.DataItem).Name.HtmlEncode()%>
                <img id="btnToggleNavigator" runat="server" class="controlButton" />
            </div>
            
            <div id="oper_<%# Container.ItemIndex %>" style="float: right; padding-top: 10px;display:<%# ((SearchResult)Container.DataItem).Items.Count > ((SearchResult)Container.DataItem).PresentationControl.MaxCount?"block":"none" %> ">
                <%=Resources.Resource.TotalFinded %>: <%# ((SearchResult)Container.DataItem).Items.Count%>&nbsp;&nbsp;|&nbsp;&nbsp;<span style="text-decoration:none;border-bottom:1px dotted;color: #333333;cursor:pointer;"
                    onclick="javascript:SearchManager.ShowAll(this,'<%# Container.FindControl("resultItems").ClientID %>','<%#((SearchResult)Container.DataItem).ProductID %>','<%# Container.ItemIndex %>','<%# ((SearchResult)Container.DataItem).Items.Count %>');"><%= Resources.Resource.ShowAllSearchResult %></span>
            </div>
        </div>
        <div id="resultItems" runat="server" class="borderBase1 searchResults"></div>
    </ItemTemplate>
</asp:Repeater>

<script type="text/javascript">
    jq(function() {
        jq("div.headerBase").each(function() {
            jq(this).find("img.controlButton").attr("src", "" + SkinManager.GetImage('collapse_down_dark.png') + "");
        });
        SearchManager.Label = '<%=Resources.Resource.TotalFinded %>';
    });
</script>