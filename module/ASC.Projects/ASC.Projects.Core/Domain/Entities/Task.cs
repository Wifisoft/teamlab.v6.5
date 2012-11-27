#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Task: ID = {ID}, Title = {Title}, Status = {Status}")]
    public class Task : ProjectEntity
    {
        public String Description { get; set; }

        public Guid Responsible { get; set; }

        public TaskPriority Priority { get; set; }

        public TaskStatus Status { get; set; }

        public int Milestone { get; set; }

        public int SortOrder { get; set; }

        public DateTime Deadline { get; set; }

        public List<Subtask> SubTasks { get; set; }

        public HashSet<Guid> Responsibles { get; set; }

        public Milestone MilestoneDesc { get; set; }

        public DateTime StatusChangedOn { get; set; }
    }
}
