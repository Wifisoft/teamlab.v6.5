<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Confirm.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Common.Confirm" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Import Namespace="ASC.Core.Common" %>
<div id="studio_confirmDialog<%=AdditionalID%>" style="display: none;">
    <ascwc:Container runat="server" ID="_studioConfirm">
        <Header>
            <%= Title.HtmlEncode()%>
        </Header>
        <Body>
            <asp:HiddenField runat="server" ID="_confirmEnterCode" />
            <div id="studio_confirmMessage"></div>
            <div class="clearFix">
                <div>
                    <%=SelectTitle%>:
                </div>
                <div style="margin: 3px 0px;">
                    <input id="studio_confirmInput<%=AdditionalID%>" class="textEdit" type="text" value="<%=Value %>"  style="width: 350px;" maxlength="100" />
                </div>
            </div>
            <div class="clearFix" style="margin-top: 16px;">
                    <a class="baseLinkButton" href="javascript:void(0);" onclick="StudioConfirm.Select('<%=AdditionalID%>',<%=SelectJSCallback%>);" style="float: left;"><%=Resources.Resource.SaveButton%></a> 
                    <a class="grayLinkButton" href="javascript:void(0);" onclick="StudioConfirm.Cancel();" style="float: left;margin-left: 8px;"><%=Resources.Resource.CancelButton%></a>
            </div>
        </Body>
    </ascwc:Container>
</div>
