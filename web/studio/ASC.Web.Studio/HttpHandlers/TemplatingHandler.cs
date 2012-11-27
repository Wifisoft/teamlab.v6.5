using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Xml;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Web.Studio.HttpHandlers
{
    public class TemplatingHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {
            if (String.IsNullOrEmpty(context.Request["id"]) || String.IsNullOrEmpty(context.Request["name"]))
                return;

            var TemplateName = context.Request["name"];
            var TemplatePath = context.Request["id"];
            var Template = new XmlDocument();
            try
            {
                Template.Load(context.Server.MapPath(String.Format("~{0}{1}.xsl", TemplatePath, TemplateName)));
            }
            catch (Exception)
            {
                return;
            }
            if (Template.GetElementsByTagName("xsl:stylesheet").Count == 0)
            {
                return;
            }

            var Aliases = new Dictionary<String, String>();
            var RegisterAliases = Template.GetElementsByTagName("register");
            while ((RegisterAliases = Template.GetElementsByTagName("register")).Count > 0)
            {
                var RegisterAlias = RegisterAliases.Item(0);
                if (!String.IsNullOrEmpty(RegisterAlias.Attributes["alias"].Value) && !String.IsNullOrEmpty(RegisterAlias.Attributes["type"].Value))
                {
                    Aliases.Add(RegisterAlias.Attributes["alias"].Value, RegisterAlias.Attributes["type"].Value);
                }
                RegisterAlias.ParentNode.RemoveChild(RegisterAlias);
            }

            var CurrentResources = Template.GetElementsByTagName("resource");
            while ((CurrentResources = Template.GetElementsByTagName("resource")).Count > 0)
            {
                var CurrentResource = CurrentResources.Item(0);
                if (!String.IsNullOrEmpty(CurrentResource.Attributes["name"].Value))
                {
                    var FullName = CurrentResource.Attributes["name"].Value.Split('.');
                    if (FullName.Length == 2 && Aliases.ContainsKey(FullName[0]))
                    {
                        var ResourceValue = Template.CreateTextNode(GetModuleResource(Aliases[FullName[0]], FullName[1]));
                        CurrentResource.ParentNode.InsertBefore(ResourceValue, CurrentResource);
                    }
                }
                CurrentResource.ParentNode.RemoveChild(CurrentResource);
            }

            context.Response.ContentType = "text/xml";
            context.Response.Write(Template.InnerXml);
        }


        private String GetModuleResource(String resourceClassTypeName, String resourseKey)
        {
            if (string.IsNullOrEmpty(resourseKey)) return string.Empty;
            try
            {
                var type = Type.GetType(resourceClassTypeName);

                var resManager = (ResourceManager)type.InvokeMember("resourceMan", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public, null, type, null);

                //custom
                if (!SecurityContext.IsAuthenticated)
                {
                    SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey));
                }
                var u = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                var culture = !string.IsNullOrEmpty(u.CultureName) ? CultureInfo.GetCultureInfo(u.CultureName) : CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                var value = resManager.GetString(resourseKey, culture);
                return value;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}