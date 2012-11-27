#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Projects.Core.Domain
{
    public class ProjectActivityFilter
    {
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int ProjectId { get; set; }

        public string Type { get; set; }

        public string SortBy { get; set; }

        public bool SortOrder { get; set; }

        public string SearchText { get; set; }

        public int Offset { get; set; }

        public int Max { get; set; }

        public int LastId { get; set; }


        public ProjectActivityFilter()
        {
            Max = 150001;
        }
    }
}
