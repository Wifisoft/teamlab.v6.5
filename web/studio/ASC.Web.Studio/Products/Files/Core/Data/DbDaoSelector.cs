using System;
using System.Text.RegularExpressions;
using ASC.Files.Core.ProviderDao;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core.Data.ProviderDao
{
    internal class DbDaoSelector : RegexDaoSelectorBase<int>
    {
        public DbDaoSelector()
            : base(
                new Regex(@"^\d+$", RegexOptions.Singleline | RegexOptions.Compiled),
                x => new FileDao(TenantProvider.CurrentTenantID, FileConstant.DatabaseId),
                x => new FolderDao(TenantProvider.CurrentTenantID, FileConstant.DatabaseId),
                Convert.ToInt32
                )
        {

        }

        public override object GetIdCode(object id)
        {
            return 0;
        }
    }
}