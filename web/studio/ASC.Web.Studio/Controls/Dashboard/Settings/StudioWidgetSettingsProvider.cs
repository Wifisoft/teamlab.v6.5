using System;
using System.Collections.Generic;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard.Widgets;
using ASC.Web.Studio.UserControls.Users.Activity;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.Controls.Dashboard.Settings
{
    public class StudioWidgetSettingsProvider : IWidgetSettingsProvider
    {
        #region IWidgetSettingsProvider Members

        public bool Check(List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
        {
            errorMessage = "";

            //bithdays
            if (widgetID.Equals(BirthdayReminderWidget.WidgetID))
            {
                if (settings == null || settings.Count != 1)
                    return false;

                var data = settings[0].ConvertToNumber();
                if (data.Value > 0 && data.Value <= 30)
                    return true;


                errorMessage = Resources.Resource.ErrorNotCorrectDaysBeforeBirthdaySettings;
                return false;
            }

            //user activity
            else if (widgetID.Equals(ProductActivity.WidgetID))
            {
                if (settings == null || settings.Count != 1)
                    return false;

                var data = settings[0].ConvertToNumber();
                if (data.Value > 0 && data.Value <= 30)
                    return true;


                errorMessage = Resources.Resource.ErrorNotCorrectCountActivities;
                return false;
            }

             //new employees
            else if (widgetID.Equals(NewEmployeeWidget.WidgetID))
            {
                if (settings == null || settings.Count != 1)
                    return false;

                var data = settings[0].ConvertToNumber();
                if (data.Value > 0 && data.Value <= 20)
                    return true;

                errorMessage = Resources.Resource.ErrorNotCorrectNewWorkerCountSettings;
                return false;
            }

            return false;
        }

        public void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID)
        {
            if (widgetID.Equals(BirthdayReminderWidget.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BirthdayReminderWidgetSettings>(userID);
                var data = settings[0].ConvertToNumber();
                widgetSettings.DaysBeforeBirthday = data.Value;
                SettingsManager.Instance.SaveSettingsFor<BirthdayReminderWidgetSettings>(widgetSettings, userID);
            }

            else if (widgetID.Equals(ProductActivity.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ProductActivityWidgetSettings>(userID);
                var data = settings[0].ConvertToNumber();
                widgetSettings.CountActivities = data.Value;
                SettingsManager.Instance.SaveSettingsFor<ProductActivityWidgetSettings>(widgetSettings, userID);
            }

            else if (widgetID.Equals(NewEmployeeWidget.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<NewEmployeeWidgetSettings>(userID);
                var data = settings[0].ConvertToNumber();
                widgetSettings.NewWorkerCount = data.Value;
                SettingsManager.Instance.SaveSettingsFor<NewEmployeeWidgetSettings>(widgetSettings, userID);
            }
        }

        public List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            List<WidgetSettings> settings = new List<WidgetSettings>();

            if (widgetID.Equals(BirthdayReminderWidget.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BirthdayReminderWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings()
                {
                    Title = Resources.Resource.DaysBeforeBirthdaySettingsTitle,
                    Value = widgetSettings.DaysBeforeBirthday,
                    Description = ""
                });
            }

            else if (widgetID.Equals(ProductActivity.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ProductActivityWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings()
                {
                    Title = Resources.Resource.ProductActivityCount,
                    Value = widgetSettings.CountActivities,
                    Description = ""
                });
            }

            else if (widgetID.Equals(NewEmployeeWidget.WidgetID))
            {
                var widgetSettings = SettingsManager.Instance.LoadSettingsFor<NewEmployeeWidgetSettings>(userID);
                settings.Add(new NumberWidgetSettings()
                {
                    Title = Resources.Resource.NewWorkerCountSettingsTitle,
                    Value = widgetSettings.NewWorkerCount,
                    Description = ""
                });
            }

            return settings;
        }

        #endregion
    }
}
