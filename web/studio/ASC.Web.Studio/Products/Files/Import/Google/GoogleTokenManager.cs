using ASC.Thrdparty;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Web.Files.Import.Google
{
    internal class GoogleTokenManager
    {
        internal static IConsumerTokenManager TokenManager
        {
            get
            {
                return TokenManagerHolder.Get("google", "googleConsumerKey", "googleConsumerSecret");
            }
        }
    }
}