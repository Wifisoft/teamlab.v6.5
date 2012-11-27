using System;

namespace ASC.Web.UserControls.Wiki
{
    public interface IWikiObjectOwner
    {
        Guid OwnerID { get; }
        object GetObjectId();
    }
}