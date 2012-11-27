<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpleUserSelector.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.SimpleUserSelector" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Core.Common" %>
<div id="studio_simpleUserSelectorDialog<%=AdditionalID%>" style="display: none;">    
    <ascwc:Container runat="server" ID="_simpleUserSelectorContainer">
        <Header>
            <%=Title.HtmlEncode()%>
        </Header>
        <Body>
        <asp:HiddenField runat="server" ID="_selectEnterCode" />
            <div class="clearFix">
                <div>
                    <%=SelectTitle.HtmlEncode()%>:
                </div>
                <div style="margin: 3px 0px;">
                    <select  class="comboBox"  id="studio_simple_user_selector<%=AdditionalID%>" style="width: 350px;">
                        <option value="<%=Guid.Empty %>" <%=(UserID==Guid.Empty? "selected" : "") %>></option>
                        <%foreach (ASC.Core.Users.UserInfo user in GetSortedUsers())
                          {%>
                        <option value="<%=user.ID%>" <%=(user.ID == UserID ? "selected" : "") %> >
                            <%=DisplayUserSettings.GetFullUserName(user)%></option>
                        <%}%>
                    </select>
                </div>
            </div>
            <div class="clearFix" style="margin-top: 16px;">
                    <a class="baseLinkButton" href="javascript:void(0);" onclick="StudioSimpleUserSelector.Select('<%=AdditionalID%>',<%=SelectJSCallback%>);" style="float: left;"><%=Resources.Resource.SelectButton%></a> 
                    <a class="grayLinkButton" href="javascript:void(0);" onclick="StudioSimpleUserSelector.Cancel();" style="float: left;margin-left: 8px;"><%=Resources.Resource.CancelButton%></a>
            </div>
        </Body>
    </ascwc:Container>
</div>
