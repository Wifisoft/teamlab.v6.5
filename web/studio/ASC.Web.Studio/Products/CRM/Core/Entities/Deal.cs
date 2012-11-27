#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class Deal : DomainObject, ISecurityObjectId
    {

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }    

        [DataMember(Name = "contact_id")]
        public int ContactID { get; set; }

        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        [DataMember(Name = "responsible_id")]
        public Guid ResponsibleID { get; set; }

        [DataMember(Name = "bid_type")]
        public BidType BidType { get; set; }

        [DataMember(Name = "bid_value")]
        public decimal BidValue { get; set; }

        [DataMember(Name = "bid_currency")]
        public String BidCurrency { get; set; }
        
        [DataMember(Name = "per_period_value")]
        public int PerPeriodValue { get; set; }

        [DataMember(Name = "deal_milestone")]
        public int DealMilestoneID { get; set; }

        [DataMember(Name = "deal_milestone_probability")]
        public int DealMilestoneProbability { get; set; }

        public DateTime ActualCloseDate { get; set; }

        [DataMember(Name = "actual_close_date")]
        private String ActualCloseDateStr
        {
            get
            {
                if (ActualCloseDate.Date == DateTime.MinValue.Date)
                    return String.Empty;

                return ActualCloseDate.ToString(DateTimeExtension.DateFormatPattern);
            }
            set { }
        }

        
        public DateTime ExpectedCloseDate { get; set; }

        [DataMember(Name = "expected_close_date")]
        private String ExpectedCloseDateStr
        {
            get
            {
                if (ExpectedCloseDate.Date == DateTime.MinValue.Date)
                    return String.Empty;

                return ExpectedCloseDate.ToString(DateTimeExtension.DateFormatPattern);
            }
            set { }
        }
        
        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}