﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Web.Help.DocumentGenerator;
using ASC.Api.Web.Help.Helpers;
using Microsoft.Practices.Unity;

namespace ASC.Api.Web.Help
{
    internal static class Documentation
    {
        static List<MsDocEntryPoint> _points = new List<MsDocEntryPoint>();
        static readonly Dictionary<string, MsDocEntryPointMethod> MethodList = new Dictionary<string, MsDocEntryPointMethod>();

        private static Searcher _searcher;
        private static DateTime _lastWriteTime;

        public static void Load(string msDocFolder, string searchIndexDir)
        {
            MethodList.Clear();
            //Load documentation
            _points = GenerateDocs(msDocFolder);

            var basePath = ConfigurationManager.AppSettings["apiprefix"] ?? "api";
            if (_points != null) _points.ForEach(x => x.Methods.ForEach(y => y.Path = basePath + y.Path));


            _searcher = new Searcher(searchIndexDir);
            var docFiles = Directory.GetFiles(msDocFolder);
            _lastWriteTime = docFiles.Max(x=>new FileInfo(x).LastWriteTimeUtc);
            _searcher.CreateIndexIfNeeded(_points, _lastWriteTime);
            HttpRuntime.Cache.Add("docsfile", new KeyValuePair<string, string>(msDocFolder, searchIndexDir), new CacheDependency(docFiles), Cache.NoAbsoluteExpiration,
                                  Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, OnRemove);
        }

        public static List<MsDocEntryPoint> GenerateDocs(string msDocFolder)
        {
            //Generate the docs first
            var container = ApiSetup.ConfigureEntryPoints();
            var entries = container.Resolve<IEnumerable<IApiMethodCall>>();

            var apiEntryPoints =
                container.Registrations.Where(x => x.RegisteredType == typeof(IApiEntryPoint)).ToList();

            var generator = new MsDocDocumentGenerator(Path.Combine(msDocFolder, "help.xml"),msDocFolder,container);

            foreach (var apiEntryPoint in entries.GroupBy(x => x.ApiClassType))
            {

                IGrouping<Type, IApiMethodCall> point = apiEntryPoint;
                generator.GenerateDocForEntryPoint(
                    apiEntryPoints.SingleOrDefault(x => x.MappedToType == point.Key),
                    apiEntryPoint.AsEnumerable());
            }
            
            return generator.Points;
        }

        private static void OnRemove(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.DependencyChanged)
            {
                //need http context to reload:(
                try
                {
                    var settings = (KeyValuePair<string, string>)value;
                    Load(settings.Key, settings.Value);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                //Insert again
                var settings = (KeyValuePair<string, string>)value;
                HttpRuntime.Cache.Add(key, value, new CacheDependency(Directory.GetFiles(settings.Key)), Cache.NoAbsoluteExpiration,
                                  Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, OnRemove);
            }
        }

        public static MsDocEntryPoint GetDocs(string name)
        {
            return _points.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<MsDocEntryPoint> GetAll()
        {
            return _points;
        }

        public static Dictionary<MsDocEntryPoint, Dictionary<MsDocEntryPointMethod, string>> Search(string query)
        {
            if (_searcher.IsOperational && !String.IsNullOrEmpty(query))
            {
                return SearchFullText(query);
            }
            var terms = Regex.Split(query ?? String.Empty, @"\W+").Where(x => !String.IsNullOrEmpty(x));
            var result = _points.ToDictionary(x => x, msDocEntryPoint => (msDocEntryPoint.Methods.Where(
                method => terms.All(x => method.Summary != null && method.Summary.IndexOf(x, StringComparison.OrdinalIgnoreCase) != -1))).ToDictionary(key => key, value => String.Empty));
            ThreadPool.QueueUserWorkItem(x =>
                                             {
                                                 try
                                                 {
                                                     _searcher.CreateIndexIfNeeded(_points, _lastWriteTime);
                                                 }
                                                 catch
                                                 {
                                                 }
                                             });
            return result;

        }

        private static Dictionary<MsDocEntryPoint, Dictionary<MsDocEntryPointMethod, string>> SearchFullText(string query)
        {
            return _searcher.Search(query, _points);
        }

        public static void GenerateRouteMap()
        {
            if (!MethodList.Any())
            {
                //Build list
                var reqContext = new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData());
                foreach (var msDocEntryPoint in _points)
                {
                    MvcApplication.CacheManifest.AddCached(new Uri(Url.GetDocUrl(msDocEntryPoint.Name,null,null,reqContext), UriKind.Relative));
                    foreach (var method in msDocEntryPoint.Methods)
                    {
                        method.Parent = msDocEntryPoint;
                        var url = Url.GetDocUrl(msDocEntryPoint.Name, method.HttpMethod, method.Path, reqContext);
                        MethodList.Add(url, method);
                        //MvcApplication.CacheManifest.AddCached(new Uri(url, UriKind.Relative));
                    }
                }
            }
        }

        public static MsDocEntryPointMethod GetByUri(Uri uri)
        {
            
            MsDocEntryPointMethod pointMethod;
            MethodList.TryGetValue(uri.AbsolutePath, out pointMethod);
            return pointMethod;
        }
    }
}