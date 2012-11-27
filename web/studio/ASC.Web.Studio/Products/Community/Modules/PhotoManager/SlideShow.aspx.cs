using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager
{
  [AjaxPro.AjaxNamespace("SlideShow")]
	public partial class SlideShow : BasePage
	{
		#region Members

    protected long AlbumId { get { return image.Album.Id; } }
    protected int ImageIndex { get { return 0; } }
		protected int ImageMaxHeight { get { return 690; } }
		//private int max_width = 933;

		private AlbumItem image = null;

		#endregion

		#region Methods

    protected override void PageLoad()
    {
      AjaxPro.Utility.RegisterTypeForAjax(typeof(SlideShow), this.Page);

      this.Page.Header.Controls.Add(new Literal() { Text = "<link type=\"text/css\" href=\"" + ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/css/slideshowstyle.css") + "\" rel=\"stylesheet\" />" });
      this.Page.Header.Controls.Add(new Literal() { Text = "<script type=\"text/javascript\" src=\"" + ASC.Data.Storage.WebPath.GetPath("js/auto/jquery_full.js") + "\"></script>" });
      this.Page.Header.Controls.Add(new Literal() { Text = "<script type=\"text/javascript\">var jq = jQuery.noConflict();</script>" });
      this.Page.Header.Controls.Add(new Literal() { Text = "<script type=\"text/javascript\" src=\"" + ASC.Data.Storage.WebPath.GetPath("products/community/modules/photomanager/js/slideshow.js") + "\" type=\"text/javascript\"></script>" });

      var breadCrumbs = new List<BreadCrumb>();
      breadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = ASC.PhotoManager.PhotoConst.PAGE_DEFAULT });
      breadCrumbs.Add(new ASC.Web.Controls.BreadCrumb() { Caption = PhotoManagerResource.PageTitleSlideShow });

      this.Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, breadCrumbs);

      try
      {
        LoadImage(Convert.ToInt64(Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM]), 0);
      }
      catch
      {
      }
    }

		private int LoadImage(long albumID, int index)
		{
			var storage = StorageFactory.GetStorage();
			Album album = storage.GetAlbum(albumID);

			if (album.ImagesCount == 0)
			{
				return 0;
			}

			if (index < 0)
			{
				index = album.ImagesCount - 1;
			}
			if (album.ImagesCount <= index)
			{
				index = 0;
			}

			if (album.ImagesCount > index)
			{
				image = storage.GetAlbumItem(album, index);
			}
			return index;
		}

    protected string RenderAlbumLink()
    {
      return ASC.PhotoManager.PhotoConst.PAGE_PHOTO + "?" + ASC.PhotoManager.PhotoConst.PARAM_ALBUM + "=" + Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM];
    }

    protected string RenderAlbumTitle()
    {
      return HttpUtility.HtmlEncode(image.Album.Event.Name);
    }

    protected string RenderAlbumDate()
    {
      return image.Album.Event.Timestamp.ToString("d MMM yyy");
    }

    protected string RenderImageTitle()
    {
      return HttpUtility.HtmlEncode(image.Name);
    }

    protected string RenderImagePath()
    {
      return ImageHTMLHelper.GetImageUrl(image.ExpandedStorePreview, ASC.Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo"));
    }

		#endregion

		#region AJAX methods

    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse GetImage(long albumID, int index)
		{
      return new AjaxResponse()
      {
        rs1 = LoadImage(albumID, index).ToString(),
			  rs2 = ImageHTMLHelper.GetImageUrl(image.ExpandedStorePreview, ASC.Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo")),
			  rs3 = image.Name
      };
		}

		#endregion

		#region IContextInitializer Members

		public void InitializeContext(HttpContext context)
		{
		}

		#endregion
	}
}
