using System.Collections.Generic;
using ASC.Core.Users;

namespace ASC.Web.Core.Users.Import
{
    public interface IUserImporter
    {
        IEnumerable<UserInfo> GetDiscoveredUsers();
    }
}