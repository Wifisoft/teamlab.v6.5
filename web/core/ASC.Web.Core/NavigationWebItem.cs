using System;
using System.Collections.Generic;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public class NavigationWebItem : IWebItem
    {
        public virtual Guid ID { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string StartURL { get; set; }

        public virtual WebItemContext Context { get; set; }

        
        public override bool Equals(object obj)
        {
            var m = obj as IWebItem;
            return m != null && ID == m.ID;
        }
        
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
