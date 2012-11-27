using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMResourceData.Model
{
    public class ResCurrent
    {
        public ResProject Project { get; set; }
        public ResModule Module { get; set; }
        public ResWord Word { get; set; }
        public ResCulture Language { get; set; }
        public Author Author { get; set; }
    }
}
