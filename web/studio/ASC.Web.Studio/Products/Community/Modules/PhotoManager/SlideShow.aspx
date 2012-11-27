<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>

<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="SlideShow.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.SlideShow" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Untitled Page</title>
</head>
<body>
  <form id="SlideShowForm" style="margin: 0px; padding: 0px;" enctype="multipart/form-data" method="post" runat="server">
    <input id="imgAlbumId" type="hidden" value="<%=AlbumId%>" />
    <input id="imgMaxHeight" type="hidden" value="<%=ImageMaxHeight%>" />
    <input id="imgStartIndex" type="hidden" value="<%=ImageIndex%>" />
    <table cellpadding="0" cellspacing="0" border="0" class="SlideShow_Slider">
      <tr>
        <td id="image_container" valign="middle" align="center" colspan="3" style="table-layout: fixed" class="SlideShow_Picture">
          <img id="imageContainer" src="<%=WebImageSupplier.GetAbsoluteWebPath("blank.gif")%>" alt="" />
        </td>
      </tr>
      <tr>
        <td class="SlideShow_PanelInfo SlideShow_LeftAlign">
          <a class="SlideShow_Link" href="<%=RenderAlbumLink()%>"><%=RenderAlbumTitle()%></a>
          <span class="SlideShow_AlbumInfo" id="eventInfo" ><%=RenderAlbumDate()%></span>
        </td>
        <td class="SlideShow_PanelInfo SlideShow_CenterAlign">
          <span class="SlideShow_Title" ID="photoName" ></span>
        </td>
        <td class="SlideShow_PanelInfo SlideShow_RightAlign">&nbsp;</td>
      </tr>
      <tr>
        <td class="SlideShow_PanelControls SlideShow_LeftAlign">&nbsp;</td>
        <td class="SlideShow_PanelControls SlideShow_CenterAlign">
          <span id="prevImg" class="button" onclick="slideShowManager.prevImage();" title="<%=PhotoManagerResource.PrevSlideToolTip%>"></span>
          <span id="runSlideShow" class="button stoped" onclick="slideShowManager.toggle();" title="<%=PhotoManagerResource.RunPauseToolTip%>"></span>
          <span id="nextImg" class="button" onclick="slideShowManager.nextImage();" title="<%=PhotoManagerResource.NextSlideToolTip%>"></span>
        </td>
        <td class="SlideShow_PanelControls SlideShow_RightAlign">
          <span id="closeSlideShow" class="button" onclick="slideShowManager.close();" title="<%=PhotoManagerResource.ExitToolTip%>"></span>
        </td>
      </tr>
    </table>
  </form>
</body>
</html>
