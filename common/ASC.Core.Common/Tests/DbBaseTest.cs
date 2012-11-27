#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Configuration;
    using NUnit.Framework;
    using log4net;
    using log4net.Config;
    using log4net.Appender;
    using log4net.Layout;

    public class DbBaseTest<TDbService>
    {
        protected TDbService Service
        {
            get;
            private set;
        }

        protected int Tenant
        {
            get;
            private set;
        }


        protected DbBaseTest()
        {
            Service = (TDbService)Activator.CreateInstance(typeof(TDbService), ConfigurationManager.ConnectionStrings["core"]);
            Tenant = 1024;

            var pattern = "%message (%property{duration} ms)     %property{sql}    %property{sqlParams}%newline";
            BasicConfigurator.Configure(new DebugAppender { Layout = new PatternLayout(pattern) });
        }
    }
}
#endif
