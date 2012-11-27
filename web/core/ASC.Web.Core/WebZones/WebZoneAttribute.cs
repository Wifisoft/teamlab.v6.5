using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Web.Core.WebZones
{
    [Flags]
    public enum WebZoneType
    { 
         Nowhere = 1,
        StartProductList = 2,
        TopNavigationProductList = 4,
        MyTools = 8,
        CustomProductList = 16,

        All = Nowhere | StartProductList | TopNavigationProductList | MyTools |CustomProductList
    }

    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class WebZoneAttribute : Attribute
    {
        public WebZoneType Type { get; private set; }

        public WebZoneAttribute(WebZoneType type)
        {
            this.Type = type;
        }

    }

    public interface IRenderWebItem { }
     
    
}
