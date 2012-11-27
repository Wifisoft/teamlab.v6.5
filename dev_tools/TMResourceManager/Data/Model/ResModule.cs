using System.Collections.Generic;

namespace TMResourceData.Model
{
    public class ResModule
    {
        public string Name { get; set; }
        public bool IsLock { get; set; }
        public List<ResWord> ListWords { get; set; }
        public Dictionary<WordStatusEnum, int> Counts { get; set; }

        public ResModule()
        {
            ListWords = new List<ResWord>();
            Counts = new Dictionary<WordStatusEnum, int>
                         {
                             {WordStatusEnum.Translated, 0},
                             {WordStatusEnum.Changed, 0},
                             {WordStatusEnum.All, 0},
                             {WordStatusEnum.Untranslated, 0}
                         };
        }
    }
}
