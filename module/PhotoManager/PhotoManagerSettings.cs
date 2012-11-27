using System;
using ASC.Common.Security;

namespace ASC.PhotoManager
{
    public static class PhotoManagerSettings
    {
        public static ISecurityObject AdminSecurityObject { get; set; }

        public static Guid ModuleID { get { return PhotoConst.ModuleId; } }
    }
}
