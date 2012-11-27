#region usings

using System;

#endregion

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : Attribute
    {
        public long CacheTime { get; set; }
    }
}