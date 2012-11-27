using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace ASC.Web.Core.Security
{
    public class AntiXssModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        private static readonly List<string> SkipFields = new List<string>() { "__VIEWSTATE", "__EVENTVALIDATION", "__EVENTTARGET" };

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequestHandler;
        }

        private static void BeginRequestHandler(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Form.Count>0)
            {
                //HACK: Unlock form for writing. Bad hack
                var writableMethod = HttpContext.Current.Request.Form.GetType().GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance);
                var readOnlyMethod = HttpContext.Current.Request.Form.GetType().GetMethod("MakeReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                var formField = HttpContext.Current.Request.GetType().GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance);
                writableMethod.Invoke(HttpContext.Current.Request.Form, null);
                //Filter form values
                foreach (string param in HttpContext.Current.Request.Form.AllKeys)
                {
                    if (!SkipFields.Contains(param))
                    {
                        var paramvalue = HttpContext.Current.Request.Form.Get(param);
                        //Clean it. Collection is read only
                        HttpContext.Current.Request.Form.Set(param,HtmlSanitizer.Sanitize(paramvalue));
                    }
                }
                formField.SetValue(HttpContext.Current.Request, HttpContext.Current.Request.Form);
                readOnlyMethod.Invoke(HttpContext.Current.Request.Form, null);
            }


        }

        #endregion

    }
}
