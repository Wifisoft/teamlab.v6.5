using System;
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Wiki.Data
{
    public class Page : IVersioned
    {
        public int Tenant { get; set; }
        public Guid UserID { get; set; }
        public int Version { get; set; }
        public DateTime Date { get; set; }

        public Guid OwnerID { get; set; }
        public object GetObjectId()
        {
            return PageName;
        }

        public string PageName { get; set; }
        public string Body { get; set; }
        

        public Page()
        {
            PageName = Body = string.Empty;
            Date = TenantUtil.DateTimeNow();
        }      
    }
}