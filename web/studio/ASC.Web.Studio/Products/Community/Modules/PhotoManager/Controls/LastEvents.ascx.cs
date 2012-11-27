using System;
using System.Text;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Utility.Skins;
using System.Web;

namespace ASC.Web.Community.PhotoManager.Controls
{
    public partial class LastEvents : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadEventList();
        }

        private int maxCountLastEvents = 3;

        public int MaxCountLastEvents
        {
            get { return maxCountLastEvents; }
            set { maxCountLastEvents = value; }
        }

        private void LoadEventList()
        {
            eventsContainer.Title = PhotoManagerResource.LastEventsTitle;
            eventsContainer.HeaderCSSClass = "studioSideBoxTagCloudHeader";
            eventsContainer.ImageURL = WebImageSupplier.GetAbsoluteWebPath("photo_icon.png", ASC.PhotoManager.PhotoConst.ModuleID);

            var sb = new StringBuilder();
            var storage = StorageFactory.GetStorage();
            var list = storage.GetEvents(0, maxCountLastEvents);

            if (list.Count == 0)
            {
                eventsContainer.Visible = false;
                return;
            }

            foreach (var Event in list)
            {
                var caption = HttpUtility.HtmlEncode(Event.Name);

                sb.Append("<div style=\"margin-top: 10px;padding-left:20px;word-wrap: break-word;\">");
                sb.Append("<a class=\"linkAction\" href=\"" + ASC.PhotoManager.PhotoConst.PAGE_DEFAULT + "?" + ASC.PhotoManager.PhotoConst.PARAM_EVENT + "=" + Event.Id + "\">" + caption + "</a>");
                sb.Append("</div>");
            }

            ltrEvents.Text = sb.ToString();
        }
    }
}