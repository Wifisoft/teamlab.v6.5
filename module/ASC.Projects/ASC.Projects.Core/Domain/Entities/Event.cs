#region Usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Event: ID = {ID}, Title = {Title}")]
    public class Event : ProjectEntity
    {
        public String Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
