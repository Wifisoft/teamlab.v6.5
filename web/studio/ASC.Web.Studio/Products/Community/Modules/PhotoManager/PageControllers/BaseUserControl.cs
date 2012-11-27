using System;
using System.Web.UI;
using ASC.Core.Security.Authentication;
using ASC.Common.Security.Authentication;
using ASC.Common.Security;
using ASC.Web.Studio;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Community.PhotoManager
{
    public abstract class BaseUserControl : UserControl
    {
        public Guid currentUserID
        {
            get
            {
                return SecurityContext.CurrentAccount.ID;
            }
        }

        public string UserID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_USER];
            }
        }
        public string AlbumID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM];
            }
        }
        public string PhotoID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_PHOTO];
            }
        }
    }
}