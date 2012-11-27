<%@ Assembly Name="ASC.Web.Community" %>
<%@ Assembly Name="ASC.Web.Community.PhotoManager" %>
<%@ Assembly Name="ASC.PhotoManager" %>
<%@ Assembly Name="ASC.Core.Common" %>

<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true" CodeBehind="PhotoDetails.aspx.cs" Inherits="ASC.Web.Community.PhotoManager.PhotoDetails" Title="Untitled Page" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.PhotoManager.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Community.PhotoManager" %>

<%@ Register Assembly="ASC.Web.Controls" Namespace="ASC.Web.Controls" TagPrefix="ascw" %>
<%@ Register Src="Controls/PhotoList.ascx" TagName="PhotoList" TagPrefix="ctrl" %>
<%@ Register Src="Controls/LastEvents.ascx" TagName="LastEvents" TagPrefix="ctrl" %>
<%@ Register Src="Controls/ActionContainer.ascx" TagName="ActionContainer" TagPrefix="ctrl" %>
<%@ Register Assembly="ASC.Web.Studio" Namespace="ASC.Web.Studio.Controls.Common" TagPrefix="ascwc" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="PhotoPageHeader" ContentPlaceHolderID="CommunityPageHeader" runat="server">
  
</asp:Content>

<asp:Content ID="PhotoPageContent" ContentPlaceHolderID="CommunityPageContent" runat="server">
  <ascw:container id="mainContainer" runat="server">
    <Header></Header>
    <Body>
      <asp:Panel ID="pnlContent" runat="server">
        <table id="centralContainer">
          <tr>
            <td align="center">
              <table id="mainContentWrapper">
                <tr>
                  <td style="width:1%;">
                    <div class="leftSide">
                      <a id="prevImage" href="#" onclick="photoDetails.prevImage(); return false;"><%=PhotoManagerResource.PreviousPhotoButton%></a>
                    </div>
                  </td>
                  <td align="center" style="width:98%;">
                    <table id="photo_image_container" class="tablePhoto" style="visibility:hidden;">
                      <tr class="topBorder">
                        <td class="leftSide"></td>
                        <td class="center"></td>
                        <td class="rightSide"></td>
                      </tr>
                      <tr class="container">
                        <td class="leftSide"></td>
                        <td class="center container">
                          <div class="container">
                            <div class="imageLoader"></div>
                            <a id="hrefContainer" href="#" target="_blank">
                              <img src="<%=WebImageSupplier.GetAbsoluteWebPath("blank.gif")%>" alt="" />
                            </a>
                          </div>
                        </td>
                        <td class="rightSide"></td>
                      </tr>
                      <tr class="bottomBorder">
                        <td class="leftSide"></td>
                        <td class="center"></td>
                        <td class="rightSide"></td>
                      </tr>
                    </table>
                  </td>
                  <td style="width:1%;">
                    <div class="rightSide">
                      <a id="nextImage" href="#" onclick="photoDetails.nextImage(); return false;"><%=PhotoManagerResource.NextPhotoButton%></a>
                    </div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td align="center">
              <table id="subMenu" style="visibility:hidden;">
                <tr>
                  <td>
                    <ul>
                    <% if (image != null && ASC.Web.Community.Product.CommunitySecurity.CheckPermissions(image, ASC.PhotoManager.PhotoConst.Action_EditRemovePhoto))
                       { %>
                      <li>
                        <div id="editImage" class="button" onclick="photoDetails.editImage();" href="<%=RenderEditPhotoLink()%>"><%=PhotoManagerResource.EditSmallButton%></div>
                      </li>
                      <li>
                        <div id="removeImage" class="button" onclick="photoDetails.removeImage();"><%=PhotoManagerResource.DeleteButton%></div>
                      </li>
                      <% } %>
                      <li>
                        <div id="rotateLeftImage" class="button" onclick="photoDetails.rotateImage(true);"><%=PhotoManagerResource.RotateLeftLink%></div>
                      </li>
                      <li>
                        <div id="rotateRightImage" class="button" onclick="photoDetails.rotateImage(false);"><%=PhotoManagerResource.RotateRightLink%></div>
                      </li>
                      <li id="SlideShowButton" runat="server">
                        <a id="startSlideShow" class="button" href="<%=RenderSlideShowLink()%>" target="_blank"><%=PhotoManagerResource.SlidesShowTooTip%></a>
                      </li>
                    </ul>
                  </td>
                  <td width="35%">
                    <div id="subInfo" class="textBigDescribe">
                      <span id="viewsCount"></span><span class="splitter">|</span><span id="commentsCount"></span>
                    </div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td align="center">
              <table id="thumbsBar">
                <tr>
                  <td class="leftSide" width="1%">
                    <div id="prevThumb" onclick="photoDetails.shiftToLeft();"></div>
                  </td>
                  <td class="center container" align="center" valign="middle" width="98%">
                    <div id="photoThumbnails" style="display:none;">
                      <asp:Repeater ID="PhotoThumbnails" runat="server">
                        <ItemTemplate>
                          <a id="thumbnail-<%#(Container.DataItem as Dictionary<String, String>)["Id"]%>" class="thumb" href="#<%#(Container.DataItem as Dictionary<String, String>)["Id"]%>" onclick="photoDetails.changeImage(this); return false;">
                            <div class="selector"></div>
                            <img src="<%=WebImageSupplier.GetAbsoluteWebPath("blank.gif")%>" truesrc="<%#(Container.DataItem as Dictionary<String, String>)["Src"]%>" alt="<%#(Container.DataItem as Dictionary<String, String>)["Name"]%>" title="<%#(Container.DataItem as Dictionary<String, String>)["Name"]%>" />
                          </a>
                        </ItemTemplate>
                      </asp:Repeater>
                    </div>
                  </td>
                  <td class="rightSide" width="1%">
                    <div id="nextThumb" onclick="photoDetails.shiftToRight();"></div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>

        <div id="hiddenContainer" style="display:none;">
          <asp:HiddenField ID="hdnImageID" runat="server" />
          <asp:HiddenField ID="hdnPhotoName" runat="server" />
          <asp:HiddenField ID="hdnAlbumID" runat="server" />
          <asp:HiddenField ID="hdnDefaultImageID" runat="server" />
        </div>

        <%-- commments --%>
        <div id="commentsContainer"></div>
      </asp:Panel>
      <asp:Literal ID="ltrMessage" runat="server" />
    </Body>
  </ascw:container>
</asp:Content>

<%-- side panel --%>
<asp:Content ID="PhotoPageSidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
  <ctrl:ActionContainer ID="actions" runat="server" />
  <ascwc:SideRecentActivity id="sideRecentActivity" runat="server" />
  <ascwc:SideContainer ID="albumsContainer" runat="server">
    <asp:Literal ID="ltrAlbums" runat="server" />
  </ascwc:SideContainer>
  <ctrl:LastEvents ID="lastEvents" runat="server" />
</asp:Content>
