<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainContent.ascx.cs" Inherits="ASC.Web.Files.Controls.MainContent" %>
<%@ Import Namespace="ASC.Files.Core" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="aswscc" %>

<% #if (DEBUG) %>
<link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/MainContent/maincontent.css" )%>" type="text/css" rel="stylesheet" />
<link href="<%=PathProvider.GetFileStaticRelativePath("common.css")%>" type="text/css" rel="stylesheet" />
<% #else %>
<link href="<%=PathProvider.GetFileStaticRelativePath("files-min.css")%>" type="text/css" rel="stylesheet" />
<% #endif %>

<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("zeroclipboard.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("common.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("templatemanager.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("servicemanager.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("ui.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("eventhandler.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("foldermanager.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("actionmanager.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("tree.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("dragdrop.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("filter.js") %>"></script>
<script type="text/javascript" language="javascript" src="<%= PathProvider.GetFileStaticRelativePath("auth.js") %>"></script>

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/MainMenu/mainmenu.js" ) %>"></script>
<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/MainMenu/uploadmanager.js" ) %>"></script>

<%-- Info panel --%>
<div id="infoPanelContainer" class="infoPanel files_infoPanel">
    <div>&nbsp;</div>
</div>

<%-- header --%>
<div id="folderHeader" class="clearFix">
    <div id="files_breadCrumbsContainer">
        <div id="files_breadCrumbsRoot">
            <a id="files_breadCrumbsCaption"></a>
            <span id="files_breadCrumbsNew" class="new_inshare"></span>
            <img id="files_showTreeView" title="<%=FilesUCResource.OpenTree%>"
                alt="<%=FilesUCResource.OpenTree%>" src="<%=PathProvider.GetImagePath("combobox_black.png")%>" />
        </div>
        <div id="files_breadCrumbs">
        </div>
    </div>
    <div id="currentFolderInfo">
    </div>
    
    <%-- TREE --%>
    <div id="files_treeViewPanel" class="treeViewPanel files_popup_win" style="margin-top: 1px;">
        <div class="popup-corner" style="left: 7px;">
        </div>
        <div class="jstree">
            <ul id="files_trewViewContainer">
                <%if (FolderIDUserRoot!=null)%>
                <%{ %>
                <li data-id="<%=FolderIDUserRoot%>" class="tree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.MyFiles%>" href="#<%=FolderIDUserRoot%>" >
                        <span class="jstree-icon myFiles" ></span>
                        <%=FilesUCResource.MyFiles%>
                    </a>
                </li>
                <%} %>
                <%if (FolderIDShare!=null)%>
                <%{ %>
                <li data-id="<%=FolderIDShare%>" class="tree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.SharedForMe%>" href="#<%=FolderIDShare%>" >
                        <span class="jstree-icon shareformeFiles" ></span>
                        <%=FilesUCResource.SharedForMe%>
                    </a>
                    <span class="new_inshare" data-id="<%=FolderIDShare%>"></span>
                </li>
                <%} %>
                <%if (FolderIDCommonRoot!=null)%>
                <%{ %>
                <li data-id="<%=FolderIDCommonRoot%>" class="tree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.CorporateFiles%>" href="#<%=FolderIDCommonRoot%>" >
                        <span class="jstree-icon corporateFiles" ></span>
                        <%=FilesUCResource.CorporateFiles%>
                    </a>
                </li>
                <%} %>
                <%if (FolderIDCurrentRoot!=null)
                  { %>
                <li data-id="<%=FolderIDCurrentRoot%>" class="tree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.ProjectFiles%>" href="#<%=FolderIDCurrentRoot%>" >
                        <span class="jstree-icon projectFiles" ></span>
                        <%=FilesUCResource.ProjectFiles%>
                    </a>
                </li>
                <%} %>
                <%if (FolderIDTrash!=null)
                  { %>
                <li data-id="<%=FolderIDTrash%>" class="tree_node jstree-closed">
                    <span class="jstree-icon" style="visibility: hidden;"></span>
                    <a title="<%=FilesUCResource.Trash%>" href="#<%=FolderIDTrash%>" >
                        <span class="jstree-icon trashFiles"></span>
                        <%=FilesUCResource.Trash%>
                    </a>
                </li>
                <%} %>
            </ul>
        </div>
    </div>
    <%if (ASC.Core.SecurityContext.IsAuthenticated)
      { %>
    <div id="files_treeViewPanelSelector" class="treeViewPanel files_popup_win">
        <div class="popup-corner" style="left: 21.5px;">
        </div>
        <div style="margin-bottom: 5px;">
            <b>
                <%=FilesUCResource.SelectFolder%></b>
        </div>
        <div class="jstree">
            <ul id="files_trewViewSelector">
                <%if (FolderIDUserRoot!=null)
                  { %>
                <li data-id="<%=FolderIDUserRoot%>" class="stree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.MyFiles%>" href="#<%=FolderIDUserRoot%>">
                        <span class="jstree-icon myFiles" ></span>
                        <%=FilesUCResource.MyFiles%>
                    </a>
                </li>
                <%} %>
                <%if (FolderIDCommonRoot!=null)
                  { %>
                <li data-id="<%=FolderIDCommonRoot%>" class="stree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.CorporateFiles%>" href="#<%=FolderIDShare%>">
                        <span class="jstree-icon corporateFiles" ></span>
                        <%=FilesUCResource.CorporateFiles%>
                    </a>
                </li>
                <%} %>
                <%if (FolderIDCurrentRoot!=null)
                  { %>
                <li data-id="<%=FolderIDCurrentRoot%>" class="stree_node jstree-closed">
                    <span class="jstree-icon expander"></span>
                    <a title="<%=FilesUCResource.ProjectFiles%>" href="#<%=FolderIDShare%>" >
                        <span class="jstree-icon projectFiles" ></span>
                        <%=FilesUCResource.ProjectFiles%>
                    </a>
                </li>
                <%} %>
            </ul>
        </div>
    </div>
    <%} %>
</div>

<%-- Advansed Filter --%>
<div id="files_filterContainer">
    <aswscc:advansedfilter runat="server" id="filter" blockid="files_advansedFilter"></aswscc:advansedfilter>

    <script type="text/javascript">
        (function($) {
            if (!jq("#files_advansedFilter").advansedFilter) retrun;

            ASC.Files.Filter.advansedFilter = jq("#files_advansedFilter")
                .advansedFilter({
                    anykey: true,
                    anykeytimeout: 800,
                    maxfilters: 1,
                    maxlength: "<%=Global.MAX_TITLE%>",
                    filters: [
                            { type: "combobox", id: "selected-type", title: "<%=FilesUCResource.Types%>", options: [
                                { value: "1", title: "<%=FilesUCResource.ButtonFilterFile%>" },
                                { value: "2", title: "<%=FilesUCResource.ButtonFilterFolder%>", def: true },
                                { value: "3", title: "<%=FilesUCResource.ButtonFilterDocument%>" },
                                { value: "4", title: "<%=FilesUCResource.ButtonFilterPresentation%>" },
                                { value: "5", title: "<%=FilesUCResource.ButtonFilterSpreadsheet%>" },
                                { value: "7", title: "<%=FilesUCResource.ButtonFilterImage%>"}]
                            },
                            { type: "person", id: "selected-person", title: "<%=FilesUCResource.Users%>" },
                            { type: "group", id: "selected-group", title: "<%=FilesUCResource.Departments%>" }
                        ],
                    sorters: [
                            { id: "DateAndTime", title: "<%=FilesUCResource.ButtonSortModified%>", def: <%=(ContentOrderBy.SortedBy == SortedByType.DateAndTime).ToString().ToLower() %>, dsc: <%=(!ContentOrderBy.IsAsc).ToString().ToLower() %>},
                            { id: "AZ", title: "<%=FilesUCResource.ButtonSortName%>", def: <%=(ContentOrderBy.SortedBy == SortedByType.AZ).ToString().ToLower() %>, dsc: <%=(!ContentOrderBy.IsAsc).ToString().ToLower() %>},
                            { id: "Type", title: "<%=FilesUCResource.ButtonSortType%>", def: <%=(ContentOrderBy.SortedBy == SortedByType.Type).ToString().ToLower() %>, dsc: <%=(!ContentOrderBy.IsAsc).ToString().ToLower() %>},
                            { id: "Size", title: "<%=FilesUCResource.ButtonSortSize%>", def: <%=(ContentOrderBy.SortedBy == SortedByType.Size).ToString().ToLower() %>, dsc: <%=(!ContentOrderBy.IsAsc).ToString().ToLower() %>},
                            { id: "Author", title: "<%=FilesUCResource.ButtonSortAuthor%>", def: <%=(ContentOrderBy.SortedBy == SortedByType.Author).ToString().ToLower() %>, dsc: <%=(!ContentOrderBy.IsAsc).ToString().ToLower() %>}
                        ]
                })
                .bind("setfilter", ASC.Files.Filter.setFilter)
                .bind("resetfilter", ASC.Files.Filter.resetFilter);
        })(jQuery);
        
    </script>
</div>

<%-- Main Content Header --%>
<ul id="mainContentHeader" class="clearFix contentMenu">
    <li class="menuActionSelectAll">
        <div style="float:left; margin:1px 0 0 2px;">
            <input type="checkbox" id="files_selectAll_check" title="<%=FilesUCResource.MainHeaderSelectAll%>" />
        </div>
        <div id="menuActionSelectOpen" class="menuActionSelectOpen" title="<%=FilesUCResource.TitleSelectFile%>">
        </div>
    </li>
    <li id="files_mainDownload" class="menuAction menuActionDownload" title="<%=FilesUCResource.ButtonDownload%>">
        <span><%=FilesUCResource.ButtonDownload%></span>
    </li>
    <li id="files_mainMove" class="menuAction menuActionMove" title="<%=FilesUCResource.ButtonMoveTo%>">
        <span><%=FilesUCResource.ButtonMoveTo%></span>
    </li>
    <li id="files_mainCopy" class="menuAction menuActionCopy" title="<%=FilesUCResource.ButtonCopyTo%>">
        <span><%=FilesUCResource.ButtonCopyTo%></span>
    </li>
    <li id="files_mainMarkRead" class="menuAction menuActionMarkRead" title="<%=FilesUCResource.RemoveIsNew%>">
        <span><%=FilesUCResource.RemoveIsNew%></span>
    </li>
    <li id="files_mainUnsubscribe" class="menuAction menuActionUnsubscribe" title="<%=FilesUCResource.Unsubscribe%>">
        <span><%=FilesUCResource.Unsubscribe%></span>
    </li>
    <li id="files_mainRestore" class="menuAction menuActionRestore" title="<%=FilesUCResource.ButtonRestore%>">
        <span><%=FilesUCResource.ButtonRestore%></span>
    </li>
    <li id="files_mainDelete" class="menuAction menuActionDelete" title="<%=FilesUCResource.ButtonDelete%>">
        <span><%=FilesUCResource.ButtonDelete%></span>
    </li>
    <li id="files_mainEmptyTrash" class="menuAction menuActionEmptyTrash" title="<%=FilesUCResource.ButtonEmptyTrash%>">
        <span><%=FilesUCResource.ButtonEmptyTrash%></span>
    </li>
    
    <li id="switchViewFolder">
        <div id="switchToNormal" title="<%=FilesUCResource.SwitchViewToNormal%>">
            &nbsp;</div>
        <div id="switchToCompact" title="<%=FilesUCResource.SwitchViewToCompact%>">
            &nbsp;</div>
    </li>
    
    <li id="files_listUp" title="<%=FilesUCResource.ButtonUp%>">
        <span class="baseLinkAction"><%=FilesUCResource.ButtonUp%></span>
    </li>
</ul>
<div id="mainContentHeaderSpacer">&nbsp;</div>

<%-- Main Content --%>
<div id="mainContent">
    <ul id="files_mainContent" class='<%= CompactViewFolder ? "compact" : "" %>' >
    </ul>
    <div id="files_pageNavigatorHolder">
        <a class="grayLinkButton"></a>
    </div>
    <div id="emptyContainer">
        <asp:PlaceHolder runat="server" ID="EmptyScreenFolder"></asp:PlaceHolder>
    </div>
</div>
<%--tooltip--%>
<div id="entryTooltip"></div>
<%--popup window's--%>
<div id="files_selectorPanel" class="files_action_panel files_popup_win" style="margin-top: 1px;">
    <div class="popup-corner" style="left: 20px;">
    </div>
    <ul>
        <li id="files_selectAll"><a>
            <%=FilesUCResource.ButtonSelectAll%></a></li>
        <li id="files_selectFile"><a>
            <%=FilesUCResource.ButtonFilterFile%></a></li>
        <li id="files_selectFolder"><a>
            <%=FilesUCResource.ButtonFilterFolder%></a></li>
        <li id="files_selectDocument"><a>
            <%=FilesUCResource.ButtonFilterDocument%></a></li>
        <li id="files_selectPresentation"><a>
            <%=FilesUCResource.ButtonFilterPresentation%></a></li>
        <li id="files_selectSpreadsheet"><a>
            <%=FilesUCResource.ButtonFilterSpreadsheet%></a></li>
        <li id="files_selectImage"><a>
            <%=FilesUCResource.ButtonFilterImage%></a></li>
    </ul>
</div>
<div id="files_actionsPanel" class="files_action_panel files_popup_win withIcon" style="margin: 5px 0 0 -25px;">
    <div class="popup-corner" style="left: 20px;">
    </div>
    <ul>
        <li id="files_downloadButton" ><a>
            <%=FilesUCResource.ButtonDownload%>
            (<span></span>)</a> </li>
        <li id="files_unsubscribeButton"><a>
            <%=FilesUCResource.Unsubscribe%>
            (<span></span>)</a> </li>
        <li id="files_movetoButton"><a>
            <%=FilesUCResource.ButtonMoveTo%>
            (<span></span>)</a> </li>
        <li id="files_copytoButton"><a>
            <%=FilesUCResource.ButtonCopyTo%>
            (<span></span>)</a> </li>
        <li id="files_restoreButton"><a>
            <%=FilesUCResource.ButtonRestore%>
            (<span></span>)</a> </li>
        <li id="files_deleteButton"><a>
            <%=FilesUCResource.ButtonDelete%>
            (<span></span>)</a> </li>
        <li id="files_emptyTrashButton"><a>
            <%=FilesUCResource.ButtonEmptyTrash%></a> </li>
    </ul>
</div>
<div id="files_actionPanel" class="files_action_panel files_popup_win withIcon" >
    <div class="popup-corner" >
    </div>
    <ul id="files_actionPanel_files">
        <li id="files_open_files"><a>
            <%=FilesUCResource.OpenFile%></a> </li>
        <li id="files_edit_files"><a>
            <%=FilesUCResource.ButtonEdit%></a> </li>
        <li id="files_shareaccess_files"><a>
            <%=FilesUCResource.ButtonShareAccess%></a> </li>
        <li id="files_download_files"><a>
            <%=FilesUCResource.DownloadFile%></a> </li>
        <li id="files_unsubscribe_files"><a>
            <%=FilesUCResource.Unsubscribe%></a> </li>
        <li id="files_versions_files" ><a>
            <%=FilesUCResource.ButtonShowVersions%>(<span></span>)</a> </li>
        <li id="files_moveto_files"><a>
            <%=FilesUCResource.ButtonMoveTo%></a> </li>
        <li id="files_copyto_files"><a>
            <%=FilesUCResource.ButtonCopyTo%></a> </li>
        <li id="files_restore_files"><a>
            <%=FilesUCResource.ButtonRestore%></a> </li>
        <li id="files_rename_files"><a>
            <%=FilesUCResource.ButtonRename%></a> </li>
        <li id="files_remove_files"><a>
            <%=FilesUCResource.ButtonDelete%></a> </li>
    </ul>
    <ul id="files_actionPanel_folders">
        <li id="files_open_folders"><a>
            <%=FilesUCResource.OpenFolder%></a> </li>
        <li id="files_shareAccess_folders"><a>
            <%=FilesUCResource.ButtonShareAccess%></a> </li>
        <li id="files_download_folders"><a>
            <%=FilesUCResource.DownloadFolder%></a> </li>
        <li id="files_unsubscribe_folders"><a>
            <%=FilesUCResource.Unsubscribe%></a> </li>
        <li id="files_moveto_folders"><a>
            <%=FilesUCResource.ButtonMoveTo%></a> </li>
        <li id="files_copyto_folders"><a>
            <%=FilesUCResource.ButtonCopyTo%></a> </li>
        <li id="files_restore_folders"><a>
            <%=FilesUCResource.ButtonRestore%></a> </li>
        <li id="files_rename_folders"><a>
            <%=FilesUCResource.ButtonRename%></a> </li>
        <li id="files_remove_folders"><a>
            <%=FilesUCResource.ButtonDelete%></a> </li>
        <li id="files_change_thirdparty"><a>
            <%=FilesUCResource.ButtonChangeThirdParty%></a> </li>
        <li id="files_remove_thirdparty"><a>
            <%=FilesUCResource.ButtonDeleteThirdParty%></a> </li>
    </ul>
</div>

<%--dialog window--%>
<div id="files_confirm_remove" class="popupModal" style="display: none;">
    <ascw:container id="confirmRemoveDialog" runat="server">
        <header><%=FilesUCResource.ConfirmRemove%></header>
        <body>
            <div id="confirmRemoveText">
            </div>
            <div id="confirmRemoveList" class="files_remove_list">
                <dl>
                    <dt class="confirmRemoveFolders">
                        <%=FilesUCResource.Folders%>:</dt>
                    <dd class="confirmRemoveFolders">
                    </dd>
                    <dt class="confirmRemoveFiles">
                        <%=FilesUCResource.Documents%>:</dt>
                    <dd class="confirmRemoveFiles">
                    </dd>
                </dl>
            </div>
            <span id="confirmRemoveTextDescription" class="textMediumDescribe clearFix" style="padding: 10px 0 0;">
                <%=FilesUCResource.ConfirmRemoveDescription%>
            </span>
            <div class="clearFix" style="padding-top: 16px">
                <a id="removeConfirmBtn" class="baseLinkButton" style="float: left;">
                    <%=FilesUCResource.ButtonOk%>
                </a><a class="grayLinkButton" href="" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;"
                    style="float: left; margin-left: 8px;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </body>
    </ascw:container>
</div>
<div id="files_overwriteFiles" class="popupModal" style="display: none;">
    <ascw:container id="confirmOverwriteDialog" runat="server">
        <header><%=FilesUCResource.ConfirmOverwrite%></header>
        <body>
            <div id="files_overwrite_msg" style="overflow: hidden;">
            </div>
            <ul id="files_overwrite_list" >
            </ul>
            <div class="clearFix" style="margin-top: 15px;">
                <a id="files_overwrite_overwrite" class="baseLinkButton" style="float: left;">
                    <%=FilesUCResource.ButtonRewrite%>
                </a>
                <a id="files_overwrite_skip" class="grayLinkButton" style="float: left; margin-left: 8px;">
                    <%=FilesUCResource.ButtonSkip%>
                </a>
                <a id="files_overwrite_cancel" class="grayLinkButton" style="float: left; margin-left: 8px;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </body>
    </ascw:container>
</div>
<%-- progress --%>
<div id="files_bottom_loader">
    <div id="files_progress_template" class="files_progress_box">
        <div class="headerBaseMedium">
        </div>
        <div class="progress_wrapper">
            <div class="progress">
            </div>
            <span class="percent">0</span>
        </div>
        <span class="textSmallDescribe"></span>
    </div>
</div>
<asp:PlaceHolder runat="server" ID="CommonContainer" />