using System;

namespace ASC.Web.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ProductAttribute : Attribute
    {
        public ProductAttribute(Type productType)
        {
            this.ProductType = productType;
        }

        public Type ProductType { get; private set; }
    }
}
