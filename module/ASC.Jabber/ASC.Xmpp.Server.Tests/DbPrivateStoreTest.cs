using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
    public class DbPrivateStoreTest : DbBaseTest
	{
		private DbPrivateStore store;

        public DbPrivateStoreTest()
		{
			store = new DbPrivateStore();
            store.Configure(GetConfiguration());
		}

		[Test]
		public void PrivateStoreTest()
		{
            var jid = new Jid("n", "s", "r");

			var el = new Vcard();
			el.Fullname = "x";
            store.SetPrivate(jid, el);

			var el2 = (Vcard)store.GetPrivate(jid, new Vcard());
			Assert.AreEqual(el.Fullname, el2.Fullname);

			var el3 = store.GetPrivate(new Jid("n2", "s", "r"), new Vcard());
			Assert.IsNull(el3);

            var el4 = store.GetPrivate(jid, new Roster());
            Assert.IsNull(el4);
        }
	}
}
