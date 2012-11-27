using System.Collections.Generic;

namespace TMResourceData.Model
{
    public class ResWord
    {
        public ResFile ResFile { get; set; }
        public WordStatusEnum Status { get; set; }

        public string Title { get; set; }
        public string ValueFrom { get; set; }
        public string ValueTo { get; set; }
        public string TextComment { get; set; }
        public string Link { get; set; }
        public List<string> Alternative { get; set; }

        public int Flag { get; set; }
    }
}
