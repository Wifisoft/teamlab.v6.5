using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;

namespace TMResourceData
{
    public class DBResourceManager : ResourceManager
    {
        static object lockObject = new object();
        static DateTime _updateDate = DateTime.UtcNow;
        static Hashtable _resData;
        static Hashtable _resDataForTrans;

        // settings
        readonly static int updateSeconds;
        readonly static string getPagePortal;
        readonly static List<string> updatePortals;


        readonly string _fileName;
        readonly ResourceManager _resManager;


        // not beforfieldInit
        static DBResourceManager()
        {
            updateSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["resources.cache-timeout"] ?? "2");
            updatePortals = (ConfigurationManager.AppSettings["resources.trans-portals"] ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            getPagePortal = ConfigurationManager.AppSettings["resources.pageinfo-portal"];
        }

        public DBResourceManager(string fileName, ResourceManager resManager)
        {
            ResourceSets = new Hashtable();
            _fileName = fileName;
            _resManager = resManager;
        }

        public override ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            DBResourceSet databaseResourceSet;

            while (true)
            {
                if (ResourceSets.Contains(culture.Name) && (DBResourceSet)ResourceSets[culture.Name] != null)
                    databaseResourceSet = (DBResourceSet)ResourceSets[culture.Name];
                else
                {
                    databaseResourceSet = new DBResourceSet(_fileName, culture);
                    ResourceSets.Add(culture.Name, databaseResourceSet);
                }

                if (databaseResourceSet.TableCount != 0 || culture.Equals(CultureInfo.InvariantCulture))
                    break;

                culture = culture.Parent;
            }

            if (DateTime.UtcNow > _updateDate.AddSeconds(2))
            {
                GetResource.UpdateDBRS(databaseResourceSet, _fileName, culture.Name, _updateDate);
                _updateDate = DateTime.UtcNow;
            }

            return databaseResourceSet;

        }

        public override string GetString(string name, CultureInfo culture)
        {
            try
            {
                var pageLink = string.Empty;
                var resDataTable = LoadData();

                try
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    {
                        var uri = HttpContext.Current.Request.Url;

                        if (uri.Host.Contains("-translator") || uri.Host.Contains("we-translate") || updatePortals.Contains(uri.Host, StringComparer.InvariantCultureIgnoreCase))
                        {
                            resDataTable = LoadDataTrans();
                            if (DateTime.UtcNow > _updateDate.AddSeconds(updateSeconds))
                            {
                                GetResource.UpdateHashTable(ref resDataTable, _updateDate);
                                _updateDate = DateTime.UtcNow;
                            }
                        }

                        if (uri.Host == getPagePortal)
                        {
                            pageLink = uri.AbsolutePath;
                        }
                    }
                }
                catch { }

                var ci = culture ?? CultureInfo.CurrentUICulture;
                while (true)
                {
                    var language = !string.IsNullOrEmpty(ci.Name) ? ci.Name : "Neutral";

                    var resdata = resDataTable[name + _fileName + language];
                    if (resdata != null)
                    {
                        if (!string.IsNullOrEmpty(pageLink))
                        {
                            GetResource.AddLink(name, _fileName, pageLink);
                        }
                        return resdata.ToString();
                    }

                    if (ci.Equals(CultureInfo.InvariantCulture))
                    {
                        break;
                    }
                    ci = ci.Parent;
                }
            }
            catch { }

            return _resManager.GetString(name, culture);
        }


        private Hashtable LoadData()
        {
            if (_resData == null)
            {
                lock (lockObject)
                {
                    if (_resData == null)
                    {
                        _resData = GetResource.GetAllData("tmresource");
                    }
                }
            }
            return _resData;
        }

        private Hashtable LoadDataTrans()
        {
            if (_resDataForTrans == null)
            {
                lock (lockObject)
                {
                    if (_resDataForTrans == null)
                    {
                        _resDataForTrans = GetResource.GetAllData("tmresourceTrans");
                    }
                }
            }
            return _resDataForTrans;
        }
    }
}
