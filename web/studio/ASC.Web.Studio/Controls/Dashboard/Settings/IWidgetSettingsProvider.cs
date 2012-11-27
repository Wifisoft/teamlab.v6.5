using System;
using System.Collections.Generic;

namespace ASC.Web.Studio.Controls.Dashboard.Settings
{
    public interface IWidgetSettingsProvider
    {
        bool Check(List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage);
        
        void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID);

        List<WidgetSettings> Load(Guid widgetID, Guid userID);
    }
}
