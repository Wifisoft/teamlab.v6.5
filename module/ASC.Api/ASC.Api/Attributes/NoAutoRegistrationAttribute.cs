using System;

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NoAutoRegistrationAttribute:Attribute
    {
        
    }
}