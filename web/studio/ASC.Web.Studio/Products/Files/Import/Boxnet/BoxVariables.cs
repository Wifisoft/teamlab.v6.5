using ASC.Thrdparty;

namespace ASC.Web.Files.Import.Boxnet
{
    internal class BoxVariables
    {
        internal static string AuthToken
        {
            get { return TokenHolder.GetToken("box.net_auth_token"); }
            set { TokenHolder.AddToken("box.net_auth_token", value); }
        }

        internal static string AuthTicket
        {
            get { return TokenHolder.GetToken("box.net_auth_ticket"); }
            set { TokenHolder.AddToken("box.net_auth_ticket", value); }
        }
    }
}