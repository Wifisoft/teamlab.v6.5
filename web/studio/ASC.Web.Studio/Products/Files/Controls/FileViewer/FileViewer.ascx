<%@ Assembly Name="ASC.Web.Files" %>
<%@ Assembly Name="ASC.Web.Controls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileViewer.ascx.cs"
    Inherits="ASC.Web.Files.Controls.FileViewer" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Configuration" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<% #if (DEBUG) %>
    <link href="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/FileViewer/fileviewer.css")%>" type="text/css" rel="stylesheet" />
<% #endif %>

<script type="text/javascript" language="javascript" src="<%= String.Concat(PathProvider.BaseAbsolutePath, "Controls/FileViewer/fileviewer.js") %>"></script>

<div id="other_actions" style="display:none;">
    <ul>
        <% if(!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
           {%>
        <li onclick="ASC.Files.ImageViewer.downloadImage()"><a class="action_download" href="javascript:void(0)"
            title="<%=FilesUCResource.ButtonDownload%>">
            <%=FilesUCResource.ButtonDownload%></a> </li>
        <% } %>
        <li onclick="ASC.Files.ImageViewer.deleteImage()"><a class="action_delete" href="javascript:void(0)"
            title="<%= FilesUCResource.ButtonDelete %>">
            <%= FilesUCResource.ButtonDelete %></a> </li>
    </ul>
</div>
<img id="imageViewerContainer" width="10px" height="10px" style="display: none;" />
<div id="imageViewerToolbox" style="display: none;">
    <div class="imageInfo textBigDescribe">
        &nbsp;
    </div>
    <div class="toolboxWrapper">
        <ul>
            <li onclick="ASC.Files.ImageViewer.zoomIn()">
                <img width="24" alt="<%= FilesUCResource.ButtonZoomIn %>" title="<%= FilesUCResource.ButtonZoomIn %>"
                    height="24" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_zoom_in.png", ProductEntryPoint.ID) %>" />
            </li>
            <li onclick="ASC.Files.ImageViewer.zoomOut()">
                <img alt="<%= FilesUCResource.ButtonZoomOut %>" title="<%= FilesUCResource.ButtonZoomOut %>"
                    width="24" height="24" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_zoom_out.png", ProductEntryPoint.ID) %>" />
            </li>
            <li onclick="ASC.Files.ImageViewer.fullScale()" style="margin-right: 40px;">
                <img width="24" height="24" alt="<%= FilesUCResource.ButtonFullScale %>" title="<%= FilesUCResource.ButtonFullScale %>"
                    src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_fullScale_enabled.png", ProductEntryPoint.ID) %>" />
            </li>
            <li onclick="ASC.Files.ImageViewer.prevImage()">
                <img width="24" height="24" alt="<%= FilesUCResource.ButtonPrevImg %>" title="<%= FilesUCResource.ButtonPrevImg %>"
                    src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_prev.png", ProductEntryPoint.ID) %>" />
            </li>
            <%--<img src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_slideshow.png", ProductEntryPoint.ID)  %>" />--%>
            <li onclick="ASC.Files.ImageViewer.nextImage()" style="margin-right: 40px;">
                <img alt="<%= FilesUCResource.ButtonNextImg %>" title="<%= FilesUCResource.ButtonNextImg %>"
                    width="24" height="24" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_next.png", ProductEntryPoint.ID) %>" />
            </li>
            <li onclick="ASC.Files.ImageViewer.rotateLeft()">
                <img alt="<%= FilesUCResource.ButtonRotateLeft %>" title="<%= FilesUCResource.ButtonRotateLeft %>"
                    width="24" height="24" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_rotate_left.png", ProductEntryPoint.ID) %>" />
            </li>
            <li onclick="ASC.Files.ImageViewer.rotateRight()">
                <img alt="<%= FilesUCResource.ButtonRotateRight %>" title="<%= FilesUCResource.ButtonRotateRight %>"
                    width="24" height="24" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_rotate_right.png", ProductEntryPoint.ID) %>" />
            </li>
            <li id="other_actions_switch" >
                <img alt="" title="<%= FilesUCResource.ButtonOtherAction %>" width="24" height="24"
                    src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_combobox.png", ProductEntryPoint.ID) %>" />
            </li>
        </ul>
    </div>
</div>
<div id="imageViewerInfo" style="display: none;">
    <span>100%</span>
</div>
<img id="imageViewerClose" width="15px" height="15px" src="<%=WebImageSupplier.GetAbsoluteWebPath("iv_close.png", ProductEntryPoint.ID) %>"
    style="display: none;" onclick="ASC.Files.ImageViewer.closeImageViewer();return false;"
    alt="<%= FilesUCResource.ButtonClose %>" title="<%= FilesUCResource.ButtonClose %>" />
<div id="imageBatchLoader" style="display: none;">
</div>