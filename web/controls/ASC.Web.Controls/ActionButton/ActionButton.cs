using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;

namespace ASC.Web.Controls
{

	[ToolboxData("<{0}:ActionButton runat=server></{0}:ActionButton>")]
	public class ActionButton : WebControl
	{
		public string ButtonCssClass { get; set; }

		public string ButtonID { get; set; }

		public string ButtonStyle { get; set; }

		public string ButtonText { get; set; }

		public string AjaxRequestText { get; set; }

		public string OnClickJavascript { get; set; }

		public bool EnableRedirectAfterAjax { get; set; }

		public bool DisableInputs { get; set; }

		public string ImageSrc { get; set; }

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			StringBuilder sb = new StringBuilder();

			if (string.IsNullOrEmpty(ButtonID))
			{
				ButtonID = Guid.NewGuid().ToString();
			}

			if (string.IsNullOrEmpty(ImageSrc))
			{
				try
				{
					ImageSrc = Page.ClientScript.GetWebResourceUrl(this.GetType(),
						"ASC.Web.Controls.ActionButton.images.ajax_progress_loader.gif");
				}
				catch { }
			}

			sb.AppendFormat(@"
<div style='display: none;' id='{0}AjaxRequestPanel'>
	<div class='textMediumDescribe'>
		{1}
	</div>
", ButtonID, AjaxRequestText);

			if (!string.IsNullOrEmpty(ImageSrc))
			{
				sb.AppendFormat("<img src='{0}' />", ImageSrc);
			}
			sb.AppendFormat("</div>");

			sb.AppendFormat(@"<a class='{0}' id='{1}' href='javascript:void(0);' onclick='actionButtonClick(this.id, {5}, {6}); {2}' style='{3}'>{4}</a>",
				ButtonCssClass, ButtonID, OnClickJavascript, ButtonStyle, ButtonText, EnableRedirectAfterAjax.ToString().ToLower(), DisableInputs.ToString().ToLower());

			writer.Write(sb);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			InitScripts();
		}


		#region Init Scripts
		private void InitScripts()
		{

			if (!Page.ClientScript.IsClientScriptBlockRegistered("ActionButtonJavaScript"))
			{
				string ActionButtonJavaScriptLocation = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.Controls.ActionButton.js.actionbutton.js");
				Page.ClientScript.RegisterClientScriptInclude("ActionButtonJavaScript", ActionButtonJavaScriptLocation);
			}
		}
		#endregion


		#region RenderActionButton
		public static string RenderActionButton(string buttonText, string ajaxRequestText, string onclickJavascript)
		{
			return RenderActionButton(null, buttonText, ajaxRequestText, onclickJavascript, null, null);
		}

		public static string RenderActionButton(string buttonID, string buttonText, string ajaxRequestText, string onclickJavascript, string cssClass, string imageSrc)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (HtmlTextWriter textWriter = new HtmlTextWriter(sw))
				{
					var actionButton = new ActionButton()
					{
						ButtonText = buttonText,
						AjaxRequestText = ajaxRequestText,
						OnClickJavascript = onclickJavascript,
					};
					if (!string.IsNullOrEmpty(buttonID))
					{
						actionButton.ButtonID = buttonID;
					}
					if (!string.IsNullOrEmpty(cssClass))
					{
						actionButton.ButtonCssClass = cssClass;
					}
					if (!string.IsNullOrEmpty(imageSrc))
					{
						actionButton.ImageSrc = imageSrc;
					}
					actionButton.RenderControl(textWriter);
				}
			}
			return sb.ToString();
		}
		#endregion
	}
}
