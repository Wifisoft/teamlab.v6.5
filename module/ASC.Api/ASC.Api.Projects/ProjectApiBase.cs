#region Usings

using System;
using ASC.Api.Impl;
using ASC.Core;
using ASC.Projects.Engine;
using System.Runtime.Remoting.Messaging;

#endregion

namespace ASC.Api.Projects
{
    public class ProjectApiBase
    {
        internal const string DbId = "projects";//Copied from projects
        protected ApiContext _context;
        private EngineFactory _engineFactory;

        protected EngineFactory EngineFactory
        {
            get
            {
                if (_engineFactory==null)
                {
                    _engineFactory = new EngineFactory(DbId, TenantId, Data.Storage.StorageFactory.GetStorage(TenantId.ToString(), DbId));
                }
                //NOTE: don't sure if it's need to be here since remoting is gone
                if (CallContext.GetData("CURRENT_ACCOUNT") == null && SecurityContext.IsAuthenticated)
                {
                  CallContext.SetData("CURRENT_ACCOUNT", SecurityContext.CurrentAccount.ID);
                }
                return _engineFactory;
            }
        }

        private static int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        protected static Guid CurrentUserId
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }
    }
}
