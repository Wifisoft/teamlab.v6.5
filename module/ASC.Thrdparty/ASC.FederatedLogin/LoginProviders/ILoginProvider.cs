using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty;

namespace ASC.FederatedLogin.LoginProviders
{
    public interface ILoginProvider
    {
        LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string,string> @params);
    }
}