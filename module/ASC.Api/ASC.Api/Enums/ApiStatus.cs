#region usings

using System.Runtime.Serialization;

#endregion

namespace ASC.Api.Enums
{
    [DataContract]
    public enum ApiStatus
    {
        [EnumMember(Value = "ok")] Ok,
        [EnumMember(Value = "error")] Error
    }
}