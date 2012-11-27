#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Caching;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Entities;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Utility;
using Action = ASC.Common.Security.Authorizing.Action;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;

#endregion

namespace ASC.CRM.Core
{

    public static class CRMSecurity
    {

        #region Members

        private static readonly IAction _actionRead = new Action(new Guid("{6F05C382-8BCA-4469-9424-C807A98C40D7}"), "", true, false);
        private static readonly ICache _securityChache = new AspCache();

        #endregion

        #region Check Permissions

        private static ISecurityObjectProvider GetCRMSecurityProvider()
        {
            return new CRMSecurityObjectProvider(Global.DaoFactory);
        }

        private static bool IsPrivate(ISecurityObjectId entity)
        {

            var entityAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity, GetCRMSecurityProvider());

            return entityAces.Count() > 1;

        }

        private static bool CanAccessTo(ISecurityObjectId entity)
        {
            return IsAdmin ? true : SecurityContext.CheckPermissions(entity, GetCRMSecurityProvider(), _actionRead);
        }

        private static void MakePublic(ISecurityObjectId entity)
        {
            SetAccessTo(entity, new List<Guid>());
        }

        public static IEnumerable<int>  GetPrivateItems(Type objectType)
        {
            if (IsAdmin) return new List<int>();

          //  var chacheKEY = String.Format(_securityChacheKEY, objectType.FullName,
          //                                ASC.Core.SecurityContext.CurrentAccount.ID);

          // var chacheValue = _securityChache.Get(chacheKEY);

           //if (chacheValue != null) return (IEnumerable<int>)chacheValue;
            
           var ids = CoreContext.AuthorizationManager
                    .GetAces(Guid.Empty, _actionRead.ID)
                    .Where(item => !String.IsNullOrEmpty(item.ObjectId) && item.ObjectId.StartsWith(objectType.FullName))
                    .GroupBy(item => item.ObjectId, item => item.SubjectId)
                    .Where(item => !item.Contains(ASC.Core.SecurityContext.CurrentAccount.ID))
                    .Select(item => Convert.ToInt32(item.Key.Split(new[]{'|'})[1]))
                    .ToList();

        //   _securityChache.Insert(chacheKEY, ids, TimeSpan.FromMinutes(30));

           return ids;

        }

        public static int GetPrivateItemsCount(Type objectType)
        {
            return GetPrivateItems(objectType).Count();

        }

        private static Dictionary<Guid, String> GetAccessSubjectTo(ISecurityObjectId entity)
        {

            var allAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity,
                                                                               GetCRMSecurityProvider())
                .Where(item => item.SubjectId != Constants.GroupEveryone.ID);

            var result = new Dictionary<Guid, String>();

            foreach (var azRecord in allAces)
            {
                var displayName = CoreContext.UserManager.GetUsers(azRecord.SubjectId).DisplayUserName();

                if (!result.ContainsKey(azRecord.SubjectId))
                    result.Add(azRecord.SubjectId, displayName);

            }

            return result;

        }


        private static void SetAccessTo(ISecurityObjectId entity, List<Guid> subjectID)
        {


          //  var chacheKEY = String.Format(_securityChacheKEY, entity.GetType().FullName,
          //                   ASC.Core.SecurityContext.CurrentAccount.ID);

          //  _securityChache.Remove(chacheKEY);

            // Delete relative  keys
          //  _securityChache.Insert(String.Concat(TenantProvider.CurrentTenantID.ToString(), "/contacts"), String.Empty, TimeSpan.FromMinutes(10));

            if (subjectID.Count == 0)
            {
                CoreContext.AuthorizationManager.RemoveAllAces(entity);

            
                return;
            }


            var currentObjectAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity, GetCRMSecurityProvider());

            currentObjectAces.Where(azRecord => !subjectID.Contains(azRecord.SubjectId))
            .ToList().ForEach(azRecord =>
                                  {
                                      if ((azRecord.SubjectId == Constants.GroupEveryone.ID) && azRecord.Reaction == AceType.Allow)
                                          return;

                                      CoreContext.AuthorizationManager.RemoveAce(azRecord);
                                  });



            var oldSubjectIDList = currentObjectAces.Select(azRecord => azRecord.SubjectId).ToList();

            subjectID.FindAll(item => !oldSubjectIDList.Contains(item))
            .ForEach(item => CoreContext.AuthorizationManager.AddAce(new AzRecord(item, _actionRead.ID, AceType.Allow,
                                                                                  entity)));

            CoreContext.AuthorizationManager.AddAce(new AzRecord(Constants.GroupEveryone.ID, _actionRead.ID, AceType.Deny,
                                                                 entity));
        }

        #endregion

        public static void SetAccessTo(File file)
        {

            if (IsAdmin || file.CreateBy == ASC.Core.SecurityContext.CurrentAccount.ID || file.ModifiedBy == ASC.Core.SecurityContext.CurrentAccount.ID)
                file.Access = FileShare.None;
            else
                file.Access = FileShare.Read;
        }

        public static bool CanAccessTo(Deal deal)
        {
            return CanAccessTo((ISecurityObjectId)deal);
        }

        public static bool CanAccessTo(RelationshipEvent relationshipEvent)
        {
            return CanAccessTo((ISecurityObjectId)relationshipEvent);
        }

        public static bool CanAccessTo(Contact contact)
        {
            return CanAccessTo((ISecurityObjectId)contact);
        }

        public static bool CanAccessTo(Task task)
        {
            if (IsAdmin || task.ResponsibleID == ASC.Core.SecurityContext.CurrentAccount.ID ||
                (task.ContactID == 0 && task.EntityID == 0) || task.CreateBy == ASC.Core.SecurityContext.CurrentAccount.ID) return true;

            return CanAccessTo((ISecurityObjectId)task);
        }

        public static bool CanEdit(File file)
        {
            if (!(IsAdmin || file.CreateBy == SecurityContext.CurrentAccount.ID || file.ModifiedBy == SecurityContext.CurrentAccount.ID))
                return false;

            if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                return false;

            return true;
        }


        public static bool CanEdit(RelationshipEvent relationshipEvent)
        {
            return (IsAdmin || relationshipEvent.CreateBy == SecurityContext.CurrentAccount.ID);
        }

        public static bool CanEdit(Task task)
        {
            return (IsAdmin || task.ResponsibleID == SecurityContext.CurrentAccount.ID || task.CreateBy == SecurityContext.CurrentAccount.ID);
        }

        public static bool CanEdit(ListItem listItem)
        {
            return IsAdmin;
        }

        public static bool CanEdit(Contact contact)
        {
            return CanAccessTo(contact);
        }

        public static bool CanAccessTo(Cases cases)
        {
            return CanAccessTo((ISecurityObjectId)cases);
        }

        public static void SetAccessTo(Deal deal, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)deal, subjectID);

        }

        public static void MakePublic(Deal deal)
        {
            MakePublic((ISecurityObjectId)deal);
        }


        public static void SetAccessTo(RelationshipEvent relationshipEvent, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)relationshipEvent, subjectID);
        }

        public static void MakePublic(RelationshipEvent relationshipEvent)
        {
            MakePublic((ISecurityObjectId)relationshipEvent);
        }

        public static void SetAccessTo(Contact contact, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)contact, subjectID);
        }

        public static void MakePublic(Contact contact)
        {
            MakePublic((ISecurityObjectId)contact);
        }

        public static void SetAccessTo(Cases cases, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)cases, subjectID);
        }

        public static void MakePublic(Cases cases)
        {
            MakePublic((ISecurityObjectId)cases);
        }


        public static bool IsPrivate(Deal deal)
        {
            return IsPrivate((ISecurityObjectId)deal);
        }

        public static bool IsPrivate(RelationshipEvent relationshipEvent)
        {

            return IsPrivate((ISecurityObjectId)relationshipEvent);
        }

        public static bool IsPrivate(Contact contact)
        {
            return IsPrivate((ISecurityObjectId)contact);
        }

        public static bool IsPrivate(Cases cases)
        {
            return IsPrivate((ISecurityObjectId)cases);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Deal deal)
        {
            return GetAccessSubjectTo((ISecurityObjectId)deal);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(RelationshipEvent relationshipEvent)
        {
            return GetAccessSubjectTo((ISecurityObjectId)relationshipEvent);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Contact contact)
        {
            return GetAccessSubjectTo((ISecurityObjectId)contact);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Cases cases)
        {
            return GetAccessSubjectTo((ISecurityObjectId)cases);
        }

        public static void DemandAccessTo(Deal deal)
        {
            if (!CanAccessTo(deal)) throw CreateSecurityException();
        }

        public static void DemandAccessTo(RelationshipEvent relationshipEvent)
        {
            if (!CanAccessTo(relationshipEvent)) throw CreateSecurityException();
        }

    
        public static void DemandAccessTo(Contact contact)
        {
            if (!CanAccessTo(contact)) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task)
        {
            if (!CanEdit(task)) throw CreateSecurityException();
        }

        public static void DemandEdit(ListItem listItem)
        {
            if (!CanEdit(listItem)) throw CreateSecurityException();
        }
        

        public static void DemandEdit(File file)
        {
            if (!CanEdit(file)) throw CreateSecurityException();
        }



        public static void DemandAccessTo(Cases cases)
        {
            if (!CanAccessTo(cases)) throw CreateSecurityException();
        }

        public static void DemandAccessTo(File file)
        {

         //   if (!CanAccessTo((File)file)) CreateSecurityException();

        }

        public static Exception CreateSecurityException()
        {
            throw new SecurityException("Access denied.");
        }

        public static bool IsAdmin
        {
            get
            {
                return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID) ||
                    WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, SecurityContext.CurrentAccount.ID);
            }
        }
    }
}