using System;

namespace ASC.CRM.Core.Entities
{
    public class OrderBy
    {
      
        public bool IsAsc
        {
            get;
            set;
        }

        
        public Enum SortedBy
        {
            get;
            set;
        }

        public OrderBy(Enum sortedByType, bool isAsc)
        {
            IsAsc = isAsc;
            SortedBy = sortedByType;
        }
    }
}
