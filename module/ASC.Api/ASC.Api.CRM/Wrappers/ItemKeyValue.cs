using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ASC.Api.CRM.Wrappers
{
    [DataContract]
    public class ItemKeyValuePair<TKey, TValue> 
    {
        [DataMember]
        public TKey Key { get; set; }


        [DataMember]
        public TValue Value { get; set; }

    }
}
