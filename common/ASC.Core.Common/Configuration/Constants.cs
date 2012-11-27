using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Configuration
{
    public sealed class Constants
    {
        public static readonly string NotifyEMailSenderSysName = "email.sender";

        public static readonly string NotifyMessengerSenderSysName = "messanger.sender";


        public static readonly ISystemAccount CoreSystem = new SystemAccount(new Guid("A37EE56E-3302-4a7b-B67E-DDBEA64CD032"), "asc system", true);

        public static readonly ISystemAccount Guest = new SystemAccount(new Guid("712D9EC3-5D2B-4b13-824F-71F00191DCCA"), "guest", false);

        public static readonly ISystemAccount Demo = new SystemAccount(ASC.Common.Security.Authorizing.Constants.Demo.ID, ASC.Common.Security.Authorizing.Constants.Demo.Name, true);

        public static readonly IPrincipal Anonymous = new GenericPrincipal(Guest, new[] { Role.Everyone });

        public static readonly ISystemAccount[] SystemAccounts = new[]
        {
            Demo,
            CoreSystem,
            Guest,
        };
    }
}
