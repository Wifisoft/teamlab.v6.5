using System;

namespace ASC.Web.Core.Users.Activity
{
    public interface IUserActivityFilter
    {
        bool FilterActivity(UserActivity activity);
    }
}