using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Studio.Core.Notify
{
    class NotifyConfiguration
    {
        static Regex urlReplacer = new Regex(@"(<a [^>]*href=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^>]*>)|(<img [^>]*src=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^/>]*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static readonly Regex TextileLinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);

        static object GetCurrentProductUrl()
        {
            var product = ProductManager.Instance[CommonLinkUtility.GetProductID()];
            if (product == null)
                return CommonLinkUtility.GetFullAbsolutePath("~");
            else
                return product.StartURL;
        }

        static object GetRecipientSubscriptionConfigURL()
        {
            return CommonLinkUtility.GetFullAbsolutePath(
                        CommonLinkUtility.GetMyStaff(MyStaffType.Subscriptions));
        }


        static ITagValue[] _CommonTags = new ITagValue[] { 
            //__VirtualRootPath
            new DynamicTagValue(CommonTags.VirtualRootPath, ()=>(CommonLinkUtility.GetFullAbsolutePath("~")??"").TrimEnd('/')), 
            //__ProductID
            new DynamicTagValue(CommonTags.ProductID,()=>CommonLinkUtility.GetProductID()),
            //__ProductUrl
            new DynamicTagValue(CommonTags.ProductUrl,GetCurrentProductUrl),
            //__DateTime
            new DynamicTagValue(CommonTags.DateTime,()=>TenantUtil.DateTimeNow()),

            new TagValue(CommonTags.Helper, new PatternHelper()),
            //__AuthorID
            //          add to NotifyEngine_BeforeTransferRequest
            //__AuthorName
            //          add to NotifyEngine_BeforeTransferRequest
            //__AuthorUrl
            //          add to NotifyEngine_BeforeTransferRequest
            //__RecipientID
            new TagValue(CommonTags.RecipientID,Context.SYS_RECIPIENT_ID),
            //RecipientSubscriptionConfigURL
            new DynamicTagValue(CommonTags.RecipientSubscriptionConfigURL, GetRecipientSubscriptionConfigURL)
        };


        public static void NotifyClientRegisterCallback(Context context, INotifyClient client)
        {
            client.SetStaticTags(_CommonTags);

            client.AddInterceptor(
                    new SendInterceptorSkeleton(
                        "Web.UrlAbsoluter",
                        InterceptorPlace.MessageSend,
                        InterceptorLifetime.Global,
                        (nreq, place) =>
                        {
                            if (nreq != null && nreq.CurrentMessage != null && nreq.CurrentMessage.ContentType == Pattern.HTMLContentType)
                                DoNotifyRequestAbsoluteUrl(nreq.CurrentMessage);
                            return false;
                        }
                        )
                    );

            var productSecurityAndCulture = new SendInterceptorSkeleton(
                "ProductSecurityInterceptor",
                 InterceptorPlace.DirectSend,
                 InterceptorLifetime.Global,
                 (r, p) =>
                 {
                     var u = ASC.Core.Users.Constants.LostUser;
                     try
                     {
                         if (32 <= r.Recipient.ID.Length)
                         {
                             var guid = default(Guid);
                             try
                             {
                                 guid = new Guid(r.Recipient.ID);
                             }
                             catch (FormatException) { }
                             catch (OverflowException) { }

                             if (guid != default(Guid))
                             {
                                 u = CoreContext.UserManager.GetUsers(guid);
                             }
                         }

                         if (ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByEmail(r.Recipient.ID);
                         }

                         if (ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             u = CoreContext.UserManager.GetUserByUserName(r.Recipient.ID);
                         }

                         if (!ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             var culture = !string.IsNullOrEmpty(u.CultureName) ?
                                 u.GetCulture() :
                                 CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                             Thread.CurrentThread.CurrentCulture = culture;
                             Thread.CurrentThread.CurrentUICulture = culture;
                         }
                     }
                     catch (Exception error)
                     {
                         LogManager.GetLogger(typeof(WorkContext)).Error(error);
                     }

                     if (r.Properties.ContainsKey("asc.web.product_id"))
                     {
                         var pid = (Guid)r.Properties["asc.web.product_id"];
                         var userid = Guid.Empty;
                         try
                         {
                             userid = new Guid(r.Recipient.ID);
                         }
                         catch { }
                         if (pid != Guid.Empty && !ASC.Core.Users.Constants.LostUser.Equals(u))
                         {
                             return !WebItemSecurity.IsAvailableForUser(pid.ToString(), u.ID);
                         }
                     }
                     return false;
                 });
            client.AddInterceptor(productSecurityAndCulture);
        }


        internal static void Configure()
        {
            WorkContext.NotifyContext.NotifyClientRegistration += NotifyConfiguration.NotifyClientRegisterCallback;

            WorkContext.NotifyContext.NotifyEngine.BeforeTransferRequest += NotifyEngine_BeforeTransferRequest;
            WorkContext.NotifyContext.NotifyEngine.AfterTransferRequest += NotifyEngine_AfterTransferRequest;

            //Register what's new
            StudioNotifyService.Instance.RegisterSendMethod();
        }

        static void NotifyEngine_AfterTransferRequest(ASC.Notify.Engine.NotifyEngine sender, ASC.Notify.Engine.NotifyRequest request)
        {
            var productID = (Guid)request.Properties["asc.web.product_id"];
            if (productID != Guid.Empty)
            {
                CallContext.SetData("asc.web.product_id", productID);
            }
        }

        static void NotifyEngine_BeforeTransferRequest(ASC.Notify.Engine.NotifyEngine sender, ASC.Notify.Engine.NotifyRequest request)
        {
            request.Properties.Add("asc.web.product_id", CommonLinkUtility.GetProductID());

            Guid aid = Guid.Empty;
            string aname = "";
            if (SecurityContext.IsAuthenticated)
            {
                aid = SecurityContext.CurrentAccount.ID;
                if (CoreContext.UserManager.UserExists(aid))
                    aname = CoreContext.UserManager.GetUsers(aid).DisplayUserName();
            }

            //__AuthorID
            request.Arguments.Add(new TagValue(CommonTags.AuthorID, aid));
            //__AuthorName
            request.Arguments.Add(new TagValue(CommonTags.AuthorName, aname));
            //__AuthorUrl
            request.Arguments.Add(new TagValue(CommonTags.AuthorUrl, CommonLinkUtility.GetUserProfile(aid, CommonLinkUtility.GetProductID())));

            if (!request.Arguments.Any(x => CommonTags.SendFrom.Equals(x.Tag.Name)))//If none add current
                request.Arguments.Add(new TagValue(CommonTags.SendFrom, CoreContext.TenantManager.GetCurrentTenant(false).Name));

        }


        static void DoNotifyRequestAbsoluteUrl(INoticeMessage msg)
        {
            var body = msg.Body;


            body = urlReplacer.Replace(body, (m =>
            {
                var url = m.Groups["url"].Value;
                var ind = m.Groups["url"].Index - m.Index;
                if (String.IsNullOrEmpty(url) && ind > 0)
                    return (m.Value).Insert(ind, CommonLinkUtility.GetFullAbsolutePath(""));
                return (m.Value).Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
            }));

            body = TextileLinkReplacer.Replace(body, (m =>
            {
                var url = m.Groups["link"].Value;
                var ind = m.Groups["link"].Index - m.Index;
                if (String.IsNullOrEmpty(url) && ind > 0)
                    return (m.Value).Insert(ind, CommonLinkUtility.GetFullAbsolutePath(""));
                return (m.Value).Replace(url, CommonLinkUtility.GetFullAbsolutePath(url));
            }));

            msg.Body = body;
        }
    }
}
