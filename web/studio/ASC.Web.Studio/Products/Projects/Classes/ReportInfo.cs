using System;

namespace ASC.Web.Projects.Classes
{
    [Serializable]
    public class ReportInfo
    {
        public static ReportInfo Empty = new ReportInfo(string.Empty, string.Empty, new string[0]);


        public string Description
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string[] Columns
        {
            get;
            set;
        }

        public ReportInfo(string desc, string title, string[] columns)
        {
            Description = desc;
            Title = title;
            Columns = columns ?? new string[0];
        }
    }
}