using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain.Reports;

namespace ASC.Projects.Core.Domain
{
    public class TaskFilter : ReportFilter
    {
        public int? Milestone { get; set; }

        public bool Follow { get; set; }

        public string SortBy { get; set; }

        public bool SortOrder { get; set; }

        public string SearchText { get; set; }

        public long Offset { get; set; }

        public long Max { get; set; }

        public int LastId { get; set; }

        public bool MyProjects { get; set; }

        public bool MyMilestones { get; set; }

        public Guid? ParticipantId { get; set; }

        public int TagId { get; set; }

        public TaskFilter()
        {
            Max = 150001;
        }

        public Dictionary<string, Dictionary<string, bool>> SortColumns
        {
            get
            {
                return new Dictionary<string, Dictionary<string, bool>>
                           {
                               {
                                   "Task", new Dictionary<string, bool>{{"deadline", true}, {"priority", false}, {"create_on", false}, {"title", true}}
                                   },
                               {
                                   "Milestone", new Dictionary<string, bool> {{"deadline", true}, {"create_on", false}, {"title", true}}
                                   },
                               {
                                   "Project", new Dictionary<string, bool> {{"create_on", false}, {"title", true}}
                                   },
                               {
                                   "Message", new Dictionary<string, bool> {{"create_on", false}, {"title", true}}
                                   },
                               {
                                   "TimeSpend", new Dictionary<string, bool> {{"date", false}, {"hours", false}, {"note", true}}
                                   }
                           };
            }
        }
    }
}
