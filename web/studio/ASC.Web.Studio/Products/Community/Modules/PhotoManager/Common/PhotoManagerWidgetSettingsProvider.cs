using System;
using System.Collections.Generic;
using ASC.PhotoManager.Resources;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Community.PhotoManager.Common
{
    public class PhotoManagerWidgetSettingsProvider : IWidgetSettingsProvider
    {
        #region IWidgetSettingsProvider Members

        public bool Check(System.Collections.Generic.List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
        {
            errorMessage = "";

            if (settings == null && settings.Count != 1)
                return false;

            var data = settings[0].ConvertToNumber();
            if (data.Value > 0 && data.Value <= 10)
                return true;

            errorMessage = PhotoManagerResource.ErrorNotCorrectMaxAlbumsSettings;
            return false;

        }

        public System.Collections.Generic.List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            List<WidgetSettings> settings = new List<WidgetSettings>();

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<PhotoManagerWidgetSettings>(userID);
            settings.Add(new NumberWidgetSettings()
            {
                Title = PhotoManagerResource.MaxLastAlbumsCountSettingsTitle,
                Value = widgetSettings.MaxCountLastAlbums,
                Description = ""
            });
            
            return settings;

        }

        public void Save(System.Collections.Generic.List<WidgetSettings> settings, Guid widgetID, Guid userID)
        {

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<PhotoManagerWidgetSettings>(userID);
            var data = settings[0].ConvertToNumber();
            widgetSettings.MaxCountLastAlbums = data.Value;
            SettingsManager.Instance.SaveSettingsFor<PhotoManagerWidgetSettings>(widgetSettings, userID);

        }

        #endregion
    }
}
