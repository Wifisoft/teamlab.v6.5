using System;
using System.Linq;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Forum;

namespace ASC.Web.Community.Forum
{
    public class ForumGlobalHandler : IGlobalHandler
    {
        #region IGlobalHandler Members

        public void Login(Guid userID)
        {
            ForumDataProvider.InitFirstVisit();
        }

        public void Logout(Guid userID)
        {

        }

        #endregion
    }
}
