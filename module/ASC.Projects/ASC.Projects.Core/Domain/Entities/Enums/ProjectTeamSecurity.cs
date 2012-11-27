using System;

namespace ASC.Projects.Core.Domain
{
    [Flags]
    public enum ProjectTeamSecurity
    {
        None = 0,
        Messages = 1,
        Tasks = 2,
        Files = 4,
        Milestone = 8,
    }
}
