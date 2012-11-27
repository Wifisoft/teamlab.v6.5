using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Forum
{
    public interface ISecurityActionView
    {
        bool IsAccessible { set; }

        event EventHandler<SecurityAccessEventArgs> ValidateAccess;
    }

    public class SecurityAccessEventArgs : EventArgs
    {
        public ForumAction Action { get; private set; }

        public Object TargetObject { get; private set; }

        public SecurityAccessEventArgs(ForumAction action, object targetObject)
        {
            this.Action = action;
            this.TargetObject = targetObject;

        }
    }
}
