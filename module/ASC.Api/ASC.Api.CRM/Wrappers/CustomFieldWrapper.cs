#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core.Tenants;
using ASC.Specific;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Пользовательское поле данных
    /// </summary>
    [DataContract(Name = "customField", Namespace = "")]
    public class CustomFieldWrapper : ObjectWrapperBase
    {
        public CustomFieldWrapper(int id):base(id)
        {
            
        }

        public CustomFieldWrapper(CustomField customField)
            : base(customField.ID)
        {
            EntityId = customField.EntityID;
            Label = customField.Label;
            FieldValue = customField.Value;
            FieldType = customField.FieldType;
            Position = customField.Position;
            Mask = customField.Mask;
        }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String Label { get; set; }

        [DataMember]
        public String FieldValue { get; set; }

        [DataMember]
        public CustomFieldType FieldType { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public String Mask { get; set; }

        public static CustomFieldWrapper GetSample()
        {
            return  new CustomFieldWrapper(0)
                        {
                            Position = 10,
                            EntityId = 14523423,
                            FieldType = CustomFieldType.Date,
                            FieldValue = ((ApiDateTime)DateTime.UtcNow).ToString(),
                            Label = "Birthdate",
                            Mask = ""
                        };
        }
    }
}
