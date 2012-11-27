
using System.Text;
using ASC.Notify.Messages;

#if (DEBUG)
namespace ASC.Notify.Textile
{
    using NUnit.Framework;

    [TestFixture]
    public class StylerTests
    {
        private string _pattern;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _pattern = "h1.New Post in Forum Topic: \"Project(%: \"Sample Title\"\":\"http://teamlab.com\"" + System.Environment.NewLine +
            "25/1/2022 \"Jim\":\"http://teamlab.com/my.aspx\"" + System.Environment.NewLine +
            "has created a new post in topic:" + System.Environment.NewLine +
            "<b>The text!&nbsp;</b>" + System.Environment.NewLine +
            "\"Read More\":\"http://teamlab.com/forum/post?id=4345\"" + System.Environment.NewLine +
            "Your portal address: \"http://teamlab.com\":\"http://teamlab.com\" " + System.Environment.NewLine +
            "\"Edit subscription settings\":\"http://teamlab.com/subscribe.aspx\"";

        }

        [Test]
        public void TestJabberStyler()
        {
            var message = new NoticeMessage() {Body = _pattern};
            new JabberStyler().ApplyFormating(message);
        }

        [Test]
        public void TestTextileStyler()
        {
            var message = new NoticeMessage() { Body = _pattern };
            new TextileStyler().ApplyFormating(message);
        }
    }
}
#endif