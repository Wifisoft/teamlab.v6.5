using System;

namespace ASC.Web.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
        public Type ModuleType
        {
            get;
            private set;
        }

        
        public ModuleAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }
}
