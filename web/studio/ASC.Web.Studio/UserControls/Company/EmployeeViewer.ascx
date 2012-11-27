<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeViewer.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Company.EmployeeViewer" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common"
    TagPrefix="ascws" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Users"
    TagPrefix="ascws" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.UserControls" TagPrefix="ascwsuc" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascwc" %>
<%@ Register Src="~/UserControls/Company/DepartmentEdit.ascx" TagPrefix="empUC" TagName="DepEdit" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Company" %>
<asp:PlaceHolder ID="EmployeeList" runat="Server">
    <ascwc:Container ID="EmployeeContainer" runat="server">
        <Header>
            <%=CustomNamingPeople.Substitute<Resources.Resource>("Employees").HtmlEncode()%></Header>
        <Body>
            <%--nothing found--%>
            <ascws:EmptyScreenControl Visible="False" runat="server" ID="_notFoundMessage"></ascws:EmptyScreenControl>
            <ascwc:ViewSwitcher ID="EmployeeTabs" runat="server" DisableJavascriptSwitch="true">
                <TabItems>
                    <ascwc:ViewSwitcherTabItem runat="server" ID="ListTab">
                        <asp:PlaceHolder runat="server" ID="_empContentHolder">
                            <%--emp list--%>
                            <asp:Repeater ID="rprListEmployee" runat="Server">
                                <HeaderTemplate>
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="tableContainer">
                                        <tbody>
                                            <tr style="height: 21px;">
                                                <th align="left" valign="middle">
                                                    <asp:HyperLink ID="hlEmployee" runat="Server" Text='<%#CustomNamingPeople.Substitute<Resources.Resource>("Employee")%>'
                                                        NavigateUrl='<%#GetSortUrl(EmployeeSortType.Name)%>' /><img style="margin: 0 0 -4px 2px;"
                                                            width="12" src="<%= _sortType == EmployeeSortType.Name ? _sortedImg : _blankImg %>" />
                                                </th>
                                                <th align="left" valign="middle">
                                                    <asp:HyperLink ID="hlEmployeeSector" runat="Server" Text='<%#CustomNamingPeople.Substitute<Resources.Resource>("Department")%>'
                                                        NavigateUrl='<%#GetSortUrl(EmployeeSortType.Sector)%>' /><img style="margin: 0 0 -4px 2px;"
                                                            width="12" src="<%= _sortType == EmployeeSortType.Sector ? _sortedImg : _blankImg %>" />
                                                </th>
                                                <th align="left" valign="middle">
                                                    <asp:HyperLink ID="hlEmployeeRole" runat="Server" Text='<%#ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("UserPost")%>'
                                                        NavigateUrl='<%#GetSortUrl(EmployeeSortType.Role)%>' /><img style="margin: 0 0 -4px 2px;"
                                                            width="12" src="<%= _sortType == EmployeeSortType.Role ? _sortedImg : _blankImg %>" />
                                                </th>
                                                <th style="width: 120px;" align="left" valign="middle">
                                                    <asp:HyperLink ID="hlEmployeeAdmissionDate" runat="Server" Text='<%# ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("WorkFromDate")%>'
                                                        NavigateUrl='<%#GetSortUrl(EmployeeSortType.AdmissionDate)%>' /><img style="margin: 0 0 -4px 2px;"
                                                            width="12" src="<%= _sortType == EmployeeSortType.AdmissionDate ? _sortedImg : _blankImg %>" />
                                                </th>
                                            </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class="<%#(Container.ItemIndex % 2 == 0) ? "even" : "odd"%> employee<%#((Container.DataItem as UserInfo).ActivationStatus == EmployeeActivationStatus.Pending)? " pending" : ""%>">
                                        <td style="padding-left: 0px;">
                                            <a href="<%#GetEmployeeUrl((Container.DataItem as UserInfo)) %>" style="float: left;">
                                                <img class="userMiniPhoto" style="margin: 4px 15px 4px 8px;" src="<%#UserPhotoManager.GetSmallPhotoURL((Container.DataItem as UserInfo).ID)%>" /></a>
                                            <div style="width: 180px; overflow: hidden; margin-left: 60px; margin-top: 13px; text-overflow: ellipsis;">
                                                <asp:HyperLink ID="hlEmployeeName" CssClass="linkHeaderLightMedium" runat="Server"
                                                    Text='<%#GetEmployeeName((Container.DataItem as UserInfo))%>' ToolTip='<%# HttpUtility.HtmlDecode(GetEmployeeName((Container.DataItem as UserInfo)))%>'
                                                    NavigateUrl='<%#GetEmployeeUrl((Container.DataItem as UserInfo)) %>' />&nbsp;
                                            </div>
                                        </td>
                                        <td style="padding-top: 10px; padding-bottom: 10px;">
                                            <div style="width: 146px; overflow: hidden; text-overflow: ellipsis;">
                                                <%#(Container.DataItem as UserInfo).Status == EmployeeStatus.Terminated? HttpUtility.HtmlEncode((Container.DataItem as UserInfo).Department??""):
                                        ("<a href=\"" + GetEmployeeSectorUrl(Container.DataItem as UserInfo) + "\"+ title=\"" + HttpUtility.HtmlEncode((Container.DataItem as UserInfo).Department) + "\" >" + HttpUtility.HtmlEncode((Container.DataItem as UserInfo).Department) + "</a>")%>&nbsp;
                                            </div>
                                        </td>
                                        <td style="padding-top: 10px; padding-bottom: 10px; color: #282826;">
                                            <div style="width: 146px; overflow: hidden; text-overflow: ellipsis;">
                                                <%#(Container.DataItem as UserInfo).Title.HtmlEncode()%>
                                                &nbsp;
                                            </div>
                                        </td>
                                        <td style="padding-top: 10px; padding-bottom: 10px; color: #282826;">
                                            <div style="width: 117px; overflow: hidden;">
                                                <%#((Container.DataItem as UserInfo).ActivationStatus == EmployeeActivationStatus.Pending)? Resources.Resource.PendingTitle : 
                                                (Container.DataItem as UserInfo).WorkFromDate == null ? string.Empty : (Container.DataItem as UserInfo).WorkFromDate.Value.ToShortDateString()%>&nbsp;
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <%--emp cards--%>
                            <asp:Repeater ID="rprCardEmployee" runat="Server">
                                <HeaderTemplate>
                                    <div>
                                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phTREndStaff" runat="Server" Visible='<%#Container.ItemIndex % 2 == 0 && Container.ItemIndex != 0%>'>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="height: 16px">
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTRStaff" runat="Server" Visible='<%#Container.ItemIndex % 2 == 0 && Container.ItemIndex != (rprCardEmployee.DataSource as IList).Count - 1%>'>
                                        <tr style="min-height: 122px; height: 100%">
                                    </asp:PlaceHolder>
                                    <td style="padding: 10px; height: 100%;" class="tintMedium borderBase">
                                        <ascws:EmployeeUserCard ID="ucEmployeeUserCard" runat="server" EmployeeInfo='<%#(Container.DataItem as UserInfo)%>'
                                            EmployeeUrl='<%#GetEmployeeUrl(Container.DataItem as UserInfo)%>' Height="100%"
                                            Width="330" EmployeeDepartmentUrl='<%#GetEmployeeSectorUrl(Container.DataItem as UserInfo)%>'>
                                        </ascws:EmployeeUserCard>
                                    </td>
                                    <asp:PlaceHolder ID="phEmptyCell" runat="Server" Visible='<%#Container.ItemIndex % 2 == 0%>'>
                                        <td style="width: 16px;">
                                            &nbsp;
                                        </td>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTDEnd" runat="Server" Visible='<%#Container.ItemIndex == (rprCardEmployee.DataSource as IList).Count - 1%>'>
                                        <asp:PlaceHolder ID="phOneTDEnd" runat="Server" Visible='<%#(Container.ItemIndex) % 2 == 0 %>'>
                                            <td style="width: 352px; padding-bottom: 9px; height: 100%;">
                                                &nbsp;
                                            </td>
                                        </asp:PlaceHolder>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tr> </table> </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </asp:PlaceHolder>
                    </ascwc:ViewSwitcherTabItem>
                    <ascwc:ViewSwitcherTabItem runat="server" ID="DepTab" HideSortItems="true">
                        <%--deps--%>
                        <asp:Repeater ID="rptDepartment" runat="Server">
                            <HeaderTemplate>
                                <div class="headerBase">
                                    <%#ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("CEO").HtmlEncode()%>
                                </div>
                                <div class="clearFix" style="margin-bottom: 30px; margin-top: 10px;">
                                    <div style="padding: 10px; <%#GetCeoUserInfo()==null?"display:none; height:100px;": ""%> margin: 0px;
                                        float: left;" class="tintMedium borderBase">
                                        <ascws:EmployeeUserCard ID="ucCEOUserCard" runat="server" EmployeeInfo='<%#GetCeoUserInfo()%>'
                                            EmployeeUrl='<%#GetEmployeeUrl(GetCeoUserInfo())%>' Height="100%" EmployeeDepartmentUrl='<%#GetEmployeeSectorUrl(GetCeoUserInfo())%>'>
                                        </ascws:EmployeeUserCard>
                                    </div>
                                    <%if (CanEditDepartment())
                                      {%>
                                    <a href="javascript:StudioSimpleUserSelector.OpenDialog('ceo');" class="linkAction<%=(SetupInfo.WorkMode==WorkMode.Promo?" promoAction":"")%>"
                                        style="<%#GetCeoUserInfo()==null?"": "margin-top: 95px; margin-right:5px; float:right;"%>">
                                        <%=Resources.Resource.EditButton%></a>
                                    <%} %>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="departmentItem borderBase <%#(Container.ItemIndex % 2 == 0) ?"":" tintMedium"%>"
                                    style="border-right: none; border-left: none; border-bottom: none; overflow: hidden;">
                                    <div style="padding: 10px 8px;">
                                        <a class="linkHeaderLightBig" href="<%#GetDepartmentsLink((Container.DataItem as ASC.Core.Users.GroupInfo).ID) %>"
                                            title="<%#GetGroupName(Container.DataItem as ASC.Core.Users.GroupInfo)%>">
                                            <%#GetGroupName(Container.DataItem as ASC.Core.Users.GroupInfo)%></a></div>
                                    <table cellpadding="0" cellspacing="0" style="width: 100%">
                                        <tr>
                                            <td align="left" style="width: 100px; padding: 4px 8px 4px 28px;">
                                                <nobr><%#ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("DepartmentMaster").HtmlEncode()%>:</nobr>
                                            </td>
                                            <td align="left" style="padding: 4px 0px;">
                                                <nobr><div style="max-width:400px; overflow:hidden;"><%#GetMasterRenderLink(Container.DataItem as ASC.Core.Users.GroupInfo)%></div></nobr>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 100px; padding: 4px 8px 15px 28px;">
                                                <nobr><%#Resources.Resource.DepartmentComposition%>:</nobr>
                                            </td>
                                            <td style="padding: 4px 0px 15px;" align="left">
                                                <nobr><asp:HyperLink ID="HyperLink1" runat="Server" NavigateUrl='<%#GetDepartmentsLink((Container.DataItem as ASC.Core.Users.GroupInfo).ID)%>'><%#GetContainGroupCaption(Container.DataItem as ASC.Core.Users.GroupInfo)%></asp:HyperLink></nobr>
                                            </td>
                                            <td align="right" style="padding: 4px 5px 15px 0px;">
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="Server" Visible='<%#CanEditDepartment() %>'>
                                                    <asp:HyperLink ID="HyperLink3" runat="Server" CssClass="linkAction" NavigateUrl='<%#GetDepartmentsLink((Container.DataItem as ASC.Core.Users.GroupInfo).ID) %>'><%#Resources.Resource.EditButton%></asp:HyperLink>
                                                    <span class='splitter'>|</span>
                                                    <a onclick="javascript:return confirm('<%#string.Format(CustomNamingPeople.Substitute<Resources.Resource>("DeleteDepartmentConfirmation").HtmlEncode(), GetGroupName(Container.DataItem as ASC.Core.Users.GroupInfo)).ReplaceSingleQuote()%>');"
                                                        class="linkAction" href="javascript:EmployeeService.DeleteDepartment('<%#(Container.DataItem as ASC.Core.Users.GroupInfo).ID %>');window.location.reload();">
                                                        <%#Resources.Resource.DeleteButton%></a> 
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <empUC:DepEdit runat="Server" ID="ucDepartmentEdit" />
                    </ascwc:ViewSwitcherTabItem>
                </TabItems>
                <SortItems>
                    <ascwc:ViewSwitcherLinkItem runat="server" ID="ListViewLink" />
                    <ascwc:ViewSwitcherLinkItem runat="server" ID="CardViewLink" />
                </SortItems>
            </ascwc:ViewSwitcher>
            <%--navigation--%>
            <div class="clearFix" style="margin-top: 14px;">
                <ascwc:PageNavigator ID="EmployeeCardsPageNavigator" EntryCountOnPage="12" VisiblePageCount="8"
                    ParamName="page" runat="server" />
            </div>
        </Body>
    </ascwc:Container>
</asp:PlaceHolder>
