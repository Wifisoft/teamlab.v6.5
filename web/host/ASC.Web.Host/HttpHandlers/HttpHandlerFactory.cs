using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ASC.Web.Host.Config;
using log4net;

namespace ASC.Web.Host.HttpHandlers
{
    static class HttpHandlerFactory
    {
        private static readonly IDictionary<string, IHttpHandler> handlers;

        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");


        public static IHttpHandler DefaultHttpHandler
        {
            get;
            private set;
        }

        public static IHttpHandler DirectoryHttpHandler
        {
            get;
            private set;
        }

        
        static HttpHandlerFactory()
        {
            DefaultHttpHandler = new StaticFileHttpHandler();
            DirectoryHttpHandler = new AspNetHttpHandler();

            var cache = new Dictionary<string, IHttpHandler>();
            handlers = new Dictionary<string, IHttpHandler>();
            foreach (HttpHandlerElement handlerElement in ServerConfiguration.HttpHandlers)
            {
                try
                {
                    if (handlerElement.Extension == "*")
                    {
                        DefaultHttpHandler = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                        continue;
                    }
                    if (handlerElement.Extension == "/")
                    {
                        DirectoryHttpHandler = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                        continue;
                    }
                    if (!handlers.ContainsKey(handlerElement.Extension))
                    {
                        handlers[handlerElement.Extension] = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Can not create HttpHandler '{0}'.\r\n{1}", handlerElement.HandlerType, ex);
                }
            }
        }

        private static IHttpHandler GetOrCreateHttpHandler(string handlerType, IDictionary<string, IHttpHandler> cache)
        {
            var handler = cache.ContainsKey(handlerType) ?
                cache[handlerType] :
                (IHttpHandler)Activator.CreateInstance(Type.GetType(handlerType, true));
            cache[handlerType] = handler;
            return handler;
        }

        public static IHttpHandler GetHttpHandler(HttpListenerContext context)
        {
            var extension = Path.GetExtension(context.Request.Url.AbsolutePath);
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
                return handlers.ContainsKey(extension) ? handlers[extension] : DefaultHttpHandler;
            }
            return DirectoryHttpHandler;
        }
    }
}