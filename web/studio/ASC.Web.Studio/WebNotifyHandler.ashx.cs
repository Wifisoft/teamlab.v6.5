using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio
{
    public abstract class WebNotifyHandlerConstants
    {
        public const string UserIdKey = "userId";
        public const string ActionKey = "action";
        public const string TenantIdKey = "tenantId";
        public const string ValidationKey = "key";

        public static string[] AllKeys { get { return new[] { ActionKey, TenantIdKey, UserIdKey, ValidationKey }; } }
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WebNotifyHandler : IHttpHandler
    {
        public static event EventHandler<WebNotifyActionEventArgs> NotifyAction = null;

        public void OnNotifyAction(WebNotifyActionEventArgs e)
        {
            EventHandler<WebNotifyActionEventArgs> handler = NotifyAction;
            if (handler != null) handler(this, e);
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var notifyParams =new NameValueCollection(context.Request.Form);
                notifyParams.Add(context.Request.QueryString);

                if (!WebNotifyHandlerConstants.AllKeys.All(x => notifyParams.AllKeys.Contains(x)))
                    throw new ArgumentOutOfRangeException("context","No nessary fields to accomplish operation");

                var action = notifyParams.GetValues(WebNotifyHandlerConstants.ActionKey).SingleOrDefault();
                var tenant = notifyParams.GetValues(WebNotifyHandlerConstants.TenantIdKey).SingleOrDefault();
                var userId = new Guid(notifyParams.GetValues(WebNotifyHandlerConstants.UserIdKey).SingleOrDefault());
                var validateKey = notifyParams.GetValues(WebNotifyHandlerConstants.ValidationKey).SingleOrDefault();

                int tenantId;

                if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(validateKey) && int.TryParse(tenant, out tenantId))
                {
                    var validInterval = SetupInfo.ValidEamilKeyInterval;
                    //TODO: think. maybe better to sign whole request string
                    var keyToValidate = string.Join("|", new[] { action, tenantId.ToString(CultureInfo.InvariantCulture), userId.ToString("N") });
                    var checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(keyToValidate, validateKey, validInterval);
                    if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Ok)
                    {
                        //Remove unnessesary params
                        notifyParams.Remove(WebNotifyHandlerConstants.ActionKey);
                        notifyParams.Remove(WebNotifyHandlerConstants.TenantIdKey);
                        notifyParams.Remove(WebNotifyHandlerConstants.UserIdKey);

                        CoreContext.TenantManager.SetCurrentTenant(tenantId);
                        if (CoreContext.UserManager.UserExists(userId))
                        {
                            OnNotifyAction(new WebNotifyActionEventArgs(action, notifyParams,
                                                                        CoreContext.UserManager.GetUsers(userId)));
                        }
                        else
                        {
                            throw new ArgumentException("User doesn't exists");
                        }
                    }
                    else if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Invalid)
                    {
                        throw new ArgumentException("Validation key invalid");
                    }
                    else if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Expired)
                    {
                        throw new ArgumentException("Validation key expired");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid parameters");
                }
            }
            catch (Exception e)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Bad request", e);
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class WebNotifyActionCaller
    {
        private readonly Uri _notifyUri;
        private readonly string _action;
        private readonly Guid _userId;
        private readonly int _currentTenantId;

        public WebNotifyActionCaller(Uri notifyUri, string action)
            : this(notifyUri, action, SecurityContext.CurrentAccount.ID)
        {
        }

        public WebNotifyActionCaller(Uri notifyUri, string action, Guid userId)
            : this(notifyUri, action, userId, CoreContext.TenantManager.GetCurrentTenant().TenantId)
        {
        }

        public WebNotifyActionCaller(Uri notifyUri, string action, Guid userId, int currentTenantId)
        {
            if (notifyUri == null) throw new ArgumentNullException("notifyUri");
            if (action == null) throw new ArgumentNullException("action");

            _notifyUri = notifyUri;
            _action = action;
            _userId = userId;
            _currentTenantId = currentTenantId;
        }

        public Uri GetUri(IDictionary<string, string> additionalParam)
        {
            var builder = new UriBuilder(_notifyUri);
            //Form query string
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("{1}={0}&", Uri.EscapeDataString(_action), WebNotifyHandlerConstants.ActionKey);
            queryBuilder.AppendFormat("{1}={0}&", _userId.ToString("N"), WebNotifyHandlerConstants.UserIdKey);
            queryBuilder.AppendFormat("{1}={0}&", _currentTenantId.ToString(CultureInfo.InvariantCulture),
                                      WebNotifyHandlerConstants.TenantIdKey);
            if (additionalParam != null)
            {
                foreach (var param in additionalParam)
                {
                    queryBuilder.AppendFormat("{1}={0}&", Uri.EscapeDataString(param.Value), param.Key);
                }
            }
            //Form key
            var key =
                string.Join("|",new[]{_action, _currentTenantId.ToString(CultureInfo.InvariantCulture),_userId.ToString("N")});

            queryBuilder.AppendFormat("{1}={0}", EmailValidationKeyProvider.GetEmailKey(key), WebNotifyHandlerConstants.ValidationKey);
            builder.Query = queryBuilder.ToString();
            return builder.Uri;
        }

    }

    public class WebNotifyActionEventArgs : EventArgs
    {
        public string Action { get; private set; }
        private readonly NameValueCollection _values;

        public UserInfo User { get; private set; }

        public WebNotifyActionEventArgs(string action, NameValueCollection values, UserInfo user)
        {
            Action = action;
            _values = values;
            User = user;
        }

        /// <summary>
        /// Returns additional values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">value key</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public T GetValue<T>(string key)
        {
            var value = _values[key];

            if (string.IsNullOrEmpty(value))
                throw new KeyNotFoundException("Key '" + key + "' not found");
            var converter = TypeDescriptor.GetConverter(typeof (T));
            if (converter==null)
                throw new ArgumentException("No converter '" + key + "' to type "+typeof(T).FullName);
            if (!converter.CanConvertFrom(typeof(string)))
                throw new ArgumentException("Can't convert '" + key + "' to type " + typeof(T).FullName);
            
            return (T)converter.ConvertFromString(value);
        }
    }
}
