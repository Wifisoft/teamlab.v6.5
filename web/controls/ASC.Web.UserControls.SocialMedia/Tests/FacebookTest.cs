#if DEBUG
using NUnit.Framework;
using ASC.SocialMedia.Facebook;

namespace ASC.SocialMedia.Tests
{
    [TestFixture]
    public class FacebookTest
    {
        [Test]
        public void GetUrlOfUserImageTest()
        {
            var ai = new FacebookApiInfo { AccessToken = "186245251433148|5fecd56abddd9eb63b506530.1-100002072952328|akD66RBlkeedQmhy50T9V_XCTYs" };
            var provider = new FacebookDataProvider(ai);
            var url = provider.GetUrlOfUserImage("kamorin.roman", FacebookDataProvider.ImageSize.Original);
        }
    }
}
#endif