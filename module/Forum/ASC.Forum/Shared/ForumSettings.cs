using System;
using System.Configuration;
using ASC.Common.Data;
using ASC.Common.Security;

namespace ASC.Forum
{
	public static class ForumSettings
    {
        public static ISecurityObject AdminSecurityObject { get; set; }

        public static Guid ModuleID { get { return new Guid("{853B6EB9-73EE-438d-9B09-8FFEEDF36234}"); } }


        internal static string DatabaseID { get; private set; }


        public static void Configure(string databaseID)
        {
            DatabaseID = databaseID;
        }
    }
}
