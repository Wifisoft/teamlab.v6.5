using System;
using ASC.Core.Users;

namespace ASC.Forum
{
    public class NotFoundUser
    {
        public static UserInfo GetUser(Guid userGuid)
        {
            UserInfo ui = new UserInfo();
            ui.FirstName = "User";
            ui.LastName = "Lost";

            return ui;
        }
    }
}
