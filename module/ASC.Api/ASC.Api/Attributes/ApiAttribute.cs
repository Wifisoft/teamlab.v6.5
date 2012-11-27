#region usings

using System;
using System.Collections.Generic;
using System.Web.Routing;

#endregion

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiAttribute : Attribute
    {


        public ApiAttribute(string httpMethod, string path, bool requiresAuthorization)
        {
            Method = httpMethod;
            Path = path;
            RequiresAuthorization = requiresAuthorization;
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public bool RequiresAuthorization { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CreateAttribute : ApiAttribute
    {
        public CreateAttribute(string path, bool requiresAuthorization)
            : base("POST", path, requiresAuthorization)
        {
        }
        public CreateAttribute(string path) : base("POST", path,true)
        {
        }
    }



    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UpdateAttribute : ApiAttribute
    {
        public UpdateAttribute(string path, bool requiresAuthorization)
            : base("PUT", path, requiresAuthorization)
        {
        }
        public UpdateAttribute(string path) : base("PUT", path,true)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DeleteAttribute : ApiAttribute
    {
        public DeleteAttribute(string path, bool requiresAuthorization)
            : base("DELETE", path, requiresAuthorization)
        {
        }
        public DeleteAttribute(string path)
            : base("DELETE", path,true)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ReadAttribute : ApiAttribute
    {
        public ReadAttribute(string path, bool requiresAuthorization)
            : base("GET", path, requiresAuthorization)
        {
        }
        public ReadAttribute(string path) : base("GET", path,true)
        {
        }
    }
}