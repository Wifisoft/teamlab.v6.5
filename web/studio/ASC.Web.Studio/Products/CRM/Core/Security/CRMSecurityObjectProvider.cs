#region Import

using System;
using System.Collections.Generic;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using Action = ASC.Common.Security.Authorizing.Action;
#endregion

namespace ASC.CRM.Core
{
    public class CRMSecurityObjectProvider : ISecurityObjectProvider
    {
        private readonly DaoFactory _daoFactory;

        public CRMSecurityObjectProvider(DaoFactory daoFactory)
        {
            _daoFactory = daoFactory;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            if (!(objectId is Task)) return null;

            var task = (Task)objectId;

            if (task.EntityID == 0 && task.ContactID == 0) return null;

            if (task.EntityID == 0)
                return _daoFactory.GetContactDao().GetByID(task.ContactID);

            switch (task.EntityType)
            {

                case EntityType.Opportunity:
                    return _daoFactory.GetDealDao().GetByID(task.EntityID);
                case EntityType.Case:
                    return _daoFactory.GetCasesDao().GetByID(task.EntityID);
            }

            return null;
        }

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return false; }
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
 
          //   Constants.Everyone
          // if (_daoFactory.GetManagerDao().GetAll(false).Contains(ASC.Core.CoreContext.UserManager.GetUsers(account.ID)))
          //   return new Action[]
            throw new NotImplementedException();
        }
    }
}