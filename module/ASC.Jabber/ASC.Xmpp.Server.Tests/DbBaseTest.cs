using System.Collections.Generic;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
    [TestFixture]
    public class DbBaseTest
    {
        public IDictionary<string, string> GetConfiguration()
        {
            var props = new Dictionary<string, string>();
            props["connectionString"] = "Data Source=|DataDirectory|\\test.db3;Version=3";
            //props["connectionStringName"] = "mysql";
            return props;
        }
    }
}
