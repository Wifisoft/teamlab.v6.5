using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Security.Authorizing
{
    class PermissionResolver : IPermissionResolver
    {
        private readonly AzManager azManager;


        public PermissionResolver(AzManager azManager, IPermissionProvider permissionProvider)
        {
            if (azManager == null) throw new ArgumentNullException("azManager");
            if (permissionProvider == null) throw new ArgumentNullException("permissionProvider");

            this.azManager = azManager;
        }


        public bool Check(ISubject subject, params IAction[] actions)
        {
            return Check(subject, null, null, actions);
        }

        public bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
            return denyActions.Length == 0;
        }

        public void Demand(ISubject subject, params IAction[] actions)
        {
            Demand(subject, null, null, actions);
        }

        public void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
            if (0 < denyActions.Length)
            {
                throw new AuthorizingException(
                    subject,
                    Array.ConvertAll(denyActions, r => r.TargetAction),
                    Array.ConvertAll(denyActions, r => r.DenySubject),
                    Array.ConvertAll(denyActions, r => r.DenyAction));
            }
        }


        private DenyResult[] GetDenyActions(ISubject subject, IAction[] actions, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
        {
            var denyActions = new List<DenyResult>();
            if (actions == null) actions = new IAction[0];

            if (subject == null)
            {
                denyActions = actions.Select(a => new DenyResult(a, null, null)).ToList();
            }
            else if (subject is ISystemAccount && subject.ID == Constants.CoreSystem.ID)
            {
                // allow all
            }
            else
            {
                ISubject denySubject = null;
                IAction denyAction = null;
                foreach (var action in actions)
                {
                    var allow = azManager.CheckPermission(subject, action, objectId, securityObjProvider, out denySubject, out denyAction);
                    if (!allow)
                    {
                        denyActions.Add(new DenyResult(action, denySubject, denyAction));
                        break;
                    }
                }
            }
            return denyActions.ToArray();
        }

        private class DenyResult
        {
            public readonly IAction TargetAction;
            public readonly ISubject DenySubject;
            public readonly IAction DenyAction;

            public DenyResult(IAction targetAction, ISubject denySubject, IAction denyAction)
            {
                TargetAction = targetAction;
                DenySubject = denySubject;
                DenyAction = denyAction;
            }
        }
    }
}