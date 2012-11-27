using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Data.Storage;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Web.Studio.Utility;
using System.Drawing;

namespace ASC.Web.Studio.UserControls.Users
{
	public class ThumbnailItem
	{
		public string id { get; set; }
		public Size size { get; set; }
		public string imgUrl { get; set; }
		public Bitmap bitmap { get; set; }
	}

	public interface IThumbnailsData
	{
		Guid UserID { get; set; }
		string MainImgUrl { get; }
		Bitmap MainImgBitmap { get; }
		List<ThumbnailItem> ThumbnailList { get; }
		void Save(List<ThumbnailItem> bitmaps);
	}

	[AjaxNamespace("ThumbnailEditor")]
	public partial class ThumbnailEditor : System.Web.UI.UserControl
	{
		#region Propertie

		public static string Location { get { return "~/UserControls/Users/ThumbnailEditor/ThumbnailEditor.ascx"; } }

		public Type SaveFunctionType { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string BehaviorID { get; set; }

		public System.Drawing.Size JcropMinSize { get; set; }

		public System.Drawing.Size JcropMaxSize { get; set; }

		public double JcropAspectRatio { get; set; }

		#endregion
		#region Members

		protected string MainImgUrl { get { return GetThumbnailsData(SaveFunctionType).MainImgUrl; } }

		private string _jsObjName { get { return String.IsNullOrEmpty(this.BehaviorID) ? "__thumbnailEditor" + this.UniqueID : this.BehaviorID; } }
		protected string _selectorID = Guid.NewGuid().ToString().Replace('-', '_');

		#endregion
		#region Events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_container.Options.IsPopup = true;

			if (SaveFunctionType == null)
				return;

			var SaveClass = GetThumbnailsData(SaveFunctionType);

			const string ThumbnailEditorJavaScript = "studio_thumbnaileditor_script";
			if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), ThumbnailEditorJavaScript))
			{
				Page.ClientScript.RegisterClientScriptInclude(this.GetType(), ThumbnailEditorJavaScript, WebPath.GetPath("usercontrols/users/thumbnaileditor/js/thumbnaileditor.js"));
			}

			const string jqueryJcropJavaScript = "jquery_jcrop_script";
			if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), jqueryJcropJavaScript))
			{
				Page.ClientScript.RegisterClientScriptInclude(this.GetType(), jqueryJcropJavaScript, WebPath.GetPath("usercontrols/users/thumbnaileditor/js/jquery.Jcrop.js"));
			}

			const string ThumbnailEditorCssStyle = "studio_thumbnaileditor_style";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), ThumbnailEditorCssStyle))
			{
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), ThumbnailEditorCssStyle, "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/thumbnaileditor/css/<theme_folder>/thumbnaileditor.css") + "\">", false);
			}

			StringBuilder script = new StringBuilder();
			script.Append("var " + _jsObjName + " = new ASC.Studio.ThumbnailEditor.ThumbnailEditorPrototype('" + _selectorID + "','" + _jsObjName + "'); ");

			StringBuilder sb = new StringBuilder();
			sb.Append("<table width='100%'><tr>");
			foreach (var item in SaveClass.ThumbnailList)
			{
				sb.AppendFormat(@"<td valign='top'><div class='thumbnailImg' style='height:{0}px; width:{1}px'>
													<img id='preview_{3}' src='{2}'/>
												</div></td>",
											item.size.Height,
											item.size.Width,
											item.imgUrl,
											_selectorID);

				script.AppendFormat(" {0}.ThumbnailItems.push(new ASC.Studio.ThumbnailEditor.ThumbnailItem({1}, {2}, '{3}')); ",
											_jsObjName,
											item.size.Height,
											item.size.Width,
											item.imgUrl);
			}
			sb.Append("</tr></table>");

			placeThumbnails.Controls.Add(new Literal() { Text = sb.ToString() });

			script.AppendFormat(" {0}.JcropMinSize = [ {1}, {2} ]; ", _jsObjName, JcropMinSize.Width, JcropMinSize.Height);

			script.AppendFormat(" {0}.JcropMaxSize = [ {1}, {2} ]; ", _jsObjName, JcropMaxSize.Width, JcropMaxSize.Height);

			script.AppendFormat(" {0}.JcropAspectRatio = {1}; ", _jsObjName, JcropAspectRatio);

			script.AppendFormat(" {0}.SaveThumbnailsFunction = '{1}'; ", _jsObjName, SaveFunctionType.FullName);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
		}

		#endregion
		#region Methods

		public ThumbnailEditor()
		{
			this.Description = Resources.Resource.DescriptionThumbnail;
			this.Title = Resources.Resource.TitleThumbnailPhoto;
		}

		#region Thumbnails Data factory
		private IThumbnailsData GetThumbnailsData(string saveFunctionType)
		{
			return GetThumbnailsData(saveFunctionType, Guid.Empty);
		}

		private IThumbnailsData GetThumbnailsData(string saveFunctionType, Guid userID)
		{
			return GetThumbnailsData(Type.GetType(saveFunctionType), userID);
		}

		private IThumbnailsData GetThumbnailsData(Type saveFunctionType)
		{
			return GetThumbnailsData(saveFunctionType, Guid.Empty);
		}

		private IThumbnailsData GetThumbnailsData(Type saveFunctionType, Guid userID)
		{
			var thumb = Activator.CreateInstance(saveFunctionType) as IThumbnailsData;
			if (userID == null || Guid.Empty.Equals(userID))
			{
				thumb.UserID = this.UserID;
			}
			else
			{
				thumb.UserID = userID;
			}
			return thumb;
		} 

		protected Guid UserID
		{
			get
			{
				try
				{
					var userID = CoreContext.UserManager.GetUserByUserName(HttpContext.Current.Request[CommonLinkUtility.ParamName_UserUserName]).ID;
					if (!ASC.Core.Users.Constants.LostUser.ID.Equals(userID))
					{
						return userID;
					}
				}
				catch { }
				return SecurityContext.CurrentAccount.ID;
			}
		}

		
		#endregion

		[AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public void SaveThumbnails(int x, int y, int width, int height, string saveFunctionType, Guid userID)
		{
			if (x < 0 || y < 0 || width <= 0 || height <= 0)
				return;

			Point pointSelect = new Point(x, y);
			Size sizeSelect = new Size(width, height);

			var SaveClass = GetThumbnailsData(saveFunctionType, userID);

			List<ThumbnailItem> resaltBitmaps = new List<ThumbnailItem>();

			System.Drawing.Image img = SaveClass.MainImgBitmap;
			if (img == null)
				return;

			foreach (var thumbnail in SaveClass.ThumbnailList)
			{
				Bitmap thumbnailBitmap = new Bitmap(thumbnail.size.Width, thumbnail.size.Height);

				double scaleX = thumbnail.size.Width / (1.0 * sizeSelect.Width);
				double scaleY = thumbnail.size.Height / (1.0 * sizeSelect.Height);

				Rectangle rect = new Rectangle(-(int)(scaleX * pointSelect.X),
												-(int)(scaleY * pointSelect.Y),
												(int)(scaleX * img.Width),
												(int)(scaleY * img.Height));

				using (Graphics graphic = Graphics.FromImage(thumbnailBitmap))
				{
					graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					graphic.FillRectangle(new SolidBrush(Color.Black), 0, 0, thumbnail.size.Width, thumbnail.size.Height);
					graphic.DrawImage(img, rect);
				}
				thumbnail.bitmap = thumbnailBitmap;

				resaltBitmaps.Add(thumbnail);
			}

			SaveClass.Save(resaltBitmaps);
		}

		#endregion
	}
}