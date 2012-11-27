using System;

namespace ASC.Web.Core.Users.Activity
{
    public static class UserActivityConstants
    {
        public static int ImportantContent { get { return 30; } }
        public static int NormalContent { get { return 20; } }
        public static int SmallContent { get { return 10; } }
        public static int ImportantActivity { get { return 10; } }
        public static int NormalActivity { get { return 5; } }
        public static int SmallActivity { get { return 1; } }

        public static int Max { get { return 50; } }
        public static int Min { get { return 1; } }

        public static int ActivityActionType { get { return 1; } }
        public static int ContentActionType { get { return 3; } }
        public static int AllActionType { get { return 0; } }
    }
}
