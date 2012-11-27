using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using ASC.Collections;
using ASC.Data.Storage.Configuration;

namespace ASC.Data.Storage
{
    public static class WebPath
    {
        private static readonly IDictionary<string, FixerConfigurationElement> FixersByExt;
        private static readonly AppenderConfigurationElement Appender;


        static WebPath()
        {
            FixersByExt = new Dictionary<string, FixerConfigurationElement>();
            var section = (StorageConfigurationSection)ConfigurationManager.GetSection(Schema.SECTION_NAME);
            if (section != null)
            {
                foreach (FixerConfigurationElement fixer in section.Fixers.Cast<FixerConfigurationElement>())
                {
                    FixerConfigurationElement fixer1 = fixer;
                    fixer.Extension
                        .Split(' ')
                        .Select(ext => ext.Trim().ToLowerInvariant())
                        .Where(ext => !string.IsNullOrEmpty(ext))
                        .ToList()
                        .ForEach(ext => FixersByExt[ext] = fixer1);
                }
                Appender = section.Appenders.Cast<AppenderConfigurationElement>().FirstOrDefault();
            }
        }


        public static string GetPath(string relativePath)
        {
            if (relativePath.StartsWith("~"))
                throw new ArgumentException(string.Format("bad path format {0} remove '~'", relativePath),
                                            "relativePath");

            string result = relativePath;

            try
            {
                string ext = Path.GetExtension(relativePath).ToLowerInvariant();
                FixerConfigurationElement fixer = FixersByExt.ContainsKey(ext) ? FixersByExt[ext] : null;
                if (fixer != null && !string.IsNullOrEmpty(fixer.AppendBeforeExt))
                {
                    relativePath = relativePath.Substring(0, relativePath.LastIndexOf(ext))
                                   + fixer.AppendBeforeExt + ext;
                }
            }
            catch
            {
            }

            if (Appender != null)
            {
                if (Appender.Append.StartsWith("~"))
                {
                    string query = string.Empty;
                    //Rel path
                    if (relativePath.IndexOfAny(new[] { '?', '=', '&' }) != -1)
                    {
                        //Cut it
                        query = relativePath.Substring(relativePath.IndexOf('?'));
                        relativePath = relativePath.Substring(0, relativePath.IndexOf('?'));
                    }
                    result = VirtualPathUtility.ToAbsolute(
                        string.Format("{0}/{1}{2}", Appender.Append.TrimEnd('/'),
                                      relativePath.TrimStart('/'), query)
                        );
                }
                else
                {
                    if (SecureHelper.IsSecure() && !string.IsNullOrEmpty(Appender.AppendSecure))
                    {
                        result = string.Format("{0}/{1}", Appender.AppendSecure.TrimEnd('/'),
                                               relativePath.TrimStart('/'));
                    }
                    else
                    {
                        //Append directly
                        result = string.Format("{0}/{1}", Appender.Append.TrimEnd('/'), relativePath.TrimStart('/'));
                    }
                }
            }
            //To LOWER! cause Amazon is CASE SENSITIVE!
            return result.ToLowerInvariant();
        }

        private static readonly IDictionary<string, bool> Existing = new SynchronizedDictionary<string, bool>();

        public static bool Exists(string relativePath)
        {
            var path = GetPath(relativePath);
            if (!Existing.ContainsKey(path))
            {
                if (Uri.IsWellFormedUriString(path, UriKind.Relative) && HttpContext.Current != null)
                {
                    //Local
                    Existing[path] = File.Exists(HttpContext.Current.Server.MapPath(path));
                }
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    //Make request
                    Existing[path] = CheckWebPath(path);
                }
            }
            return Existing[path];
        }

        private static bool CheckWebPath(string path)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(path);
                request.Method = "HEAD";
                var resp = (HttpWebResponse)request.GetResponse();
                resp.Close();
                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}