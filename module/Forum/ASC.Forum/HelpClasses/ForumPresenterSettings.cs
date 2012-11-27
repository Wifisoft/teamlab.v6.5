using System;
using ASC.Common.Security;
using System.Reflection;

namespace ASC.Forum
{
	public class ForumPresenterSettings
    {
        public Guid ID { get; private set; }

        public ISecurityObject AdminSecurityObject { get; set; }

        public ForumPresenterSettings()
        {
            this.ID = Guid.NewGuid();
        }

    }
}
