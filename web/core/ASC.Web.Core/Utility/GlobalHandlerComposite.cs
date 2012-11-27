using System;
using System.Collections.Generic;
using log4net;

namespace ASC.Web.Core.Utility
{
    public class GlobalHandlerComposite : IGlobalHandler
    {
        private readonly List<IGlobalHandler> handlers;


        public GlobalHandlerComposite(List<IGlobalHandler> handlers)
        {
            this.handlers = handlers;
        }


        public void Login(Guid userID)
        {
            DoAction(h => h.Login(userID));
        }

        public void Logout(Guid userID)
        {
            DoAction(h => h.Logout(userID));
        }


        private void DoAction(Action<IGlobalHandler> action)
        {
            handlers.ForEach(h =>
            {
                try
                {
                    action(h);
                }
                catch (Exception err)
                {
                    LogManager.GetLogger("ASC.Web").Error(action.Method + " GlobalHandler Error", err);
                }
            });
        }
    }
}
