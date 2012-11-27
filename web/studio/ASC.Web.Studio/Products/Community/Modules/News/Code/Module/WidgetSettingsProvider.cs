using System;
using System.Collections.Generic;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard.Settings;

namespace ASC.Web.Community.News.Code.Module
{
	public class WidgetSettingsProvider : IWidgetSettingsProvider
	{
		#region IWidgetSettingsProvider Members

		public bool Check(System.Collections.Generic.List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
		{
			errorMessage = "";

			if (settings == null && settings.Count != 1) return false;
			var data = settings[0].ConvertToNumber();
			if (data.Value > 0 && data.Value <= 30) return true;
			errorMessage = Resources.NewsResource.ErrorNotCorrectMaxNewsSettings;
			return false;
		}

		public List<WidgetSettings> Load(Guid widgetID, Guid userID)
		{
			List<WidgetSettings> settings = new List<WidgetSettings>();

			var widgetSettings = SettingsManager.Instance.LoadSettingsFor<FeedWidgetSettings>(userID);
			settings.Add(new NumberWidgetSettings()
			{
				Title = Resources.NewsResource.MaxNewsCountSettingsTitle,
				Value = widgetSettings.NewsCount,
				Description = ""
			});

			return settings;
		}

		public void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID)
		{
			var widgetSettings = SettingsManager.Instance.LoadSettingsFor<FeedWidgetSettings>(userID);
			var data = settings[0].ConvertToNumber();
			widgetSettings.NewsCount = data.Value;
			SettingsManager.Instance.SaveSettingsFor<FeedWidgetSettings>(widgetSettings, userID);
		}

		#endregion
	}
}
