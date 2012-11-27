#region usings

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace ASC.Api.Collections
{
    [CollectionDataContract(Name = "list", Namespace = "", ItemName = "entry")]
    public class ItemList<TItem> : List<TItem>
    {
        public ItemList()
        {
        }

        public ItemList(IEnumerable<TItem> items)
            : base(items)
        {
        }
    }
}