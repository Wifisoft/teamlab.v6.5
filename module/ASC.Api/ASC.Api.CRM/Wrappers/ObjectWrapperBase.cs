#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.CRM.Core;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Namespace = "")]
    public class ObjectWrapperBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        public ObjectWrapperBase(int id)
        {
            ID = id;
        }



    }

}
