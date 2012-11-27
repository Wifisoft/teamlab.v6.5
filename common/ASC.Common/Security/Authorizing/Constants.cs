using System;

namespace ASC.Common.Security.Authorizing
{
    public sealed class Constants
    {
        public static readonly Role Admin = new Role(new Guid("cd84e66b-b803-40fc-99f9-b2969a54a1de"), "Admin", "System administrator");

        public static readonly Role Everyone = new Role(new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), "Everyone", "Everyone");


        public static readonly Role User = new Role(new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), "User", "User, employee of company");

        public static readonly Role Visitor = new Role(new Guid("aaaaf67e-37b9-4e8e-9612-130c1a1cda64"), "Visitor", "Visitor");

        public static readonly Role Demo = new Role(new Guid("64a18d36-7d1b-4509-9616-4c3dbd043de2"), "Demo", "Demo user account");


        public static readonly Role Member = new Role(new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), "Member", "Member of something");

        public static readonly Role Owner = new Role(new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), "Owner", "Object owner");

        public static readonly Role Self = new Role(new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), "Self", "Self");
    }
}