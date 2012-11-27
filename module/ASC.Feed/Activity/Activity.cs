using System;

namespace ASC.Feed.Activity
{
    public class Activity
    {
        public ActivityAction? Action { get; set; }
        public ActivityImportance? Importance { get; set; }
        public int Priority { get; set; }
        public bool? IsNew { get; set; }

        public string Source { get; set; }
        public Guid? CreatedBy { get; set; }
        public object RelativeTo { get; set; }
        public DateTime? When { get; set; }
        public DateTime? ValidTo { get; set; }

        public object ItemType { get; set; }
        public object Item { get; set; }

        public Activity()
        {
            
        }

        public Activity(string source, object item) : this(source, item, (Guid?) null)
        {
        }

        public Activity(string source, object item, Guid? createdBy) : this(source, item, createdBy, null)
        {
        }

        public Activity(string source, object item, DateTime? when) : this(source, item, null, when)
        {
        }

        public Activity(string source, object item, Guid? createdBy, DateTime? when) : this(source, item, createdBy, when, null)
        {
        }

        public Activity(string source, object item, Guid? createdBy, DateTime? when, ActivityAction? action)
        {
            if (source == null) throw new ArgumentNullException("source");

            Source = source;
            Item = item;
            CreatedBy = createdBy;
            When = when;
            Action = action;
        }
    }
}