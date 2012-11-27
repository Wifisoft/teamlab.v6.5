﻿#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using ASC.Core.Security.Authentication;
    using NUnit.Framework;

    [TestFixture]
    public class CookieStorageTest
    {
        [Test]
        public void Validate()
        {
            var t1 = 1;
            var id1 = Guid.NewGuid();
            var login1 = "l1";
            var pwd1 = "p1";

            var cookie = CookieStorage.EncryptCookie(t1, id1, login1, pwd1);

            int t2;
            Guid id2;
            string login2;
            string pwd2;

            CookieStorage.DecryptCookie(cookie, out t2, out id2, out login2, out pwd2);

            Assert.AreEqual(t1, t2);
            Assert.AreEqual(id1, id2);
            Assert.AreEqual(login1, login2);
            Assert.AreEqual(pwd1, pwd2);
        }
    }
}
#endif
