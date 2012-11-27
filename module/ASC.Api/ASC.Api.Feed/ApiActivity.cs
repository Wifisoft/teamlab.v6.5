using System;
using ASC.Feed.Activity;
using ASC.Specific;

namespace ASC.Api.Feed
{
    public class ApiActivity
    {
        public ApiActivity(Activity activity)
        {
            this.Action = activity.Action;
            this.Importance = activity.Importance;
            this.IsNew = activity.IsNew;
            this.Item = activity.Item;
            this.ItemType = activity.ItemType;
            this.Priority = activity.Priority;
            this.Source = activity.Source;
            this.ValidTo = (ApiDateTime) activity.ValidTo;
            this.When = (ApiDateTime) activity.When;

            if (activity.CreatedBy.HasValue)
            {
                this.CreatedBy = Employee.EmployeeWraper.Get(activity.CreatedBy.Value);
            }
            this.RelativeTo = activity.RelativeTo;
        }

        public ActivityAction? Action { get; set; }
        public ActivityImportance? Importance { get; set; }
        public int Priority { get; set; }
        public bool? IsNew { get; set; }

        public string Source { get; set; }

        public object ItemType { get; set; }
        public object Item { get; set; }

        public Employee.EmployeeWraper CreatedBy { get; set; }
        public object RelativeTo { get; set; }
        public ApiDateTime When { get; set; }
        public ApiDateTime ValidTo { get; set; }
    }
}