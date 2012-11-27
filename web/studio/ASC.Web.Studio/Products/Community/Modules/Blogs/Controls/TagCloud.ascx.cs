using System;
using System.Collections.Generic;
using System.Web;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Resources;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Community.Blogs.Controls
{
	public partial class TagCloud : BaseUserControl
	{
		public string UserID
		{
			get;
			set;
		}

		public void SetTags(List<TagStat> tags)
		{
			if (tags != null && tags.Count != 0)
			{
				TabCloudContainer.Title = BlogsResource.TagCloudTitle;
				TabCloudContainer.HeaderCSSClass = "studioSideBoxTagCloudHeader";
				TabCloudContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("tagcloud.png");

				ASC.Web.Controls.TagCloud tagCloud = new ASC.Web.Controls.TagCloud();
				tagCloud.BoxCSSClass = "tagCloudBox";
				tagCloud.LinkCSSClass = "tagCloudItem";

				tags.Sort((t1, t2) => String.Compare(t1.Name, t2.Name, true));

				foreach (TagStat tag in tags)
				{
					ASC.Web.Controls.TagCloudItem item = new ASC.Web.Controls.TagCloudItem();
					item.TagName = tag.Name;
					item.Rank = tag.Count;
					item.URL = "./?tagname=" + HttpUtility.UrlEncode(tag.Name);

					if (!string.IsNullOrEmpty(UserID))
					{
						item.URL += "&userID=" + UserID;
					}

					tagCloud.Items.Add(item);
				}

				tagCloudHolder.Controls.Add(tagCloud);
			}
			else
			{
				this.Visible = false;
			}
		}
	}
}