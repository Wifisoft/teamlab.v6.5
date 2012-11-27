using System;

namespace ASC.Web.UserControls.Wiki.Data
{
    public interface IVersioned : IWikiObjectOwner
    {
        int Tenant { get; set; }

        Guid UserID { get; set; }
        
        int Version { get; set; }
        
        DateTime Date { get; set; }
    }
}