// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="License.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using ASC.Licensing.Utils;

namespace ASC.Licensing
{
    public interface ILicense
    {
        void Refresh();
        ILicenseRequest GetOfflineActivationCode(byte[] licenseCode);
        void ActivateFromOfflineCode(byte[] code);
        bool IsValid();
        bool HasFeature(string featureName);
        string GetFeature(string featureName);
        T GetFeature<T>(string featureName);
        bool HasLimit(string limitName);
        string GetLimit(string limitName);
        T GetLimit<T>(string limitName);
        DateTime? Issued { get; }
        DateTime? ValidTo { get; }
        string Serial { get; }
    }

#if (LICENSE)
    public sealed class License : ILicense
    {
        private volatile static License _current;
        private static readonly object SyncLock = new object();
        private Client _client;
        private LicenseInfo _info = new LicenseInfo();

        public static ILicense Current
        {
            get
            {
                if (_current == null)
                {
                    lock (SyncLock)
                    {
                        if (_current == null)
                            _current = new License();
                    }
                }
                return _current;
            }
        }

        internal ICollection<LicenseException> Errors { get; set; }
        internal ICollection<LicenseException> Warnings { get; set; }

        private License()
        {
            Initialize(true);
        }

        public void Refresh()
        {
            Refresh(true);
        }

        internal void Refresh(bool useServer)
        {
            _info = null;
            Initialize(!useServer);
        }

        public ILicenseRequest GetOfflineActivationCode(byte[] licenseCode)
        {
            return new LicenseRequest(RequestType.Activate, _client, licenseCode);
        }

        public void ActivateFromOfflineCode(byte[] code)
        {
            ValidateLicense(code);
        }

        private void Initialize(bool trylocal)
        {
            Errors = new HashSet<LicenseException>();
            Warnings = new HashSet<LicenseException>();

            try
            {
                _client = new Client();
                //First try load from isolated store

                byte[] licenseData;
                if (trylocal)
                {
                    try
                    {
                        using (var isoStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                        using (
                            var file = new IsolatedStorageFileStream(_client.GetSerialNumber().Encode(),
                                                                     FileMode.Open,
                                                                     isoStore))
                        {
                            var dataTs = new byte[8];
                            file.Read(dataTs, 0, dataTs.Length);
                            var dateTime = BitConverter.ToInt64(dataTs, 0);

                            if (dateTime > DateTime.UtcNow.Ticks)
                                throw new LicenseException("Time shifting detected");

                            licenseData = file.ReadAllBytes(false);
                        }
                        ValidateLicense(licenseData);
                    }
                    catch (LicenseException e)
                    {
                        Warnings.Add(new LicenseException("Failed to load offline license", e));
                    }
                    catch (IOException e)
                    {
                        Warnings.Add(new LicenseException("Failed to access offline storage", e));
                    }
                }

                if (_info == null || !_info.IsValid())
                {
                    var request = new LicenseRequest(RequestType.License,
                                                     _client.GetSerialNumber(),
                                                     null,
                                                     null);
                    licenseData = request.ToWebRequest(_client).GetResponse().GetResponseStream().ReadAllBytes();
                    ValidateLicense(licenseData);
                }
            }
            catch (LicenseException e)
            {
                Errors.Add(e);
            }
            catch (WebException e)
            {
                try
                {
                    var response = Encoding.UTF8.GetString(e.Response.GetResponseStream().ReadAllBytes());
                    Errors.Add(new LicenseException(response, e));
                }
                catch (Exception)
                {
                    Errors.Add(new LicenseException("Can't read server error", e));
                }
            }
            catch (Exception e)
            {
                Errors.Add(new LicenseException("License error", e));
            }
        }

        private void ValidateLicense(byte[] licenseData)
        {
            if (licenseData != null)
            {
                try
                {

                    var result = new LicenseResult(licenseData, _client);

                    _info = LicenseInfo.Deserialize(result.AsString());
                    if (_info == null)
                        throw new LicenseException("License info can't be read");
                    if (!_info.IsValid())
                    {
                        throw new LicenseException("License info invalid");
                    }
                    //If all ok - save to store
                    try
                    {
                        using (var isoStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                        using (
                            var file = new IsolatedStorageFileStream(_client.GetSerialNumber().Encode(),
                                                                     FileMode.OpenOrCreate, isoStore))
                        {
                            var dataTs = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
                            file.Write(dataTs, 0, dataTs.Length);
                            file.Write(licenseData, 0, licenseData.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Warnings.Add(new LicenseException("Failed to save license to offline storage", e));
                    }
                }
                catch (Exception e)
                {
                    throw new LicenseValidationException("License invalid", e);
                }
            }
            else
            {
                throw new LicenseException("License can't be retrieved");
            }
        }

        public bool IsValid()
        {
            return _info != null && _info.IsValid();
        }

        public bool HasFeature(string featureName)
        {
            return IsValid() && _info.Features.ContainsKey(featureName);
        }

        public string GetFeature(string featureName)
        {
            if (HasFeature(featureName))
                return _info.Features[featureName];
            throw new LicenseFeatureNotFoundException(featureName);
        }

        public T GetFeature<T>(string featureName)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(GetFeature(featureName));
        }

        public bool HasLimit(string limitName)
        {
            return IsValid() && _info.Limits.ContainsKey(limitName);
        }

        public string GetLimit(string limitName)
        {
            if (HasLimit(limitName))
                return _info.Limits[limitName];
            throw new LicenseFeatureNotFoundException(limitName);
        }

        public T GetLimit<T>(string limitName)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(GetLimit(limitName));
        }

        public DateTime? Issued
        {
            get { return IsValid() ? _info.Issued : (DateTime?)null; }
        }

        public DateTime? ValidTo
        {
            get { return IsValid() ? _info.ValidTo : (DateTime?)null; }
        }


        public string Serial
        {
            get { return _client.GetSerialNumber().Encode(); }
        }

        public static class LimitsConstants
        {
            public const string UserCount = "user.limit";
            public const string SpaceCount = "space.limit";
        }

        public static class FeaturesConstants
        {
            public const string CommunityFeature = "community.feature";
            public const string ProjectsFeature = "projects.feature";
            public const string FilesFeature = "files.feature";
            public const string CrmFeature = "crm.feature";
            public const string MailFeature = "mail.feature";
            public const string TalkFeature = "talk.feature";
        }
    }
#else
    public sealed class License :ILicense
    {
        private volatile static License _current;
        private static readonly object SyncLock = new object();

        public static ILicense Current
        {
            get
            {
                if (_current == null)
                {
                    lock (SyncLock)
                    {
                        if (_current == null)
                            _current = new License();
                    }
                }
                return _current;
            }
        }

        public void Refresh()
        {
            
        }

        public ILicenseRequest GetOfflineActivationCode(byte[] licenseCode)
        {
            return null;
        }

        public void ActivateFromOfflineCode(byte[] code)
        {
            
        }

        public bool IsValid()
        {
            return true;
        }

        public bool HasFeature(string featureName)
        {
            return true;
        }

        public string GetFeature(string featureName)
        {
            return string.Empty;
        }

        public T GetFeature<T>(string featureName)
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)true;
            return default(T);
        }

        public bool HasLimit(string limitName)
        {
            return false;
        }

        public string GetLimit(string limitName)
        {
            return string.Empty;
        }

        public T GetLimit<T>(string limitName)
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)true;
            if (typeof(T) == typeof(int))
                return (T)(object)int.MaxValue;
            return default(T);
        }

        public DateTime? Issued
        {
            get { return DateTime.MinValue; }
        }

        public DateTime? ValidTo
        {
            get { return DateTime.MaxValue; }
        }

        private readonly string _serial = new Guid().ToString("N");

        public string Serial
        {
            get { return _serial; }
        }
    }
#endif
}