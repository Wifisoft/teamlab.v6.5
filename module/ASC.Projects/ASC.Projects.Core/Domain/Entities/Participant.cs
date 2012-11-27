using System;
using System.Diagnostics;
using ASC.Core;
using ASC.Core.Users;
using System.Collections;

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("{UserInfo.ToString()}")]
    public class Participant : IComparable
    {
        public Guid ID { get; private set; }

        public bool CanReadFiles { get; private set; }

        public bool CanReadMilestones { get; private set; }

        public bool CanReadMessages { get; private set; }

        public bool CanReadTasks { get; private set; }

        public UserInfo UserInfo
        {
            get { return CoreContext.UserManager.GetUsers(ID); }
        }


        public Participant(Guid userID)
        {
            ID = userID;
        }
        public Participant(Guid userID, ProjectTeamSecurity security)
        {
            ID = userID;
            CanReadFiles = (security & ProjectTeamSecurity.Files) != ProjectTeamSecurity.Files;
            CanReadMilestones = (security & ProjectTeamSecurity.Milestone) != ProjectTeamSecurity.Milestone;
            CanReadMessages = (security & ProjectTeamSecurity.Messages) != ProjectTeamSecurity.Messages;
            CanReadTasks = (security & ProjectTeamSecurity.Tasks) != ProjectTeamSecurity.Tasks;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var p = obj as Participant;
            return p != null && p.ID == ID;
        }

        public int CompareTo(object obj)
        {
            var other = obj as Participant;
            return other == null ?
                Comparer.Default.Compare(this, obj) :
                UserFormatter.Compare(UserInfo, other.UserInfo);
        }
    }
}

