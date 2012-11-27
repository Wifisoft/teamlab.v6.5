using System;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.TopNavigationProductList | WebZoneType.StartProductList)]
    public interface IProduct : IWebItem
    {
        Guid ProductID { get; }

        IModule[] Modules { get; }

        new ProductContext Context { get; }


        void Init(ProductContext context);

        void Shutdown();
    }
}
