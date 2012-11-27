using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki.Common
{
    public class WikiWidgetSettingsProvider : IWidgetSettingsProvider
    {
        #region IWidgetSettingsProvider Members

        public bool Check(List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
        {
            errorMessage = "";

            if (settings == null && settings.Count != 1)
                return false;

            foreach (WidgetSettings sett in settings)
            {
                if (sett.ID.Equals(new Guid("{F95073D9-7EBF-42a3-A06C-5B2EF88AE57B}")))
                {
                    NumberWidgetSettings data = sett.ConvertToNumber();
                    if (data.Value > 0 && data.Value <= 30)
                        return true;

                    errorMessage = WikiResource.ErrorNotCorrectMaxPageCountSettings;
                    return false;

                }
            }

            return false;
        }

        public List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            List<WidgetSettings> settings = new List<WidgetSettings>();
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<WikiWidgetSettings>(userID);
            settings.Add(new NumberWidgetSettings()
            {
                ID = new Guid("{F95073D9-7EBF-42a3-A06C-5B2EF88AE57B}"),
                Title = WikiResource.MaxLastPagesCountSettingsTitle,
                Value = widgetSettings.NewPageCount,
                Description = ""
            });


            return settings;
        }

        public void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID)
        {
            if (settings == null && settings.Count != 2)
                return;

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<WikiWidgetSettings>(userID);
            foreach (var set in settings)
            {

                if (set.ID.Equals(new Guid("{F95073D9-7EBF-42a3-A06C-5B2EF88AE57B}")))
                {
                    var data = set.ConvertToNumber();
                    widgetSettings.NewPageCount = data.Value;
                    
                }
            }

            SettingsManager.Instance.SaveSettingsFor<WikiWidgetSettings>(widgetSettings, userID);
        }

        #endregion
    }
}
