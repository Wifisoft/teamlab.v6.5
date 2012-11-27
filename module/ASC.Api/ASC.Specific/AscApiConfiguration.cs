using System.Linq;
using ASC.Api.Interfaces;

namespace ASC.Specific
{
    public class AscApiConfiguration : IApiConfiguration
    {
        public const uint DefaultItemsPerPage = 25;

        public string ApiPrefix
        {
            get;
            set;
        }

        public string ApiVersion
        {
            get;
            set;
        }

        public char ApiSeparator
        {
            get;
            set;
        }

        public AscApiConfiguration(string version)
            : this(string.Empty, version, DefaultItemsPerPage)
        {
        }

        public AscApiConfiguration(string prefix, string version)
            : this(prefix, version, DefaultItemsPerPage)
        {
        }

        public AscApiConfiguration(string prefix, string version, uint maxPage)
        {
            ApiSeparator = '/';
            ApiPrefix = prefix??string.Empty;
            ApiVersion = version;
            ItemsPerPage = maxPage;
        }

        public string GetBasePath()
        {
            return (ApiPrefix + ApiSeparator + ApiVersion + ApiSeparator).TrimStart('/', '~');
        }

        public uint ItemsPerPage { get; private set; }
    }
}