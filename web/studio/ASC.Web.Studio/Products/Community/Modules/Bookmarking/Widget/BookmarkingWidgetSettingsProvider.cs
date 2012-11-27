using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Core.Utility.Settings;
using BookmarkingResource = ASC.Web.UserControls.Bookmarking.Resources.BookmarkingUCResource;


namespace ASC.Web.Community.Bookmarking.Widget
{
	public class BookmarkingWidgetSettingsProvider : IWidgetSettingsProvider
	{
		#region IWidgetSettingsProvider Members

		public bool Check(List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
		{
			errorMessage = string.Empty;

			if (settings == null && settings.Count != 1)
				return false;

			var data = settings[0].ConvertToNumber();
			if (data.Value > 0 && data.Value <= 30)
				return true;

			errorMessage = BookmarkingResource.ErrorNotCorrectMaxBookmarksSettings;
			return false;
		}

		public List<WidgetSettings> Load(Guid widgetID, Guid userID)
		{
			List<WidgetSettings> settings = new List<WidgetSettings>();

			//amount of the bookmarks to be displayed
			var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BookmarkingWidgetSettings>(userID);
			settings.Add(new NumberWidgetSettings()
			{
				Title = BookmarkingResource.MaxBookmarksCountSettingsTitle,
				Value = widgetSettings.MaxCountBookmarks,
				Description = string.Empty
			});

			return settings;
		}

		public void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID)
		{
			//amount of the bookmarks to be displayed
			var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BookmarkingWidgetSettings>(userID);
			var data = settings[0].ConvertToNumber();
			widgetSettings.MaxCountBookmarks = data.Value;
			SettingsManager.Instance.SaveSettingsFor<BookmarkingWidgetSettings>(widgetSettings, userID);
		}

		#endregion
	}
}
