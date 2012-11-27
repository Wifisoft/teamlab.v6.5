﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace ASC.Api.Web.Help.Helpers
{

    public abstract class CombinedFileCollection
    {
        public int Version { get; set; }
        private readonly string _target;

        private readonly UrlHelper _urlHelper;
        private readonly bool _combine;
        private readonly CacheManifest _cache;
        private readonly List<string> _urls = new List<string>();
        private readonly HttpContextBase _context;
        private readonly string _targetFile;
        private readonly List<string> _urlsFile = new List<string>();

        protected CombinedFileCollection(string target, HtmlHelper helper, UrlHelper urlHelper, bool combine, int version) : this(target, helper, urlHelper, combine, version, MvcApplication.CacheManifest)
        {
        }

        protected CombinedFileCollection(string target, HtmlHelper helper, UrlHelper urlHelper, bool combine, int version, CacheManifest cache)
        {
            if (cache!=null)
            {
                Version = -1;
            }
            else
            {
                Version = version;
            }
            _urlHelper = urlHelper;
            _combine = combine;
            _cache = cache;
            _context = helper.ViewContext.HttpContext;
            _target = _urlHelper.Content(target);
            _targetFile = _context.Server.MapPath(_target);
        }

        protected virtual string MapPath(string file)
        {
            return _context.Server.MapPath(file);
        }

        protected virtual CombinedFileCollection AddUrl(string url)
        {
            var urlR = _urlHelper.Content(url);
            _urls.Add(urlR);
            _urlsFile.Add(_context.Server.MapPath(urlR));
            return this;
        }

        protected virtual CombinedFileCollection AddUrls(IEnumerable<string> urls)
        {
            if (urls != null)
            {
                foreach (var url in urls)
                {
                    AddUrl(url);
                }
            }
            return this;
        }

        public MvcHtmlString Render()
        {
            PreRender();
            if (_combine)
            {
                var resultFile = _context.Server.MapPath(_target);
                bool shouldRebuild = HttpRuntime.Cache.Get(resultFile) == null || !File.Exists(resultFile);

                if (shouldRebuild)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(_targetFile)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(_targetFile));
                    }
                    using (var fs = File.Create(_targetFile))
                    {
                        BuildResult(fs, _urlsFile);
                    }
                    HttpRuntime.Cache.Insert(resultFile, DateTime.UtcNow, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                }
                TagBuilder tagBuilder = BuildTag(AppendVersionTo(_target));
                _cache.AddCached(new Uri(_target, UriKind.Relative));
                return MvcHtmlString.Create(tagBuilder.ToString(Mode));
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var url in _urls)
                {
                    _cache.AddCached(new Uri(url, UriKind.Relative));
                    builder.AppendLine(BuildTag(AppendVersionTo(url)).ToString(Mode));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        private string AppendVersionTo(string url)
        {
            if (Version != -1)
            {
                if (url.IndexOf("?") != -1)
                {
                    return url + "&v=" + Version;
                }
                return url + "?v=" + Version;
            }
            return url;
        }

        protected virtual void PreRender()
        {

        }

        protected abstract TagBuilder BuildTag(string target);

        protected abstract TagRenderMode Mode { get; }

        protected abstract void BuildResult(Stream fs, List<string> filePaths);
    }

    public class CssFileCollection : CombinedFileCollection
    {


        public CssFileCollection(string target, HtmlHelper helper, UrlHelper urlHelper, bool combine, int version)
            : base(target, helper, urlHelper, combine, version)
        {
        }

        protected override TagBuilder BuildTag(string target)
        {
            var builder = new TagBuilder("link");
            builder.MergeAttribute("href", target);
            builder.MergeAttribute("rel", "stylesheet");
            builder.MergeAttribute("type", "text/css");
            return builder;
        }

        public CssFileCollection With(string url)
        {
            return (CssFileCollection)base.AddUrl(url);
        }

        public CssFileCollection WithMany(IEnumerable<string> urls)
        {
            return (CssFileCollection)base.AddUrls(urls);
        }

        protected override TagRenderMode Mode
        {
            get { return TagRenderMode.SelfClosing; }
        }

        protected override void BuildResult(Stream fs, List<string> filePaths)
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            minifier.WarningLevel = 3;
            var styleBuilder = new StringBuilder();

            foreach (var style in filePaths)
            {
                var text = FileOperator.ReadAllText(style);
                try
                {
                    var mintext = minifier.MinifyStyleSheet(text);
                    if (minifier.Errors.Count == 0)
                    {
                        text = mintext;
                    }
                }
                catch (Exception)
                {
                }
                styleBuilder.AppendLine(text);
            }
            var buffer = Encoding.UTF8.GetBytes(styleBuilder.ToString());
            fs.Write(buffer, 0, buffer.Length);
        }
    }

    public class JsFileCollection : CombinedFileCollection
    {
        public JsFileCollection(string target, HtmlHelper helper, UrlHelper urlHelper, bool combine, int version)
            : base(target, helper, urlHelper, combine, version)
        {
        }

        protected override TagBuilder BuildTag(string target)
        {
            var builder = new TagBuilder("script");
            builder.MergeAttribute("src", target);
            builder.MergeAttribute("async","async");
            builder.MergeAttribute("type", "text/javascript");
            return builder;
        }

        protected override TagRenderMode Mode
        {
            get { return TagRenderMode.Normal; }
        }

        public JsFileCollection With(string url)
        {
            return (JsFileCollection)base.AddUrl(url);
        }

        public JsFileCollection WithMany(IEnumerable<string> urls)
        {
            return (JsFileCollection)base.AddUrls(urls);
        }

        protected override void BuildResult(Stream fs, List<string> filePaths)
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            minifier.WarningLevel = 3;
            var styleBuilder = new StringBuilder();


            foreach (var style in filePaths)
            {
                var text = FileOperator.ReadAllText(style);
                if (style.IndexOf(".min.") == -1)
                {
                    try
                    {
                        
                        var mintext = minifier.MinifyJavaScript(text);
                        text = mintext;
                    }
                    catch (Exception)
                    {
                    }
                }
                styleBuilder.Append(';');
                styleBuilder.AppendLine(text);

            }
            var buffer = Encoding.UTF8.GetBytes(styleBuilder.ToString());
            fs.Write(buffer, 0, buffer.Length);
        }
    }

    public class FileOperator
    {
        public static string ReadAllText(string file)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(fs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void WriteAllText(string file, string text)
        {
            using (var fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(text);
                    fs.Flush();
                }
            }
        }
    }
}