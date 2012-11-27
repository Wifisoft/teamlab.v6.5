#region Usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Issue: ID = {ID}, Title = {Title}, Status = {Status}")]
    public class Issue : ProjectEntity
    {
        public int ProjectID { get; set; }

        public string Description { get; set; }

        public Guid AssignedOn { get; set; }

        public IssuePriority Priority { get; set; }

        public IssueStatus Status { get; set; }

        public string DetectedInVersion { get; set; }

        public string CorrectedInVersion { get; set; }
    }
}
