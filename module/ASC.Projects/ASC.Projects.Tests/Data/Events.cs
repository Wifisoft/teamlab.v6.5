using System;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Events : TestBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void SaveOrUpdateTest()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var eventDao = daoFactory.GetEventDao();

            eventDao.Save(new Event { Title = "123", FromDate = DateTime.Now, ToDate = DateTime.Now });
        }
    }
}
