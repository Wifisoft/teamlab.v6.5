using System.Collections.Generic;

namespace ASC.Web.Studio.Core.Import
{
    public interface IUserImporter
    {
        IEnumerable<ContactInfo> GetDiscoveredUsers();
    }
}