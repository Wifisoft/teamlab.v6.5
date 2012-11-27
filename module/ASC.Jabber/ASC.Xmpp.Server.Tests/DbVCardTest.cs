using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
	public class DbVCardTest:DbBaseTest
	{
		private DbVCardStore store;

        public DbVCardTest()
		{
			store = new DbVCardStore();
            store.Configure(GetConfiguration());
        }

		[Test]
		public void VCardTest()
		{
			var jid = new Jid("jid1", "s", "R1");
            var vcard = new Vcard();
            vcard.JabberId = jid;

            store.SetVCard(jid, vcard);
            var v = store.GetVCard(jid);
            Assert.IsNotNull(v);
            Assert.AreEqual(vcard.JabberId, v.JabberId);

            v = store.GetVCard(new Jid("jid2", "s", "R1"));
            Assert.IsNull(v);
		}
	}
}
