using System;
using System.Collections.Generic;

namespace ASC.Web.Core.ModuleManagement.Common
{
    public class StatisticItem
    {
        public long Count { get; set; }        
        
        public string Name { get; set; }

        public string URL { get; set; }

        public string Description { get; set; }

        public string AdditionalData { get; set; }
    }

    public interface IStatisticProvider
    {
        StatisticItem GetMainStatistic(Guid userID);

        List<StatisticItem> GetAllStatistic(Guid userID);
    }
}
