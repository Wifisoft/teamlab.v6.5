using System;

namespace ASC.Web.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class AddonAttribute : Attribute
    {
        public Type AddonType { get; private set; }

        public AddonAttribute(Type addonType)
        {
            AddonType = addonType;
        }
    }
}
