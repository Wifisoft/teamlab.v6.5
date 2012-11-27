#region Import

using System;
using System.Collections;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Data;
using ASC.Projects.Engine;
using log4net;
using NUnit.Framework;

#endregion

namespace ASC.Projects.Tests.Data
{
    public class Search : TestBase
    {        
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void SearchTest()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);

            IDaoFactory daoFactory = new DaoFactory("projects", 0);
            EngineFactory engineFactory = new Engine.EngineFactory("projects", 0, null);
           
            IList searchDao = engineFactory.GetSearchEngine().Search("p", 0);

            Console.WriteLine(searchDao.Count);
        }

    }
}
