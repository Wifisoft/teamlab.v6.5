using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ASC.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using log4net;

namespace ASC.Web.Core
{
    public class ProductManager
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web");


        public static ProductManager Instance
        {
            get;
            private set;
        }

        public List<IProduct> Products
        {
            get;
            private set;
        }

        public List<IGlobalHandler> GlobalHandlers
        {
            get;
            private set;
        }

        public Hashtable ProductContexts
        {
            get;
            private set;
        }


        static ProductManager()
        {
            Instance = new ProductManager();
        }

        private ProductManager()
        {
            Products = new List<IProduct>();
            ProductContexts = Hashtable.Synchronized(new Hashtable());
            GlobalHandlers = new List<IGlobalHandler>();
        }


        public void RegisterProduct(IProduct product)
        {
            RegisterProduct(product, Assembly.GetExecutingAssembly());
        }

        private void RegisterProduct(IProduct product, Assembly assembly)
        {
            try
            {
                var productContext = new ProductContext { AssemblyName = assembly.FullName };

                product.Init(productContext);
                ProductContexts.Add(product.ID, productContext);
                WebItemManager.Instance.RegistryItem(product);

                if (productContext.GlobalHandler != null)
                {
                    GlobalHandlers.Add(productContext.GlobalHandler);
                }
                foreach (var module in product.Modules)
                {
                    try
                    {
                        if (module.Context != null && module.Context.UserActivityPublishers != null)
                        {
                            module.Context.UserActivityPublishers.ForEach(p => p.DoUserActivity += DoUserActivityHandler);
                        }
                        if (module.Context != null && module.Context.SearchHandler != null)
                        {
                            SearchHandlerManager.Registry(module.Context.SearchHandler);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error(string.Format("Couldn't load module activity publisher {0}", module.Name), exc);
                    }
                }
                if (product.Context.UserActivityPublishers != null)
                {
                    product.Context.UserActivityPublishers.ForEach(p => p.DoUserActivity += DoUserActivityHandler);
                }

                Products.Add(product);
                Products.Sort((p1, p2) => p1.Context.DefaultSortOrder.CompareTo(p2.Context.DefaultSortOrder));

                log.DebugFormat("product {0} loaded", product.Name);
            }
            catch (Exception exc)
            {
                log.Error(string.Format("Couldn't load product {0}", product.Name), exc);
            }
        }

        public void LoadProducts()
        {
            if (HttpContext.Current == null) return;

            foreach (var path in Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/products")))
            {
                var productAssemblyPath = Path.Combine(path, "bin\\ASC.Web." + Path.GetFileName(path) + ".dll");
                if (File.Exists(productAssemblyPath))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(productAssemblyPath);
                        var attr = assembly.GetCustomAttributes(typeof(ProductAttribute), false).Cast<ProductAttribute>().FirstOrDefault();
                        if (attr != null)
                        {
                            RegisterProduct((IProduct)Activator.CreateInstance(attr.ProductType), assembly);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error(String.Format("Couldn't load product {0}", Path.GetFileName(path)), exc);
                    }
                }
            }
        }

        public IProduct this[Guid id]
        {
            get
            {
                return Products.Find(p => id.Equals(p.ID));
            }
        }

        public IModule GetModuleByID(Guid moduleID)
        {
            foreach (var product in Products)
            {
                var module = Array.Find(product.Modules, m => m.ID.Equals(moduleID));
                if (module != null) return module;
            }
            return null;
        }

        public ModuleContext GetModuleContext(Guid moduleID)
        {
            foreach (var product in Products)
            {
                var module = Array.Find(product.Modules, m => m.ID.Equals(moduleID));
                if (module != null) return module.Context;
            }
            return null;
        }

        public static void RegisterUserActivityPublisher(IUserActivityPublisher publisher)
        {
            if (publisher != null)
                publisher.DoUserActivity += DoUserActivityHandler;
        }
        public static void UnregisterUserActivityPublisher(IUserActivityPublisher publisher)
        {
            if (publisher != null)
                publisher.DoUserActivity -= DoUserActivityHandler;
        }

        private static void DoUserActivityHandler(object sender, UserActivityEventArgs e)
        {
            if (e.UserActivity != null)
            {
                if (e.UserActivity.TenantID == 0)
                {
                    e.UserActivity.TenantID = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                }
                //Fix url
                if (Uri.IsWellFormedUriString(e.UserActivity.URL, UriKind.Relative))
                {
                    //It's relative remove and make it look like ~/
                    if (HttpContext.Current != null)
                    {
                        //Get rel app app path
                        var appPath = VirtualPathUtility.ToAbsolute("~/");
                        if (e.UserActivity.URL.StartsWith(appPath, StringComparison.OrdinalIgnoreCase))
                        {
                            //Replace it with ~/
                            e.UserActivity.URL = "~/" + e.UserActivity.URL.Remove(0, appPath.Length).TrimStart('/');
                        }
                    }

                }
                UserActivityManager.SaveUserActivity(e.UserActivity);
            }
        }
    }
}
