using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using ASC.Notify.Cron;

namespace ASC.FullTextIndex.Service.Config
{
    class TextIndexCfg
    {
        private readonly string indexPath;


        public readonly static int MaxQueryLength = 30;


        public CronExpression ChangedCron
        {
            get;
            private set;
        }

        public CronExpression RemovedCron
        {
            get;
            private set;
        }

        public TimeSpan Delay
        {
            get;
            private set;
        }

        public IList<ModuleInfo> Modules
        {
            get;
            private set;
        }

        public ConnectionStringSettings ConnectionString
        {
            get;
            private set;
        }


        public TextIndexCfg()
        {
            var cfg = (TextIndexCfgSectionHandler)ConfigurationManager.GetSection("fullTextIndex");

            indexPath = cfg.IndexPath.Trim('\\').Trim();
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(indexPath))
            {
                indexPath = currentDirectory;
            }
            if (!Path.IsPathRooted(indexPath))
            {
                indexPath = Path.Combine(currentDirectory, indexPath);
            }

            ChangedCron = new CronExpression(cfg.ChangedCron);
            RemovedCron = new CronExpression(cfg.RemovedCron);
            Delay = TimeSpan.FromMilliseconds(cfg.Delay);
            ConnectionString = ConfigurationManager.ConnectionStrings[cfg.ConnectionStringName];
            Modules = cfg.Modules
                .Cast<TextIndexCfgModuleElement>()
                .Select(e => new ModuleInfo(e.Name, e.Select, ConfigurationManager.ConnectionStrings[e.ConnectionStringName] ?? ConnectionString))
                .ToList();
        }


        public string GetIndexPath(int tenantId, string module)
        {
            var path = Path.Combine(indexPath, tenantId.ToString("00/00/00", CultureInfo.InvariantCulture));
            path = Path.Combine(path, module);
            return path;
        }
    }
}
