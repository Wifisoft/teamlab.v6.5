using System;
using System.IO;
using System.Text;
using System.Web.UI;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class BookmarkAddedByUserContorl : System.Web.UI.UserControl
	{

		public const string Location = "~/Products/Community/Modules/Bookmarking/Controls/BookmarkAddedByUserContorl.ascx";

		public bool TintFlag { get; set; }

		public string UserImage { get; set; }

		public string UserPageLink { get; set; }

		public string UserBookmarkDescription { get; set; }

		public string DateAddedAsString { get; set; }

		public string DivID { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public string GetAddedByTableItem(bool TintFlag, string UserImage, string UserPageLink,
										 string UserBookmarkDescription, string DateAddedAsString, object DivID) 
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					using (var c = LoadControl(Location) as BookmarkAddedByUserContorl)
					{
						c.DivID = DivID.ToString();
						c.TintFlag = TintFlag;
						c.UserImage = UserImage;
						c.UserPageLink = UserPageLink;
						c.UserBookmarkDescription = UserBookmarkDescription;
						c.DateAddedAsString = DateAddedAsString;
						c.RenderControl(textWriter);
					}
				}
			}
			return sb.ToString();
		}		
	}
}