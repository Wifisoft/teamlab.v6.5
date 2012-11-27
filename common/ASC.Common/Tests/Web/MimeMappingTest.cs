#if DEBUG
using ASC.Common.Web;
using NUnit.Framework;

namespace ASC.Common.Tests.Web {

	[TestFixture]
	public class MimeMappingTest {

		[Test]
		public void GetExtentionTest() {
			var ext = MimeMapping.GetExtention("application/x-zip-compressed");
			Assert.AreEqual(".zip", ext);

			ext = MimeMapping.GetExtention("Application/x-zip-Compressed");
			Assert.AreEqual(".zip", ext);

			ext = MimeMapping.GetExtention("Application/ZIP");
			Assert.AreEqual(".zip", ext);
		}
	}
}
#endif