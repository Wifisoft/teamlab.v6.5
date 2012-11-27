using System.Collections.Generic;

namespace TMResourceData.Model
{
    public class StatisticModule
    {
        public string Culture { get; set; }
        public Dictionary<string, int> Counts { get; set; }

        public StatisticModule()
        {
            Counts = new Dictionary<string, int>();
        }
    }
}
