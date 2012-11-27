using System.Diagnostics;
using ASC.Xmpp.Common;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	//[TestFixture]
	public class JabberServiceClientTest
	{
		private JabberServiceClient jabberClient;

		[TestFixtureSetUp]
		public void SetUp()
		{
			jabberClient = new JabberServiceClient();
		}

		[Test]
		public void AvailableTest()
		{
			Assert.IsTrue(jabberClient.IsAvailable());
		}

		[Test]
		public void CurrentUserNameTest()
		{
			Assert.AreEqual("nikolay.ivanov", jabberClient.GetCurrentUserName());
		}

		[Test]
		public void GetNewMessagesCountTest()
		{
			Assert.AreEqual(0, jabberClient.GetNewMessagesCount("nikolay.ivanov",0));
		}

		[Test]
		public void GetAuthTokenTest()
		{
			var token = jabberClient.GetAuthToken(0);
			Debug.WriteLine(string.Format("Success get authentication token from jabber service. Token is {0}", token));
		}

		[Test]
		public void GetUserAvailable()
		{
			var available = jabberClient.IsUserAvailable("Nikolay.Ivanov",0);
			available = jabberClient.IsUserAvailable("Nikolay.Ivanov",0);
			available = jabberClient.IsUserAvailable("Nikolay.Petrov",0);
		}
	}
}
