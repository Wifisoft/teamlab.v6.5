using System;

namespace ASC.Web.Studio.Controls.Dashboard.Settings
{   

    public class WidgetSettings
    {
        public Guid ID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual object Value { get; set; }

        public WidgetSettings()
        {
            this.ID = Guid.NewGuid();
        }

        public NumberWidgetSettings ConvertToNumber()
        {
            int val = 0;
            try
            {
                val = Convert.ToInt32(this.Value);
            }
            catch
            {
                val = 0;
            }

            return new NumberWidgetSettings()
            {
                ID = this.ID,
                Description = this.Description,
                Title = this.Title,
                Value = val
            };
        }

        public BoolWidgetSettings ConvertToBool()
        {
            bool val = false;
            try
            {
                val = Convert.ToBoolean(this.Value);
            }
            catch
            {
                val = false;
            }

            return new BoolWidgetSettings()
            {
                ID = this.ID,
                Description = this.Description,
                Title = this.Title,
                Value = val
            };
        }
    }

    public sealed class NumberWidgetSettings : WidgetSettings
    {
        public new int Value { get; set; }
    }

    public sealed class BoolWidgetSettings : WidgetSettings
    {
        public new bool Value { get; set; }
    }
}
