#if DEBUG
using ASC.Common.Utils;
using NUnit.Framework;

namespace ASC.Common.Tests.Utils
{
    [TestFixture]
    public class MimeHeaderUtilsTest
    {
        [Test]
        public void Encode()
        {
            Assert.AreEqual("=?utf-8?B?0YrRitGK?=", MimeHeaderUtils.EncodeMime("ъъъ"));
            Assert.AreEqual("ddd", MimeHeaderUtils.EncodeMime("ddd"));
        }
    }
}
#endif