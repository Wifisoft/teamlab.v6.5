using System;

namespace ASC.Web.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WidePageAttribute : Attribute
    {
        public bool Wide { get; private set; }

        public WidePageAttribute(bool wide)
        {
            Wide = wide;
        }
    }
}