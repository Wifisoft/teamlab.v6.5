using System.Runtime.Serialization;
using System.Diagnostics;

namespace ASC.Files.Core
{
    [DataContract(Name = "sorted_by_type", Namespace = "")]
    public enum SortedByType
    {
        [EnumMember] DateAndTime,

        [EnumMember] AZ,

        [EnumMember] Size,

        [EnumMember] Author,

        [EnumMember] Type
    }

    [DataContract(Name = "orderBy", Namespace = "")]
    [DebuggerDisplay("{SortedBy} {IsAsc}")]
    public class OrderBy
    {
        [DataMember(Name = "is_asc")]
        public bool IsAsc { get; set; }

        [DataMember(Name = "property")]
        public SortedByType SortedBy { get; set; }

        public OrderBy(SortedByType sortedByType, bool isAsc)
        {
            SortedBy = sortedByType;
            IsAsc = isAsc;
        }
    }
}