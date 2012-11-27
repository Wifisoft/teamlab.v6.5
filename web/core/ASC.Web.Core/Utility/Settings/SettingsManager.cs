﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Core.Caching;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Core.Utility.Settings
{
    public class SettingsManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SettingsManager));

        private readonly ICache cache;
        private readonly TimeSpan expirationTimeout;
        private readonly IDictionary<Type, DataContractJsonSerializer> jsonSerializers;
        private readonly string dbId = "webstudio";


        public static SettingsManager Instance
        {
            get;
            private set;
        }


        static SettingsManager()
        {
            Instance = new SettingsManager();
        }

        private SettingsManager()
        {
            cache = new AspCache();
            expirationTimeout = TimeSpan.FromMinutes(5);
            jsonSerializers = new Dictionary<Type, DataContractJsonSerializer>();
        }


        public bool SaveSettings<T>(T settings, int tenantID) where T : ISettings
        {
            return SaveSettingsFor<T>(settings, tenantID, Guid.Empty);
        }

        public bool SaveSettingsFor<T>(T settings, Guid userID) where T : ISettings
        {
            return SaveSettingsFor<T>(settings, TenantProvider.CurrentTenantID, userID);
        }

        public T LoadSettings<T>(int tenantID) where T : ISettings
        {
            return LoadSettingsFor<T>(tenantID, Guid.Empty);
        }

        public T LoadSettingsFor<T>(Guid userID) where T : ISettings
        {
            return LoadSettingsFor<T>(TenantProvider.CurrentTenantID, userID);
        }


        private bool SaveSettingsFor<T>(T settings, int tenantID, Guid userID) where T : ISettings
        {
            if (settings == null) throw new ArgumentNullException("settings");
            try
            {
                using (var db = GetDbManager())
                {
                    var data = Serialize(settings);
                    var defaultData = Serialize(settings.GetDefault());

                    ISqlInstruction i;
                    if (data.SequenceEqual(defaultData))
                    {
                        // remove default settings
                        i = new SqlDelete("webstudio_settings")
                            .Where("id", settings.ID.ToString())
                            .Where("tenantid", tenantID)
                            .Where("userid", userID.ToString());
                    }
                    else
                    {
                        i = new SqlInsert("webstudio_settings", true)
                            .InColumnValue("id", settings.ID.ToString())
                            .InColumnValue("userid", userID.ToString())
                            .InColumnValue("tenantid", tenantID)
                            .InColumnValue("data", data);
                    }
                    db.ExecuteNonQuery(i);
                }
                ToCache(tenantID, userID, settings);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        private T LoadSettingsFor<T>(int tenantID, Guid userID) where T : ISettings
        {
            var settingsInstance = (ISettings)Activator.CreateInstance<T>();
            var settings = FromCache(settingsInstance.ID, tenantID, userID);
            
            if (settings != null)
            {
                return (T)settings;
            }

            try
            {
                using (var db = GetDbManager())
                {
                    var q = new SqlQuery("webstudio_settings")
                        .Select("data")
                        .Where("id", settingsInstance.ID.ToString())
                        .Where("tenantid", tenantID)
                        .Where("userid", userID.ToString());
                    
                    var result = db.ExecuteScalar<string>(q);
                    if (result != null)
                    {
                        settings = Deserialize<T>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            if (settings == null)
            {
                settings = settingsInstance.GetDefault();
            }
            if (settings == null)
            {
                throw new InvalidOperationException(string.Format("Default settings of type '{0}' can not be null.", typeof(T)));
            }

            ToCache(tenantID, userID, settings);
            return (T)settings;
        }

        private ISettings Deserialize<T>(string data)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var settings = GetJsonSerializer(typeof(T)).ReadObject(stream);
                return (ISettings)settings;
            }
        }

        private string Serialize(ISettings settings)
        {
            using (var stream = new MemoryStream())
            {
                GetJsonSerializer(settings.GetType()).WriteObject(stream, settings);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private void ToCache(int tenantID, Guid userID, ISettings settings)
        {
            var key = settings.ID.ToString() + tenantID.ToString() + userID.ToString();
            cache.Insert(key, settings, DateTime.UtcNow.Add(expirationTimeout));
        }

        private ISettings FromCache(Guid settingsID, int tenantID, Guid userID)
        {
            var key = settingsID.ToString() + tenantID.ToString() + userID.ToString();
            return cache.Get(key) as ISettings;
        }

        private DbManager GetDbManager()
        {
            return DbManager.FromHttpContext(dbId);
        }

        private DataContractJsonSerializer GetJsonSerializer(Type type)
        {
            lock (jsonSerializers)
            {
                if (!jsonSerializers.ContainsKey(type))
                {
                    jsonSerializers[type] = new DataContractJsonSerializer(type);
                }
                return jsonSerializers[type];
            }
        }
    }
}
