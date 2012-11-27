using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Users;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
    [TestFixture]
    public class DbUserTest : DbBaseTest
    {
        private DbUserStore store;

        public DbUserTest()
        {
            store = new DbUserStore();
            store.Configure(GetConfiguration());
        }

        [Test]
        public void UserTest()
        {
            var jid = new Jid("jid1", "s", "R1");
            var user = new User(jid, "1", true);

            store.SaveUser(user);
            var u = store.GetUser(jid);
            Assert.IsNotNull(u);
            Assert.AreEqual(user.Password, u.Password);
            Assert.AreEqual(user.IsAdmin, u.IsAdmin);
            Assert.AreEqual(user.Jid.Bare, u.Jid.Bare);

            u = store.GetUser(new Jid("jid2", "s", "R1"));
            Assert.IsNull(u);

            CollectionAssert.IsEmpty(store.GetUsers("s2"));
            foreach (var us in store.GetUsers("s"))
            {
                store.RemoveUser(us.Jid);
            }
            CollectionAssert.IsEmpty(store.GetUsers("s"));
        }
    }
}
