#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    [DataContract(Name = "historyCategory", Namespace = "")]
    public class HistoryCategoryWrapper : ListItemWrapper
    {

        public HistoryCategoryWrapper():base(0)
        {
            
        }

        public HistoryCategoryWrapper(ListItem listItem)
            : base(listItem)
        {
            if (!String.IsNullOrEmpty(listItem.AdditionalParams))
                ImagePath = WebImageSupplier.GetAbsoluteWebPath(listItem.AdditionalParams, ProductEntryPoint.ID);
        }

        [DataMember]
        public String ImagePath { get; set; }
        
        public static HistoryCategoryWrapper GetSample()
        {
           return new HistoryCategoryWrapper
                       {
                           ID = 30,
                           Title = "Lunch",
                           SortOrder = 10,
                           Color = String.Empty,
                           Description = "",
                           ImagePath = "path to image"
                       };
        }
    }

    [DataContract(Name = "opportunityStages", Namespace = "")]
    public class DealMilestoneWrapper : ListItemWrapper
    {
        public DealMilestoneWrapper()
            : base(0)
        {
            
        }


        public DealMilestoneWrapper(DealMilestone dealMilestone)
            : base(dealMilestone.ID)
        {

            SuccessProbability = dealMilestone.Probability;
            StageType = dealMilestone.Status;
            Color = dealMilestone.Color;
            Description = dealMilestone.Description;
            Title = dealMilestone.Title;
        }

        [DataMember]
        public int SuccessProbability { get; set; }

        [DataMember]
        public DealMilestoneStatus StageType { get; set; }

        public static DealMilestoneWrapper GetSample()
        {
            return new DealMilestoneWrapper
            {
                ID = 30,
                Title = "Discussion",
                SortOrder = 2,
                Color = "#B9AFD3",
                Description = "The potential buyer showed his/her interest and sees how your offering meets his/her goal",
                StageType = DealMilestoneStatus.Open,
                SuccessProbability = 20
            };
        }

    }


    [DataContract(Name = "taskCategory", Namespace = "")]
    public class TaskCategory : ListItemWrapper
    {

        public TaskCategory()
            :base(0)
        {
            
        }

        public TaskCategory(ListItem listItem)
            : base(listItem)
        {

            ImagePath = WebImageSupplier.GetAbsoluteWebPath(listItem.AdditionalParams, ProductEntryPoint.ID);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String ImagePath { get; set; }

        public static TaskCategory GetSample()
        {
            return new TaskCategory
            {
                ID = 30,
                Title = "Appointment",
                SortOrder = 2,
                Description = "",
                ImagePath = "path to image"
            };
        }
    }

    [DataContract(Name = "contactType", Namespace = "")]
    public class ContactType : ListItemWrapper
    {

        public ContactType():
            base(0)
        {
            
        }

        public ContactType(ListItem listItem)
            : base(listItem)
        {

        }

        public static ContactType GetSample()
        {
            return new ContactType
            {
                ID = 30,
                Title = "Cold",
                SortOrder = 2,
                Description = ""
            };
            
        }

    }

    [DataContract(Name = "listItem", Namespace = "")]
    public abstract class ListItemWrapper : ObjectWrapperBase
    {
        protected ListItemWrapper(int id)
            : base(id)
        {

        }

        protected ListItemWrapper(ListItem listItem)
            : base(listItem.ID)
        {
            Title = listItem.Title;
            Description = listItem.Description;
            Color = listItem.Color;
            SortOrder = listItem.SortOrder;

        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Color { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int SortOrder { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
       public int RelativeItemsCount { get; set; }
    
    }
}
