using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Web.Core
{
    public interface IHtmlInjectionProvider
    {
        string GetInjection();
    }
}
