using System;
using System.Linq;
using System.Web;

namespace ASC.Web.Core.Security.Ajax
{
    public abstract class AjaxSecurityAttribute:Attribute
    {
        public abstract bool CheckAuthorization(HttpContext context);
    }

    public class AjaxSecurityPassthroughAttribute : AjaxSecurityAttribute
    {
        public override bool CheckAuthorization(HttpContext context)
        {
            return true;//Always authorized
        }
    }

}