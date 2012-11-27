#region Import

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ASC.Api.Attributes;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Core;
using ASC.CRM.Core.Entities;

#endregion

namespace ASC.Api.Documents
{
    public partial class DocumentsApi
    {

        #region Members

        private DaoFactory _crmDaoFactory;

        #endregion

        #region Methods

        /// <summary>
        /// Gets root folder for CRM files
        /// </summary>
        /// <short>CRM Root Folder</short>
        /// <category>CRM Integration</category>
        /// <returns>CRM root folder Id</returns>
        [Read("crmdoc/root")]
        public int GetCRMRootFolder()
        {
            if (_crmDaoFactory == null)
                _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                CRMConstants.DatabaseId);

            return  _crmDaoFactory.GetFileDao().GetRoot();
            
        }

        /// <summary>
        /// Gets CRM files associated with contact of given type
        /// </summary>
        /// <short>Get CRM files by contact</short>
        /// <category>CRM Integration</category>
        /// <param name="contactID">Id of contact</param>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityID">Id of entity</param>
        /// <returns>List of files</returns>
        [Read("crmdoc")]
        public IEnumerable<FileWrapper> GetCRMFiles(int contactID, EntityType entityType, int entityID)
        {

            if (_crmDaoFactory == null)
                _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                CRMConstants.DatabaseId);
                      
            var members = new List<int>();

            if (contactID > 0)
            {
                members.Add(contactID);

                if (_crmDaoFactory.GetContactDao().GetByID(contactID) is Company)
                 members.AddRange(_crmDaoFactory.GetContactDao().GetMembers(contactID).Select(item => item.ID));

            }

            var answer = _crmDaoFactory.GetRelationshipEventDao().GetAllFiles(members.ToArray(), entityType, entityID)
                .ConvertAll(file => new FileWrapper(file));

            return answer;
        }

        /// <summary>
        /// Attach files to CRM entities
        /// </summary>
        /// <short>Attach CRM files</short>
        /// <category>CRM Integration</category>
        /// <param name="contactID">Id of contact</param>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityID">Id of entity</param>
        /// <param name="fileIDs">Id's of files to attach</param>
        /// <returns>WTF?! What does it returns???!! An int??</returns>
        [Create("crmdoc")]
        public int AttachCRMFiles(int contactID, EntityType entityType, int entityID, IEnumerable<int> fileIDs)
        {
            if (_crmDaoFactory == null)
                _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                CRMConstants.DatabaseId);

          return  _crmDaoFactory.GetRelationshipEventDao().AttachFiles(contactID, entityType, entityID, fileIDs.ToArray());

        }

        /// <summary>
        /// Remove association between file and CRM entity
        /// </summary>
        /// <short>Detach CRM files</short>
        /// <category>CRM Integration</category>
        /// <param name="fileID"></param>
        [Delete(@"crmdoc")]
        public void DeleteCRMFile(int fileID)
        {
            if (_crmDaoFactory == null)
                _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                CRMConstants.DatabaseId);

            _crmDaoFactory.GetRelationshipEventDao().RemoveFile(fileID);
        } 

        #endregion

    }
}
