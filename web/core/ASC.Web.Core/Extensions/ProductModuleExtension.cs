using System;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core
{
    public static class ProductModuleExtension
    {
        
        
        public static string GetSmallIconAbsoluteURL(this IModule module)
        {
            if (module == null || module.Context == null || String.IsNullOrEmpty(module.Context.SmallIconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(module.Context.SmallIconFileName, module.ID);
        }

        public static string GetSmallIconAbsoluteURL(this IProduct product)
        {
            if (product == null || product.Context == null || String.IsNullOrEmpty(product.Context.SmallIconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(product.Context.SmallIconFileName, product.ID);
        }

        public static string GetIconAbsoluteURL(this IModule module)
        {
            if (module == null || module.Context == null || String.IsNullOrEmpty(module.Context.IconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(module.Context.IconFileName, module.ID);
        }

        public static string GetIconAbsoluteURL(this IProduct product)
        {
            if (product == null || product.Context == null || String.IsNullOrEmpty(product.Context.IconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(product.Context.IconFileName, product.ID);
        }

        public static IHtmlInjectionProvider GetInjectionProvider(this IProduct product)
        {
            if (product != null && product.Context != null)
                return GetInjectionProvider(product.Context.HtmlInjectionProviderType);

            return null;
        }

        public static IHtmlInjectionProvider GetInjectionProvider(this IModule module)
        {
            if (module != null && module.Context != null)
                return GetInjectionProvider(module.Context.HtmlInjectionProviderType);

            return null;
        }

        private static IHtmlInjectionProvider GetInjectionProvider(Type type)
        {
            if (type == null)
                return null;

            foreach (var interfaceType in type.GetInterfaces())
            { 
                if(interfaceType.Equals(typeof(IHtmlInjectionProvider)))
                    return (IHtmlInjectionProvider)Activator.CreateInstance(type);
            }

            return null;
        }

    }
}
