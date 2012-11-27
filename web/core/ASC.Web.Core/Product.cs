using System;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.TopNavigationProductList | WebZoneType.StartProductList)]
    public abstract class Product : IProduct
    {
        public abstract Guid ProductID { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string StartURL { get; }

        public abstract IModule[] Modules { get; }

        public abstract void Init(ProductContext context);

        public abstract ProductContext Context { get; }

        public abstract void Shutdown();

        public virtual string ExtendedDescription { get { return Description; } }

        WebItemContext IWebItem.Context { get { return ((IProduct)this).Context; } }

        Guid IWebItem.ID { get { return ProductID; } }
    }
}
