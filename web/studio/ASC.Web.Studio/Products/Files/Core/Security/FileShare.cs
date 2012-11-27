using System.Runtime.Serialization;

namespace ASC.Files.Core.Security
{
    [DataContract(Name = "fileShare", Namespace = "")]
    public enum FileShare
    {
        [EnumMember(Value = "0")]
        None,

        [EnumMember(Value = "1")]
        ReadWrite,

        [EnumMember(Value = "2")]
        Read,

        [EnumMember(Value = "3")]
        Restrict,
    }
}