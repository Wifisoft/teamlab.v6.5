using System;
using System.Globalization;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Code.Module;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News.Controls
{
	[AjaxNamespace("FeedItem")]
	public partial class FeedItem : UserControl
	{
		public Feed Feed { get; set; }
		public bool IsEditVisible { get; set; }
		public string FeedLink { get; set; }
		public Uri RemoveUrlWithParam { get; set; }
		public Uri EditUrlWithParam { get; set; }
		public string FeedType { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime ExpirationTime { get; set; }
		public int PollVotes { get; set; }

		protected Guid RequestedUserId
		{
			get
			{
				Guid result = Guid.Empty;
				try
				{
					result = new Guid(Request["uid"]);
				}
				catch { }

				return result;
			}
		}

		protected string UserIdAttribute
		{
			get
			{
				if (!RequestedUserId.Equals(Guid.Empty))
				{
					return string.Format(CultureInfo.CurrentCulture, "?uid={0}", RequestedUserId);
				}
				return string.Empty;
			}

		}


		protected void Page_Load(object sender, EventArgs e)
		{
			Utility.RegisterTypeForAjax(typeof(FeedItem), this.Page);
		}

		public override void DataBind()
		{
			base.DataBind();

			if (Feed != null)
			{
				Date.Text = Feed.Date.ToShortDateString();
				NewsLink.NavigateUrl = FeedLink;
				NewsLink.Text = Feed.Caption.HtmlEncode();
				if (!Feed.Readed)
				{
                    NewsLink.Style.Value = "font-weight:bold;width: 390px; display: block;";
				}

				string logopath = "";
				logopath = WebImageSupplier.GetAbsoluteWebPath(FeedTypeInfo.FromFeedType(Feed.FeedType).TypeLogoPath, NewsConst.ModuleId);
				Type.Text = string.Format(@"<img src=""{0}"" class=""NewsImgStyle"" border=""0"" alt=""{1}"" />", logopath, "");
				profileLink.Text = CoreContext.UserManager.GetUsers(new Guid(Feed.Creator)).RenderProfileLink(ASC.Web.Community.Product.CommunityProduct.ID);
			}
		}

		private string GetEditUrl()
		{
			return (Feed is FeedPoll ? FeedUrls.EditPollUrl : FeedUrls.EditNewsUrl) + UserIdAttribute;
		}

		[AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse Remove(string id)
		{
			AjaxResponse resp = new AjaxResponse();
			resp.rs1 = "0";
			if (!string.IsNullOrEmpty(id))
			{
                CommunitySecurity.DemandPermissions(NewsConst.Action_Edit);

				var storage = FeedStorageFactory.Create();
				ActivityPublisher.DeletePost(storage.GetFeed(Convert.ToInt64(id)), SecurityContext.CurrentAccount.ID);
				storage.RemoveFeed(Convert.ToInt64(id, CultureInfo.CurrentCulture));

                CommonControlsConfigurer.FCKUploadsRemoveForItem("news", id);

				resp.rs1 = id;
				resp.rs2 = NewsResource.FeedDeleted;
			}
			return resp;
		}
	}
}