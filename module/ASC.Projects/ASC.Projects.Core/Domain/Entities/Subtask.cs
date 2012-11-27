using System;
using System.Diagnostics;


namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("SubTask: ID = {ID}, Title = {Title}")]
    public class Subtask
    {
        public int ID { get; set; }

        public String Title { get; set; }

        public Guid Responsible { get; set; }

        public TaskStatus Status { get; set; }

        public int Task { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public Guid LastModifiedBy { get; set; }

        public DateTime StatusChangedOn { get; set; }
    }
}
