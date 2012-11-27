#region Import

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ASC.Data.Storage;
using ASC.Web.Core.Users.Activity;
using AjaxPro;
using System.Linq;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class DealActionView : BaseUserControl
    {
        #region Members

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/DealActionView.ascx"); }
        }

        public Deal TargetDeal { get; set; }

        protected bool HavePermission { get; set; }

        protected List<Int32> MembersIDs { get; set; }
        protected List<Int32> ExcludedIDs { get; set; }
        protected bool ShowMembersPanel { get; set; }

        #endregion

        #region Events

        public bool IsSelectedBidCurrency(String abbreviation)
        {
            if (TargetDeal != null)
                return String.Compare(abbreviation, TargetDeal.BidCurrency) == 0;

            return String.Compare(abbreviation, Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(ContactSelector));

            MembersIDs = new List<Int32>();
            ExcludedIDs = new List<int>();

            if (TargetDeal != null)
            {
                HavePermission = CRMSecurity.IsAdmin || TargetDeal.CreateBy == SecurityContext.CurrentAccount.ID;
                
                ExcludedIDs = Global.DaoFactory.GetDealDao().GetMembers(TargetDeal.ID).ToList();
                MembersIDs = new List<int>(ExcludedIDs);
                if (TargetDeal.ContactID != 0) MembersIDs.Remove(TargetDeal.ContactID);
            }
            else
            {
                HavePermission = true;
            }
            
            if (IsPostBack) return;

            List<CustomField> data;
            if (TargetDeal == null)
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Opportunity);
                saveDealButton.Text = CRMDealResource.AddThisDealButton;
                saveAndCreateDealButton.Text = CRMDealResource.AddThisAndCreateDealButton;

                userSelector.SelectedUserId = SecurityContext.CurrentAccount.ID;

                cancelButton.Attributes.Add("href",
                                            Request.UrlReferrer != null
                                                ? Request.UrlReferrer.OriginalString
                                                : "deals.aspx");
            }
            else
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, TargetDeal.ID, true);
                saveDealButton.Text = CRMCommonResource.SaveChanges;
                
                if (TargetDeal.ResponsibleID != Guid.Empty)
                    userSelector.SelectedUserId = TargetDeal.ResponsibleID;

                Page.JsonPublisher(TargetDeal, "targetDeal");

                cancelButton.Attributes.Add("href", String.Format("deals.aspx?id={0}", TargetDeal.ID));
            }

            Page.JsonPublisher(data, "customFieldList");
            Page.JsonPublisher(Global.DaoFactory.GetDealMilestoneDao().GetAll(), "dealMilestones");

            InitPrivatePanel();
            InitDealClientSelector();
            InitDealMembersSelector();
        }

        protected void InitPrivatePanel()
        {
            var cntrlPrivatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            cntrlPrivatePanel.CheckBoxLabel = CRMDealResource.PrivatePanelCheckBoxLabel;

            if (TargetDeal != null)
            {
                cntrlPrivatePanel.IsPrivateItem = CRMSecurity.IsPrivate(TargetDeal);
                if (cntrlPrivatePanel.IsPrivateItem)
                    cntrlPrivatePanel.SelectedUsers = CRMSecurity.GetAccessSubjectTo(TargetDeal);
            }

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode(), CRMDealResource.ResponsibleDeal };

            cntrlPrivatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            cntrlPrivatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            phPrivatePanel.Controls.Add(cntrlPrivatePanel);
        
        }

        protected void InitDealClientSelector()
        {
            var client = new List<Contact>();
            var hasTargetClient = false;
            if (TargetDeal != null && TargetDeal.ContactID != 0)
            {
                var contact = Global.DaoFactory.GetContactDao().GetByID(TargetDeal.ContactID);
                if(contact!=null)
                    client.Add(contact);
            }
            else
            {
                var URLContactID = UrlParameters.ContactID;
                if (URLContactID != 0)
                {
                    var target = Global.DaoFactory.GetContactDao().GetByID(URLContactID);
                    if (target != null)
                    {
                        client.Add(target);
                        hasTargetClient = true;
                    }
                }
            }

            var clientSelector = (ContactSelector)LoadControl(ContactSelector.Location);
            clientSelector.CurrentType = ContactSelector.SelectorType.CompaniesAndPersonsWithoutCompany;
            clientSelector.SelectedContacts = client;
            clientSelector.ExcludedArrayIDs = MembersIDs;
            clientSelector.ID = "dealClientSelector";
            clientSelector.ShowAddButton = true;
            clientSelector.ShowChangeButton = hasTargetClient ? false : true; //: TargetDeal == null || TargetDeal.ContactID == 0;
            clientSelector.ShowContactImg = true;
            clientSelector.DescriptionText = CRMCommonResource.FindContactByName;
            clientSelector.ShowNewCompanyContent = true;
            clientSelector.ShowNewContactContent = true;
            clientSelector.DeleteContactText = CRMCommonResource.DeleteParticipant;
            phDealClient.Controls.Add(clientSelector);
        }

        protected void InitDealMembersSelector()
        {
            var members = new List<Contact>();
            if (TargetDeal != null)
            {
                members.AddRange(Global.DaoFactory.GetContactDao().GetContacts(MembersIDs.ToArray()));
                if (members.Count > 0) ShowMembersPanel = true;
            }
            var memberSelector = (ContactSelector)LoadControl(ContactSelector.Location);
            memberSelector.CurrentType = ContactSelector.SelectorType.All;
            memberSelector.SelectedContacts = members;
            if (TargetDeal != null && TargetDeal.ContactID != 0)
                memberSelector.ExcludedArrayIDs = new List<int> { TargetDeal.ContactID };
            memberSelector.ID = "dealMemberSelector";
            memberSelector.ShowAddButton = true;
            memberSelector.ShowChangeButton = true;
            memberSelector.ShowContactImg = true;
            memberSelector.ShowAllDeleteButton = true;
            memberSelector.DescriptionText = CRMCommonResource.FindContactByName;
            memberSelector.ShowNewCompanyContent = true;
            memberSelector.ShowNewContactContent = true;
            memberSelector.DeleteContactText = CRMCommonResource.DeleteParticipant;
            memberSelector.AddButtonText = CRMCommonResource.AddParticipant;
            phDealMembers.Controls.Add(memberSelector);
        }

        protected void SaveOrUpdateDeal(Object sender, CommandEventArgs e)
        {
            int dealID;

            var deal = new Deal
                           {
                               Title = Request["nameDeal"],
                               Description = Request["descriptionDeal"],
                               DealMilestoneID = Convert.ToInt32(Request["dealMilestone"])
                           };
        
            int contactID;

            if (int.TryParse(Request["selectedContactID"], out contactID))
                deal.ContactID = contactID;

            int probability;

            if (int.TryParse(Request["probability"], out probability))
                deal.DealMilestoneProbability = probability;

            deal.BidCurrency = Request["bidCurrency"];

            if (String.IsNullOrEmpty(deal.BidCurrency))
                deal.BidCurrency = Global.TenantSettings.DefaultCurrency.Abbreviation;

            if (!String.IsNullOrEmpty(Request["bidValue"]))
            {
               
                decimal bidValue;

                if (!decimal.TryParse(Request["bidValue"],   out bidValue))
                    bidValue = 0;

                deal.BidValue = bidValue;

             

                deal.BidType = (BidType)Enum.Parse(typeof(BidType), Request["bidType"]);

                if (deal.BidType != BidType.FixedBid)
                {
                    int perPeriodValue;

                    if (int.TryParse(Request["perPeriodValue"], out perPeriodValue))
                        deal.PerPeriodValue = perPeriodValue;
                }

            }
            else
            {
                deal.BidValue = 0;
                deal.BidType = BidType.FixedBid;
            }

            DateTime expectedCloseDate;

            if (!DateTime.TryParse(Request["expectedCloseDate"], out expectedCloseDate))
                expectedCloseDate = DateTime.MinValue;
            
            deal.ExpectedCloseDate = expectedCloseDate;

            deal.ResponsibleID = new Guid(Request["responsibleID"]);

            var dealMilestone = Global.DaoFactory.GetDealMilestoneDao().GetByID(deal.DealMilestoneID);

            if (TargetDeal == null)
            {

                if (dealMilestone.Status != DealMilestoneStatus.Open)
                    deal.ActualCloseDate = TenantUtil.DateTimeNow();


                dealID = Global.DaoFactory.GetDealDao().CreateNewDeal(deal);
                deal.ID = dealID;
                deal.CreateBy = ASC.Core.SecurityContext.CurrentAccount.ID;
                deal.CreateOn = TenantUtil.DateTimeNow();
                deal = Global.DaoFactory.GetDealDao().GetByID(dealID);
                
                SetPermission(deal);
             
                TimeLinePublisher.Deal(deal, CRMCommonResource.ActionText_Create, UserActivityConstants.ContentActionType, UserActivityConstants.ImportantContent);
            }
            else
            {
                dealID = TargetDeal.ID;
                deal.ID = TargetDeal.ID;
                deal.ActualCloseDate = TargetDeal.ActualCloseDate;

                if (TargetDeal.DealMilestoneID != deal.DealMilestoneID)
                    deal.ActualCloseDate = dealMilestone.Status != DealMilestoneStatus.Open ? TenantUtil.DateTimeNow() : DateTime.MinValue;                
                
                Global.DaoFactory.GetDealDao().EditDeal(deal);
                deal = Global.DaoFactory.GetDealDao().GetByID(dealID);
                SetPermission(deal);

                TimeLinePublisher.Deal(deal, CRMCommonResource.ActionText_Update, UserActivityConstants.ActivityActionType, UserActivityConstants.ImportantActivity);
            }

            var dealMembers = !String.IsNullOrEmpty(Request["selectedMembersID"])
                                                          ? Request["selectedMembersID"].Split(new[] {','}).Select(
                                                              id => Convert.ToInt32(id)).Where(id => id != deal.ContactID).ToList()
                                                          : new List<int>();

            if (deal.ContactID > 0 && !dealMembers.Contains(deal.ContactID))
                dealMembers.Add(deal.ContactID);

            Global.DaoFactory.GetDealDao().SetMembers(dealID, dealMembers.ToArray());


            foreach (var customField in Request.Form.AllKeys)
            {
                if (!customField.StartsWith("customField_")) continue;

                var fieldID = Convert.ToInt32(customField.Split('_')[1]);

                var fieldValue = Request.Form[customField];

                if (String.IsNullOrEmpty(fieldValue) && TargetDeal == null)
                    continue;

                Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Opportunity,dealID, fieldID, fieldValue);

            }

            if (TargetDeal == null && UrlParameters.ContactID != 0)
                Response.Redirect(String.Format("default.aspx?id={0}#deals", UrlParameters.ContactID));

            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("deals.aspx?id={0}", dealID)
                                  : "deals.aspx?action=manage");
        }

        #endregion

        #region Methods

        protected void SetPermission(Deal deal)
        {
            if (CRMSecurity.IsAdmin || deal.CreateBy == SecurityContext.CurrentAccount.ID)
            {
                var isPrivate = Convert.ToBoolean(Request.Form["isPrivateDeal"]);
                var notifyPrivateUsers = Convert.ToBoolean(Request.Form["notifyPrivateUsers"]);

                if (isPrivate)
                {
                    var selectedUsers = Request.Form["selectedPrivateUsers"]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList();

                    selectedUsers.Add(SecurityContext.CurrentAccount.ID);
                    selectedUsers.Add(new Guid(Request["responsibleID"]));

                    CRMSecurity.SetAccessTo(deal, selectedUsers);

                    if (notifyPrivateUsers)
                        Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Opportunity, deal.ID, selectedUsers.ToArray());

                }
                else
                {
                    CRMSecurity.MakePublic(deal);
                }
            }
        }

        #endregion
    }
}