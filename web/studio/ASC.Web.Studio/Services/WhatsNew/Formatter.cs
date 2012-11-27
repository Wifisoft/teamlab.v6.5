using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web;
using ASC.Collections;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Studio.Utility;
using Commons.Collections;
using NVelocity;
using NVelocity.App;

namespace ASC.Web.Studio.Services.WhatsNew
{
    public class Formatter
    {
        private static readonly VelocityEngine Velocity;
        private static readonly CachedDictionary<Template> Patterns = new CachedDictionary<Template>("feed_patterns");
        private static bool _isInitialized;

        static Formatter()
        {
            Velocity = new VelocityEngine();
        }

        public static VelocityContext PrepareContext(UserActivity userActivity, UserInfo user)
        {
            var velocitycontext = new VelocityContext();
            velocitycontext.Put("activity", userActivity);
            velocitycontext.Put("url", CommonLinkUtility.GetFullAbsolutePath(userActivity.URL));
            velocitycontext.Put("user", user);
            velocitycontext.Put("displayName", user.DisplayUserName());
            velocitycontext.Put("userLink", CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(user.ID, userActivity.ProductID)));
            velocitycontext.Put("moduleName", GetModuleName(userActivity));
            velocitycontext.Put("productName", GetProductName(userActivity));
            velocitycontext.Put("additionalData", userActivity.AdditionalData);
            return velocitycontext;
        }

        public static string Format(HttpContext context, string pattern, VelocityContext velocitycontext)
        {
                using (var writer = new StringWriter())
                {
                    try
                    {
                        if (!_isInitialized)
                        {
                            var props = new ExtendedProperties();
                            props.AddProperty("file.resource.loader.path",
                                              new ArrayList(new[]
                                                                {
                                                                    ".",
                                                                    Path.Combine(
                                                                        context.Server.MapPath(feed.HandlerBasePath),
                                                                        "Patterns")
                                                                }));
                            Velocity.Init(props);
                            _isInitialized = true;
                        }
                        //Load patterns
                        var template = Patterns.Get(pattern, () => LoadTemplate(pattern));

                        template.Merge(velocitycontext, writer);
                        return writer.GetStringBuilder().ToString();

                    }
                    catch (Exception)
                    {
                        //Format failed some way
                        return writer.GetStringBuilder().ToString();
                    }
                }
        }

        private static Template LoadTemplate(string pattern)
        {
            return Velocity.GetTemplate(pattern);
        }

        private static string GetModuleName(UserActivity userActivity)
        {
            IModule module = ProductManager.Instance.GetModuleByID(userActivity.ModuleID);
            return module == null ? "Unknown module" : module.Name;
        }

        private static string GetProductName(UserActivity userActivity)
        {
            var module = ProductManager.Instance.Products.Where(x=>userActivity.ProductID == x.ID).SingleOrDefault();
            return module == null ? "Unknown module" : module.Name;
        }
    }
}