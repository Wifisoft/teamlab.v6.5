using System;
using System.Configuration;
using ASC.Common.Data;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace ASC.Projects.Tests
{
    [TestFixture]
    public class TestBase : IDisposable
    {
        protected static string DbId = "test";

        protected DbManager Db
        {
            get;
            private set;
        }

        protected ILog Log
        {
            get;
            private set;
        }

        protected TestBase()
        {
            if (!DbRegistry.IsDatabaseRegistered(DbId))
            {
                DbRegistry.RegisterDatabase(DbId, ConfigurationManager.ConnectionStrings[DbId]);
            }
            Db = new DbManager(DbId);

            XmlConfigurator.Configure();
            Log = LogManager.GetLogger("ASC.Projects.Tests");
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
