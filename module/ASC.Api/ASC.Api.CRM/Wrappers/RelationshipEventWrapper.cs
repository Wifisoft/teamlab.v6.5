#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Api.Documents;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    [DataContract(Name = "entity", Namespace = "")]
    public class EntityWrapper
    {
        [DataMember]
        public String EntityType { get; set; }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String EntityTitle { get; set; }

        public static EntityWrapper GetSample()
        {
            return new EntityWrapper
                       {
                           EntityId = 123445,
                           EntityType = "opportunity",
                           EntityTitle = "Интернет-магазин бытовой техники"
                       };
        }
    }


    [DataContract(Name = "historyEvent", Namespace = "")]
    public class RelationshipEventWrapper :
        ObjectWrapperBase
    {

        public RelationshipEventWrapper():
            base(0)
        {
            
        }

        public RelationshipEventWrapper(RelationshipEvent relationshipEvent)
            : base(relationshipEvent.ID)
        {

            CreateBy = EmployeeWraper.Get(relationshipEvent.CreateBy);
            Created = (ApiDateTime)relationshipEvent.CreateOn;
            Content = relationshipEvent.Content;
            Files = new List<FileWrapper>();
            CanEdit = CRMSecurity.CanEdit(relationshipEvent);

        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Content { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public HistoryCategoryWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<FileWrapper> Files { get; set; }

        public static RelationshipEventWrapper GetSample()
        {
            return new RelationshipEventWrapper
                        {
                           CanEdit = true,
                           Category = HistoryCategoryWrapper.GetSample(),
                           Entity = EntityWrapper.GetSample(),
                           Contact = ContactBaseWrapper.GetSample(),
                           Created = (ApiDateTime)DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample(),
                           Files = new[] {FileWrapper.GetSample()},
                           Content = @"Договорились встретиться с клиентом за обедом и обсудить коммерческое предложение
                                       "
                        };
        }

    }
}
