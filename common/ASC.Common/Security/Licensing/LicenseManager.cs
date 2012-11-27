using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using log4net;

namespace ASC.Common.Security.Licensing
{
    public class LicenseManager
    {
        private const string LicensingServerConfigurationKey = "asc.licensing.server";
        private static readonly object Synclock = new object();

        private static volatile byte[] _hwId;
        private static HashAlgorithm _hAlg;

        private static readonly Dictionary<string, string> HwIdKeys = new Dictionary<string, string>
                                                                          {
                                                                              {"ProcessorId", "Win32_Processor"}
                                                                          };

        private static volatile RSACryptoServiceProvider _rsaCrypto;
        private static readonly List<Timer> Timers = new List<Timer>();
        private static License _license;
        private static readonly ILog Log;

        static LicenseManager()
        {
            Log = LogManager.GetLogger("ASC.Licensing");
        }

        private static void InitLicense()
        {
            InitLicense(false);
        }

        private static void InitLicense(bool force)
        {
            if (_license == null || force)
            {
                //Do request
                ConfigurationManager.RefreshSection("appSettings");
                var serverAddress = ConfigurationManager.AppSettings[LicensingServerConfigurationKey];
                Uri serverUri;
                if (string.IsNullOrEmpty(serverAddress) || !Uri.IsWellFormedUriString(serverAddress, UriKind.Absolute) || !Uri.TryCreate(serverAddress, UriKind.Absolute, out serverUri)) //TODO: We can also check domain here
                {
                    throw new ArgumentException("server address is malformed");
                }
                Log.InfoFormat("using license server: {0}", serverAddress);
                WebRequest request = WebRequest.Create(serverUri);
                request.Method = "POST";
                request.ContentType = "application/octet-stream";
                request.ContentLength = _hwId.Length;
                request.Timeout = 90000; //5 sec timeout
                request.GetRequestStream().Write(_hwId, 0, _hwId.Length);

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        var signHeader = response.Headers["X-License-Sign"];
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var origin = streamReader.ReadToEnd();
                            var scriptSerializer = new JavaScriptSerializer();
                            _license = scriptSerializer.Deserialize<License>(origin);

                            //Serialize back and check
                            string verify = scriptSerializer.Serialize(_license);
                            if (!_rsaCrypto.VerifyHash(_hAlg.ComputeHash(Encoding.UTF8.GetBytes(verify)), null,
                                                       HttpServerUtility.UrlTokenDecode(signHeader)))
                            {
                                Log.Error("Hash verification failed");
                                _license = null; //Invalid
                            }
                            else
                            {
                                foreach (Timer timer in Timers)
                                {
                                    timer.Dispose();
                                }
                                Timers.Clear();

                                if (_license.RestartAt.HasValue)
                                {
                                    //Schedule restart
                                    TimeSpan restart = (_license.RestartAt.Value - _license.TimeNow);
                                    Timers.Add(new Timer((x) => AppDomain.Unload(AppDomain.CurrentDomain), null, restart,
                                                         restart));
                                }
                                if (_license.RefreshAt.HasValue)
                                {
                                    //Schedule restart
                                    TimeSpan refresh = (_license.RefreshAt.Value - _license.TimeNow);

                                    Timers.Add(new Timer((x) => InitLicense(force), null, refresh,
                                                         refresh));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("License server error", e);
                }
            }
        }

        private static void InitRsa()
        {
            if (_rsaCrypto == null)
            {
                lock (Synclock)
                {
                    if (_rsaCrypto == null)
                    {
                        _rsaCrypto = GetCryptoConfig();
                    }
                }
            }
        }

        private static RSACryptoServiceProvider GetCryptoConfig()
        {
            //Use our PK as config
            _hAlg = SHA1.Create();
            return EncryptionUtils.GetPublicKeyFromAssembly(typeof(LicenseManager).Assembly);
        }

        private static void IniHwid()
        {
            if (_hwId == null)
            {
                lock (Synclock)
                {
                    if (_hwId == null)
                    {
                        _hwId = GetHardwareId();
                    }
                }
                Log.InfoFormat("HwId is {0}", Convert.ToBase64String(_hwId));
            }
        }

        private static byte[] GetHardwareId()
        {
            //Get hw id
            KeyValuePair<string, string>[] values = (from hwIdKey in HwIdKeys
                                                     select
                                                         new ManagementObjectSearcher(
                                                         string.Format("select {1} from {0}", hwIdKey.Value, hwIdKey.Key))
                                                         into searcher
                                                         from ManagementObject share in searcher.Get()
                                                         from PropertyData propData in share.Properties
                                                         where !propData.IsArray && propData.Value != null
                                                         select
                                                             new KeyValuePair<string, string>(propData.Name,
                                                                                              propData.Value.ToString()))
                .ToArray();
            return
                EncryptionUtils.AsymmetricEncrypt(
                    Encoding.UTF8.GetBytes(string.Join("&",
                                                       values.Select(x => string.Format("{0}={1}", HttpUtility.UrlEncode(x.Key), HttpUtility.UrlEncode(x.Value))).
                                                           ToArray())), _rsaCrypto);
        }

        public static bool ValidateLicense(string[] features)
        {
#if (LICENSING)
            try
            {
                InitRsa();
                IniHwid();
                InitLicense();

                bool licenceValid = _license != null && DateTime.UtcNow < _license.ValidTo && LicenseHasFeatures(features);
                return licenceValid;
            }
            catch (Exception e)
            {
                Log.Error("License validation failed",e);
                return false;
            }
#else
            return true;
#endif
        }

        public static object GetLicenseFeature(string feature)
        {
            return ValidateLicense(new[] { feature }) ? _license.Params[feature] : null;
        }

        private static bool LicenseHasFeatures(string[] features)
        {
            if (_license != null && features != null && features.Any())
            {
                return features.All(x => _license.Params.ContainsKey(x) && _license.Params[x] != null);
            }
            return true;
        }
    }
}