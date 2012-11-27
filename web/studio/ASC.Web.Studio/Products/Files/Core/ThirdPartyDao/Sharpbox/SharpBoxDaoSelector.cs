using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ASC.Files.Core.ProviderDao;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core.ThirdPartyDao.Sharpbox
{
    internal class SharpBoxDaoSelector : RegexDaoSelectorBase<string>
    {
        internal class SharpBoxInfo
        {
            public SharpBoxProviderInfo SharpBoxProviderInfo { get; set; }

            public string Path { get; set; }
            public string PathPrefix { get; set; }
        }

        public SharpBoxDaoSelector()
            : base(new Regex(@"^sbox-(?'id'\d+)(-(?'path'.*)){0,1}$", RegexOptions.Singleline | RegexOptions.Compiled))
        {
        }

        public override IFileDao GetFileDao(object id)
        {
            return new SharpBoxFileDao(GetInfo(id), this);
        }

        public override IFolderDao GetFolderDao(object id)
        {
            return new SharpBoxFolderDao(GetInfo(id), this);
        }

        public override object ConvertId(object id)
        {
            if (id != null)
            {
                var match = Selector.Match(Convert.ToString(id, CultureInfo.InvariantCulture));
                if (match.Success)
                {
                    return match.Groups["path"].Value.Replace('|', '/');
                }
                throw new ArgumentException("Id is not a sharpbox id");
            }
            return base.ConvertId(null);
        }

        private SharpBoxInfo GetInfo(object objectId)
        {
            if (objectId == null) throw new ArgumentNullException("objectId");
            var id = Convert.ToString(objectId, CultureInfo.InvariantCulture);
            var match = Selector.Match(id);
            if (match.Success)
            {
                var providerInfo = GetProviderInfo(Convert.ToInt32(match.Groups["id"].Value));

                return new SharpBoxInfo
                           {
                               Path = match.Groups["path"].Value,
                               SharpBoxProviderInfo = providerInfo,
                               PathPrefix = "sbox-" + match.Groups["id"].Value
                           };
            }
            throw new ArgumentException("Id is not a sharpbox id");
        }

        public override object GetIdCode(object id)
        {
            if (id != null)
            {
                var match = Selector.Match(Convert.ToString(id, CultureInfo.InvariantCulture));
                if (match.Success)
                {
                    return match.Groups["id"].Value;
                }
            }
            return base.GetIdCode(id);
        }

        private SharpBoxProviderInfo GetProviderInfo(int linkId)
        {
            SharpBoxProviderInfo info;

            using (var dbDao = Global.DaoFactory.GetProviderDao())
            {
                try
                {
                    info = (SharpBoxProviderInfo) dbDao.GetProviderInfo(linkId);
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException("Provider id not found or you have no access");
                }
            }
            return info;
        }

        public void RenameProvider(SharpBoxProviderInfo sharpBoxProviderInfo, string newTitle)
        {
            using (var dbDao = new SharpBoxAccountDao(TenantProvider.CurrentTenantID, FileConstant.DatabaseId))
            {
                dbDao.UpdateProviderInfo(sharpBoxProviderInfo.ID, newTitle);
                sharpBoxProviderInfo.UpdateTitle(newTitle); //This will update cached version too
            }
        }
    }
}