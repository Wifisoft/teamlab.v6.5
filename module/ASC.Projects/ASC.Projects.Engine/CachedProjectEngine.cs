using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Caching;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class CachedProjectEngine : ProjectEngine
    {
        private static readonly ICache cache = new AspCache();
        private static readonly TimeSpan cacheExpiration = TimeSpan.FromMinutes(10);


        public CachedProjectEngine(IDaoFactory daoFactory, EngineFactory factory)
            : base(daoFactory, factory)
        {
        }

        public override int Count()
        {
            var key = GetCountKey();
            var value = cache.Get(key);
            if (value != null)
            {
                return (int)value;
            }
            else
            {
                var count = base.Count();
                cache.Insert(key, count, DateTime.UtcNow.Add(cacheExpiration));
                return count;
            }
        }

        public override Project SaveOrUpdate(Project project, bool notifyManager, bool isImport)
        {
            var p = base.SaveOrUpdate(project, notifyManager, isImport);
            cache.Remove(GetCountKey());
            return p;
        }

        public override void Delete(int projectId)
        {
            base.Delete(projectId);
            cache.Remove(GetCountKey());
        }

        private string GetCountKey()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId + "/projects/count";
        }
    }
}
