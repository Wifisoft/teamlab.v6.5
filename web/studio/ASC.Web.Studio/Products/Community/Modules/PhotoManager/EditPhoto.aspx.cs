using System;
using System.Collections.Generic;
using System.Text;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Core;
using System.Web.UI;
using System.Web;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager
{
	public partial class EditPhoto : BasePage
	{
		#region Members

		private int countItems;
		private Album albumForPublisher = null;
		ASC.Data.Storage.IDataStore store = ASC.Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

		#endregion

		#region Methods

		protected override void PageLoad()
		{
			if (SetupInfo.WorkMode == WorkMode.Promo)
				Response.Redirect(ASC.PhotoManager.PhotoConst.PAGE_DEFAULT, true);

			btnSave.Text = PhotoManagerResource.SaveButton;

			mainContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = ASC.PhotoManager.PhotoConst.PAGE_DEFAULT });
			mainContainer.BreadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = PhotoManagerResource.EditPhotoTitle });
			this.Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, mainContainer.BreadCrumbs);

			if (Session[ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS] == null && !String.IsNullOrEmpty(this.PhotoID))
				Session.Add(ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS, new List<String> { this.PhotoID });

			/*if (Session[ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS] == null)
				Response.Redirect(ASC.PhotoManager.PhotoConst.PAGE_PHOTO);*/

			countItems = 0;

			if (!IsPostBack)
			{
				var storage = StorageFactory.GetStorage();
				var selectedItems = new List<string>();
                if (Session[ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS] != null)
                {
                    selectedItems = (List<string>)Session[ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS];
                    Session[ASC.PhotoManager.PhotoConst.PARAM_SELECTED_ITEMS] = null;
                }

				var sb = new StringBuilder();

				// edit one photo
				if (!string.IsNullOrEmpty(this.PhotoID))
				{
					var image = storage.GetAlbumItem(Convert.ToInt64(this.PhotoID));
					var album = image.Album;

					if (selectedItems.Contains(image.Id.ToString()))
						sb.Append(AddAlbumItem(image, album.FaceItem.Id == image.Id));

				}
				// edit albums photos
				else if (selectedItems.Count > 0)
				{
					var albumId = Convert.ToInt64(this.AlbumID);
					var album = storage.GetAlbum(albumId);
					var images = storage.GetAlbumItems(album);

					if (0 < images.Count)
					{
						foreach (var item in images)
						{
							if (selectedItems.Contains(item.Id.ToString())) sb.Append(AddAlbumItem(item, album.FaceItem.Id == item.Id));
						}
					}
				}

				sb.Append("</div>");

				this.imageListHolder.Controls.Add(new LiteralControl(sb.ToString()));

				sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
				sideRecentActivity.ProductId = Product.CommunityProduct.ID;
				sideRecentActivity.ModuleId = ASC.PhotoManager.PhotoConst.ModuleID;
			}

		}

		private void UpdateImage(int index)
		{
			var storage = StorageFactory.GetStorage();
			AlbumItem image = storage.GetAlbumItem(Convert.ToInt64(Request.Form[ASC.PhotoManager.PhotoConst.PARAM_EDIT_ITEMID + index]));

			image.Name = GetLimitedText(Request.Form[ASC.PhotoManager.PhotoConst.PARAM_EDIT_NAME + index]);

			if (albumForPublisher == null)
				albumForPublisher = image.Album;

			storage.SaveAlbumItem(image);
		}

		private string AddAlbumItem(AlbumItem albumItem, bool isFace)
		{
			StringBuilder sb = new StringBuilder();

			if (countItems != 0 && countItems % 3 == 0)
				sb.Append("</div>");

			if (countItems % 3 == 0)
				sb.Append("<div class=\"borderLight tintMediumLight clearFix\" style=\"padding:20px; border-left:none;border-right:none;margin-bottom:8px;\">");


			sb.Append("<div style='float:left;margin-bottom:5px;" + (countItems % 3 == 0 ? "" : "margin-left:22px; ") + "'>");

			sb.Append("<span>");
			sb.Append("<input name=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_ITEMID + countItems + "\" name=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_ITEMID + countItems + "\" type=\"hidden\" value=\"" + albumItem.Id + "\" />");
			sb.Append("<div style=\"width: 200px;height:200px;background-color: #EBF0F4;padding:5px;\">" + ImageHTMLHelper.GetHTMLThumb(ImageHTMLHelper.GetImageUrl(albumItem.ExpandedStorePreview, store), 200, albumItem.PreviewSize.Width, albumItem.PreviewSize.Height) + "</div>");

            sb.Append("<div class='textBigDescribe clearFix' style=\" padding-left:5px;padding-top:5px;width:200px;\">");
			sb.Append("<div style=\"padding-top: 5px; float: left;\">" + PhotoManagerResource.EditPhotoNameTitle + "</div>");

			sb.Append("<div style=\"float: right;\">");
			sb.Append("<label class='textMediumDescribe' for=\"face_" + countItems + "\">");
			sb.Append(PhotoManagerResource.AlbumCoverTitle + "</label><input type=\"radio\" id=\"face_" + countItems + "\" name=\"album_face\" value=\"" + albumItem.Id + "\" " + (isFace ? "checked" : "") + " /></div></div>");

			sb.Append("<div><input class=\"textEdit\" style=\"width:200px;margin-top:5px;\" maxlength=\"255\" name=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_NAME + countItems + "\" id=\"" + ASC.PhotoManager.PhotoConst.PARAM_EDIT_NAME + countItems + "\" value='" + HttpUtility.HtmlEncode(albumItem.Name) + "' type=\"text\"/></div>");
			sb.Append("</span>");

			sb.Append("</div>");

			countItems++;

			return sb.ToString();
		}

		#endregion

		#region Events

		protected void btnSave_Click(object sender, EventArgs e)
		{
			int i = 0;

			while (Request.Form[ASC.PhotoManager.PhotoConst.PARAM_EDIT_ITEMID + i] != null)
			{
				UpdateImage(i);
				i++;
			}

			long albumItemId = 0;
			if (!string.IsNullOrEmpty(Request.Form["album_face"]) && long.TryParse(Request.Form["album_face"], out albumItemId))
			{
				var storage = StorageFactory.GetStorage();
				var item = storage.GetAlbumItem(albumItemId);
				albumForPublisher.FaceItem = item;
				storage.SaveAlbum(albumForPublisher);
			}

			PhotoUserActivityPublisher.EditPhoto(albumForPublisher, SecurityContext.CurrentAccount.ID);

			if (i > 1)
			{
				Response.Redirect(ASC.PhotoManager.PhotoConst.PAGE_PHOTO + "?" + ASC.PhotoManager.PhotoConst.PARAM_ALBUM + "=" + Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM]);
			}
			else
			{
				Response.Redirect(ASC.PhotoManager.PhotoConst.PAGE_PHOTO_DETAILS + "?" + ASC.PhotoManager.PhotoConst.PARAM_PHOTO + "=" + Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_PHOTO]);
			}
		}

		#endregion
	}
}