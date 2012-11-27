using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace ASC.Web.Core.Utility
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Medium)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.High)]
    public class AssemblyResourceProvider : VirtualPathProvider
    {
        private List<IProduct> _products;
        
        public AssemblyResourceProvider(List<IProduct> products)
        {
            _products = products;
        }

        private object GetProductOrModule(string current_vp)
        {   
            string dirVP = VirtualPathUtility.GetDirectory(VirtualPathUtility.ToAppRelative(current_vp));

            object result = null;
            int matchCount = int.MaxValue;

            foreach (var product in _products)
            {
                var prodVP = VirtualPathUtility.GetDirectory(product.StartURL);
                if (dirVP.IndexOf(prodVP, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    int diff = dirVP.Length - prodVP.Length;
                    if (diff == 0)
                        return product;

                    else if (matchCount > diff)
                    {
                        matchCount = diff;
                        result = product;
                    }
                }
                
                if (product.Modules != null)
                {
                    foreach (var module in product.Modules)
                    {
                        var modVP = VirtualPathUtility.GetDirectory(module.StartURL);
                        if (dirVP.IndexOf(modVP, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            int diff = dirVP.Length - modVP.Length;
                            if (diff == 0)
                                return module;

                            else if (matchCount > diff)
                            {
                                matchCount = diff;
                                result = module;
                            }
                        }
                    }
                }
            }

            return result;
        }


        private bool IsAppResourcePath(string virtualPath)
        {
            var obj = GetProductOrModule(virtualPath);
            return obj != null;
        }

        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) || base.FileExists(virtualPath));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var obj = GetProductOrModule(virtualPath);          
            if (obj != null)
            {
                if(obj is IProduct)
                {
                    var product = obj as IProduct;
                    return new AssemblyResourceVirtualFile(virtualPath) { AssemblyName = product.Context.AssemblyName};
                }
            }
            
            return base.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return null;
        }

        private class AssemblyResourceVirtualFile : VirtualFile
        {
            private string _path;
            public string AssemblyName { get; set; }

            public AssemblyResourceVirtualFile(string virtualPath)
                : base(virtualPath)
            {
                _path = VirtualPathUtility.ToAppRelative(virtualPath);
            }

            public override Stream Open()
            {   
                Stream s = null;
                Assembly assembly = Assembly.Load(this.AssemblyName);

                if (assembly == null) 
                    throw new Exception("Failed to load asembly");

                var resourceNames = assembly.GetManifestResourceNames();
                if (resourceNames != null && resourceNames.Length > 0)
                {
                    var resName = Array.Find<string>(resourceNames, n=> n.EndsWith("."+VirtualPathUtility.GetFileName(_path), StringComparison.InvariantCultureIgnoreCase));
                    if(!String.IsNullOrEmpty(resName))
                        s = assembly.GetManifestResourceStream(resName);

                   
                }
                
                if (s == null) 
                    throw new Exception("Failed to load resource");

                return s;
            }
        }
    }
    
}
