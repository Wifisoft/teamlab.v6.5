using ASC.Files.Core.Security;
using ASC.Projects.Data;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Projects.Engine
{
    public class SecurityAdapterProvider : IFileSecurityProvider
    {
        public IFileSecurity GetFileSecurity(string data)
        {
            int id;
            return int.TryParse(data, out id) ? GetFileSecurity(id) : null;
        }

        public static IFileSecurity GetFileSecurity(int projectId)
        {
            return new SecurityAdapter(new DaoFactory("projects", TenantProvider.CurrentTenantID), projectId);
        }
    }
}
