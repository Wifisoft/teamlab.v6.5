<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactSelectorRow.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.ContactSelectorRow" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>

<div id="item_<%= ObjID %>" class="contactSelector-item">

    <div id="selectorContent_<%= ObjID %>" style="position: relative;margin-bottom:10px;<%=GetCssSelectorStyle()%>">

        <table cellspacing="0" cellpadding="1" class="contactSelector-inputContainer" width="100%" style="height: 18px;">
            <tbody>
                <tr>
                    <td width="16px" class="borderBase" style="border-right: 0 none;">
                        <img align="absmiddle" id="searchImg_<%= ObjID %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("search.png", ProductEntryPoint.ID  ) %>"
                        onclick="<%= ObjName %>.quickSearch('<%= ObjID %>');"
                        style="cursor:pointer"/>
                        <img align="absmiddle" id="loaderImg_<%= ObjID %>" src="<%= WebImageSupplier.GetAbsoluteWebPath("mini_loader.gif" ) %>" style="display:none;"/>
                    </td>
                    <td class="borderBase" style="border-left: 0 none; border-right: 0 none;">
                        <input type="text" id="contactTitle_<%= ObjID %>" value="<%=GetContactTitle() %>" class="textEdit" autocomplete="off"/>
                        <input type="hidden" id="contactID_<%= ObjID %>" value="<%=GetContactID() %>"/>
                    </td>
                    <td width="16px" class="borderBase" style="border-left: 0 none;">
                        <img align="absmiddle" src="<%= WebImageSupplier.GetAbsoluteWebPath("cross-grey.png", ProductEntryPoint.ID  ) %>"
                         onclick="<%= ObjName %>.crossButtonEventClick('<%= ObjID %>');" style="cursor:pointer;display:none;"
                         class="crossButton" alt="<%=CRMCommonResource.Cancel%>" title="<%=CRMCommonResource.Cancel%>"/>
                    </td>


                    <% if (ShowDeleteButton)%>
                    <% { %>
                    <td width="16px;">
                        <a class="crm-deleteLink" title="<%=DeleteContactText%>" onclick="<%= ObjName %>.deleteContact('<%= ObjID %>');">
                            &nbsp;
                        </a>
                    </td>
                    <% } %>

                    <% if (ShowAddButton) %>
                    <% { %>
                    <td width="16px;">
                        <a title="<%= AddButtonText %>" class="crm-addNewLink" onclick="<%=ObjName%>.AddNewSelector(jq(this));">
                            &nbsp;
                        </a>
                    </td>
                    <% } %>
                </tr>
            </tbody>
        </table>


        <div id="noMatches_<%= ObjID %>" class="borderBase noMatches">
            <div style="padding: 5px;">
                <%= CRMCommonResource.NoMatches %>
            </div>
            <% if (ShowNewCompanyContent) %>
            <% { %>
            <div style="padding: 0pt 5px">
                <a class="linkAddSmallText baseLinkAction" onclick="<%= ObjName %>.showNewCompany('<%= ObjID %>');">
                    <%=CRMContactResource.AddNewCompany%>
                </a>
            </div>
            <% } %>
            <% if (ShowNewContactContent) %>
            <% { %>
            <div style="padding: 5px;">
                <a class="linkAddSmallText baseLinkAction" onclick="<%= ObjName %>.showNewContact('<%= ObjID %>');">
                    <%=CRMContactResource.AddNewContact %>
                </a>
            </div>
            <% } %>
        </div>

    </div>
    <% if (!ShowOnlySelectorContent) %>
    <% { %>
    <div id="infoContent_<%= ObjID %>" style="<%=SelectedContact==null ? "display: none;" : string.Empty%>">

        <table width="100%" cellspacing="0" cellpadding="3">
            <tbody>
                <tr>
                    <% if (ShowContactImg) %>
                    <% { %>
                    <td width="40px">
                        <img src="<%= GetContactImgSrc() %>" />
                    </td>
                    <% } %>
                    <td>
                        <span class="splitter">
                            <b><%= GetContactTitle()%></b>
                        </span>
                        <% if (ShowChangeButton)%>
                        <% { %>
                        <a class="crm-editLink" title="<%= CRMCommonResource.UnlinkContact%>" onclick="<%= ObjName %>.changeContact('<%= ObjID %>');">
                            &nbsp;
                        </a>
                        <% } %>
                        <% if (ShowDeleteButton)%>
                        <% { %>
                        <a class="crm-deleteLink" title="<%=DeleteContactText%>" onclick="<%= ObjName %>.deleteContact('<%= ObjID %>');">
                            &nbsp;
                        </a>
                        <% } %>

                        <% if (ShowAddButton)%>
                        <% { %>
                        <a title="<%= AddButtonText %>" class="crm-addNewLink" onclick="<%=ObjName%>.AddNewSelector(jq(this));">
                            &nbsp;
                        </a>
                        <% } %>

                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <% } %>
    <% if (ShowNewCompanyContent || ShowNewContactContent) %>
    <% { %>
    <div id="newContactContent_<%= ObjID %>" style="display: none;">

        <table width="100%" cellspacing="0" cellpadding="3">
            <tbody>
                <tr>
                    <td width="40px">
                        <% if (ShowNewCompanyContent) %>
                        <% { %>
                        <img id="newCompanyImg_<%= ObjID %>" src="<%= ContactPhotoManager.GetSmallSizePhoto(0, true) %>" />
                        <% } %>
                        <% if (ShowNewContactContent) %>
                        <% { %>
                        <img id="newContactImg_<%= ObjID %>" src="<%= ContactPhotoManager.GetSmallSizePhoto(0, false) %>" style="display:none;"/>
                        <% } %>
                        <input type="hidden" id="hiddenIsCompany_<%= ObjID %>" />
                    </td>
                    <td style="white-space:nowrap;">
                        <span class="splitter">
                        <% if (ShowNewCompanyContent) %>
                        <% { %>
                        <input type="text" id="newCompanyTitle_<%= ObjID %>" class="textEdit" autocomplete="off" style="width:200px;"/>
                        <% } %>
                        <% if (ShowNewContactContent) %>
                        <% { %>
                        <input type="text" id="newContactFirstName_<%= ObjID %>" class="textEdit" autocomplete="off" style="width:200px;display:none;"/>
                        <input type="text" id="newContactLastName_<%= ObjID %>" class="textEdit" autocomplete="off" style="width:200px;display:none;"/>
                        <% } %>
                        </span>
                        <a class="crm-acceptLink" title="<%=CRMCommonResource.Add%>" onclick="<%= ObjName %>.acceptNewContact('<%= ObjID %>');">
                            &nbsp;
                        </a>
                        <a class="crm-rejectLink" title="<%=CRMCommonResource.Cancel%>" onclick="<%= ObjName %>.rejectNewContact('<%= ObjID %>');">
                            &nbsp;
                        </a>

                        <% if (ShowAddButton)%>
                        <% { %>
                        <a title="<%= AddButtonText %>" class="crm-addNewLink" onclick="<%=ObjName%>.AddNewSelector(jq(this));">
                            &nbsp;
                        </a>
                        <% } %>

                    </td>
                </tr>
            </tbody>
        </table>

    </div>
    <% } %>
</div>




