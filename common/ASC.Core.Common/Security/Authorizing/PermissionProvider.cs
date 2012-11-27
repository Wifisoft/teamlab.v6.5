using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Security.Authorizing
{
    class PermissionProvider : IPermissionProvider
    {
        public IEnumerable<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (action == null) throw new ArgumentNullException("action");

            return CoreContext.AuthorizationManager
                .GetAcesWithInherits(subject.ID, action.ID, objectId, secObjProvider)
                .Select(r => new Ace(r.ActionId, r.Reaction));
        }
    }
}