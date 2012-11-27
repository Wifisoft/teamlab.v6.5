using System;
using ASC.Core;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider;

namespace ASC.Files.Core.ThirdPartyDao
{
    public class SharpBoxProviderInfo : IProviderInfo
    {
        public int ID { get; set; }
        public Guid Owner { get; private set; }

        private readonly nSupportedCloudConfigurations _providerName;
        private readonly AuthData _authData;
        private readonly FolderType _rootFolderType;
        private readonly DateTime _createOn;

        public SharpBoxProviderInfo(int id, string providerName, string customerTitle, AuthData authData, Guid owner, FolderType rootFolderType, DateTime createOn)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            if (string.IsNullOrEmpty(authData.Token) && string.IsNullOrEmpty(authData.Password))
                throw new ArgumentNullException("token", "Both token and password can't be null");
            if (!string.IsNullOrEmpty(authData.Login) && string.IsNullOrEmpty(authData.Password) && string.IsNullOrEmpty(authData.Token))
                throw new ArgumentNullException("password", "Password can't be null");

            ID = id;
            CustomerTitle = customerTitle;
            Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;

            _providerName = (nSupportedCloudConfigurations) Enum.Parse(typeof (nSupportedCloudConfigurations), providerName, true);
            _authData = authData;
            _rootFolderType = rootFolderType;
            _createOn = createOn;
        }

        private void CreateStorage()
        {
            _storage = new CloudStorage();
            var config = CloudStorage.GetCloudConfigurationEasy(_providerName);
            if (!string.IsNullOrEmpty(_authData.Token))
            {
                if (_providerName != nSupportedCloudConfigurations.BoxNet)
                {
                    var token = _storage.DeserializeSecurityTokenFromBase64(_authData.Token);
                    _storage.Open(config, token);
                }
            }
            else
            {
                _storage.Open(config, new GenericNetworkCredentials {Password = _authData.Password, UserName = _authData.Login});
            }
        }

        private CloudStorage _storage;

        internal CloudStorage Storage
        {
            get
            {
                if (_storage == null)
                {
                    CreateStorage();
                }
                else
                {
                    if (!_storage.IsOpened)
                    {
                        //TODO: Check corrupted storage
                        CreateStorage();
                    }
                }
                return _storage;
            }
        }

        internal void UpdateTitle(string newtitle)
        {
            CustomerTitle = newtitle;
        }

        public string CustomerTitle { get; private set; }

        public DateTime CreateOn
        {
            get { return _createOn; }
        }

        public string UserName
        {
            get { return _authData.Login; }
        }

        public object RootFolderId
        {
            get { return "sbox-" + ID; }
        }

        public bool CheckAccess()
        {
            try
            {
                return Storage.GetRoot() != null;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public string ProviderName
        {
            get { return _providerName.ToString(); }
        }

        public FolderType RootFolderType
        {
            get { return _rootFolderType; }
        }
    }
}