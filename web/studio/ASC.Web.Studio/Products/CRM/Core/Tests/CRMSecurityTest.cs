#region Import
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using ASC.CRM.Core.Entities;
using ASC.Common.Data;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Files.Core.Data;
using ASC.Web.CRM.Classes;
using NUnit.Framework;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.CRM.Core.Tests
{
    [TestFixture]
    public class CRMSecurityTest
    {
        [Test]
        public void SetAccessToTest()
        {
            if (!DbRegistry.IsDatabaseRegistered(CRMConstants.DatabaseId))
                DbRegistry.RegisterDatabase(CRMConstants.DatabaseId,"Server=teamlab;Database=Test;UserID=dev;Pwd=dev;pooling=True;Character Set=utf8");


            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);

           
            var deal =  Global.DaoFactory.GetDealDao().GetByID(17);

            CRMSecurity.SetAccessTo(deal, new List<Guid>()
                                              {
                                                  ASC.Core.SecurityContext.CurrentAccount.ID
                                              });


        }
    }
}