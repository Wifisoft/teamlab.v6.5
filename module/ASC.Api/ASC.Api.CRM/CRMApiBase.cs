#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.CRM.Core.Dao;
using ASC.Core;
using ASC.CRM.Core;
using ASC.Files.Core;

#endregion

namespace ASC.Api.CRM
{
    public class CRMApiBase
    {
        private DaoFactory _crmDaoFactory;
        private IDaoFactory _filesDaoFactory;

        protected DaoFactory DaoFactory
        {

            get
            {
                if (_crmDaoFactory == null)
                    _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                    CRMConstants.DatabaseId);

                return _crmDaoFactory;
            }
        }

        protected IDaoFactory FilesDaoFactory
        {

            get
            {
                if (_filesDaoFactory == null)
                    _filesDaoFactory = new Files.Core.Data.DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                       FileConstant.DatabaseId);

                return _filesDaoFactory;
            }
        }    


    }
}
