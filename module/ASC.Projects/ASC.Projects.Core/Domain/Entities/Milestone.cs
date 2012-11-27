#region Usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Milestone: ID = {ID}, Title = {Title}, DeadLine = {DeadLine}")]
    public class Milestone : ProjectEntity
    {
        public String Description { get; set; }

        public Guid Responsible { get; set; }

        public MilestoneStatus Status { get; set; }

        public bool IsNotify { get; set; }

        public bool IsKey { get; set; }

        public DateTime DeadLine { get; set; }

        public bool CurrentUserHasTasks { get; set; }

        public int ActiveTaskCount { get; set; }

        public int ClosedTaskCount { get; set; }

        public DateTime StatusChangedOn { get; set; }
    }
}
