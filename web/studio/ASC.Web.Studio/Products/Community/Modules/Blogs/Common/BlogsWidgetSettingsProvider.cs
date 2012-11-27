using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Community.Blogs.Common
{
    public class BlogsWidgetSettingsProvider : IWidgetSettingsProvider
    {
        #region IWidgetSettingsProvider Members

        public bool Check(System.Collections.Generic.List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
        {
            errorMessage = "";

            if (settings == null && settings.Count != 1)
                return false;

            var data = settings[0].ConvertToNumber();
            if (data.Value > 0 && data.Value <= 30)
                return true;

            errorMessage = ASC.Blogs.Core.Resources.BlogsResource.ErrorNotCorrectMaxBlogsSettings;
            return false;
            
        }

        public System.Collections.Generic.List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            List<WidgetSettings> settings = new List<WidgetSettings>();

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BlogsWidgetSettings>(userID);
            settings.Add(new NumberWidgetSettings()
            {
                Title = ASC.Blogs.Core.Resources.BlogsResource.MaxPostsCountSettingsTitle,
                Value = widgetSettings.MaxCountPosts,
                Description = ""
            });

            return settings;

        }

        public void Save(System.Collections.Generic.List<WidgetSettings> settings, Guid widgetID, Guid userID)
        {

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BlogsWidgetSettings>(userID);
            var data = settings[0].ConvertToNumber();
            widgetSettings.MaxCountPosts = data.Value;
            SettingsManager.Instance.SaveSettingsFor<BlogsWidgetSettings>(widgetSettings, userID);

        }

        #endregion
    }
}
