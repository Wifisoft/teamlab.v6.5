using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASC.Web.Files.Services.WCFService
{
    [CollectionDataContract(Name = "{0}List", Namespace = "", ItemName = "entry")]
    public class ItemList<TItem> : List<TItem>
    {
        public ItemList()
            : base()
        {
        }

        public ItemList(IEnumerable<TItem> items)
            : base(items)
        {
        }
    }

    [CollectionDataContract(Name = "{1}Hash", Namespace = "", ItemName = "entry", KeyName = "key", ValueName = "value")]
    public class ItemDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public ItemDictionary()
            : base()
        {
        }

        public ItemDictionary(IDictionary<TKey, TValue> items)
            : base(items)
        {
        }
    }
}