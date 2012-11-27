using System.Collections.Generic;
using AjaxPro;
using ASC.Bookmarking.Pojo;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking
{
	[AjaxNamespace("SingleBookmarkUserControl")]
	public partial class SingleBookmarkUserControl : BookmarkInfoBase
	{

		/// <summary>
		/// Init tags and added by list
		/// </summary>
		public override void InitUserControl()
		{
			if (Bookmark != null)
			{
				var tags = Bookmark.Tags;
				if (tags != null && tags.Count > 0)
				{
					InitTagsRepeater(tags);
				}
			}			
		}

		private void InitTagsRepeater(IList<Tag> tags)
		{
			if (tags != null && tags.Count > 0)
			{
				TagsRepeater.DataSource = tags;
				TagsRepeater.DataBind();

				UserTagsString = BookmarkingServiceHelper.ConvertTagsToString(tags);
			}
		}

	}
}