#region Import

using System;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.Core.Tenants;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.Studio.Utility;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Web.Studio.Controls.Common;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    [AjaxNamespace("AjaxPro.DealDetailsView")]
    public partial class DealDetailsView : BaseUserControl
    {

        #region Property

        public Deal TargetDeal { get; set; }

        public static String Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/DealDetailsView.ascx"); }
        }

        #endregion

        #region Methods

        protected String RenderExpectedValue()
        {
            switch (TargetDeal.BidType)
            {
                case BidType.PerYear:
                    return String.Concat(CRMDealResource.BidType_PerYear, " ",
                                         String.Format(CRMJSResource.PerPeriodYears, TargetDeal.PerPeriodValue));
                case BidType.PerWeek:
                    return String.Concat(CRMDealResource.BidType_PerWeek, " ",
                                         String.Format(CRMJSResource.PerPeriodWeeks, TargetDeal.PerPeriodValue));
                case BidType.PerMonth:
                    return String.Concat(CRMDealResource.BidType_PerMonth, " ",
                                         String.Format(CRMJSResource.PerPeriodMonths, TargetDeal.PerPeriodValue));
                case BidType.PerHour:
                    return String.Concat(CRMDealResource.BidType_PerHour, " ",
                                         String.Format(CRMJSResource.PerPeriodHours, TargetDeal.PerPeriodValue));
                case BidType.PerDay:
                    return String.Concat(CRMDealResource.BidType_PerDay, " ",
                                         String.Format(CRMJSResource.PerPeriodDays, TargetDeal.PerPeriodValue));
                default:
                    return String.Empty;
            }
        }

        protected String GetExpectedOrActualCloseDateStr()
        {
            if (TargetDeal.ActualCloseDate == DateTime.MinValue)
            {
                return TargetDeal.ExpectedCloseDate == DateTime.MinValue ? CRMJSResource.NoCloseDate : TargetDeal.ExpectedCloseDate.ToString(DateTimeExtension.ShortDatePattern);
            }
            return TargetDeal.ActualCloseDate.ToString(DateTimeExtension.ShortDatePattern);

        }

        protected String GetExpectedValueStr()
        {
            if (TargetDeal.BidValue == 0)
                return CRMDealResource.NoExpectedValue;

            var currencyInfo = CurrencyProvider.Get(TargetDeal.BidCurrency);

            return String.Format("{2}{0:N} {1} <br/> <span class='headerBaseSmall'>{3}</span>", TargetDeal.BidValue,
                                 currencyInfo.Abbreviation, currencyInfo.Symbol,
                                 RenderExpectedValue());

        }

        private void ExecTasksView()
        {
            TasksTab.TabAnchorName = "tasks";
            TasksTab.TabName = CRMTaskResource.Tasks;

            var ctrlListTaskView = (ListTaskView)LoadControl(ListTaskView.Location);
            ctrlListTaskView.CurrentContact = null;
            ctrlListTaskView.CurrentEntityType = EntityType.Opportunity;
            ctrlListTaskView.EntityID = TargetDeal.ID;
            ctrlListTaskView.HideContactSelector = true;

            if (CRMSecurity.IsPrivate(TargetDeal))
            {
                var users = CRMSecurity.GetAccessSubjectTo(TargetDeal).Keys.ToList<Guid>();
                //with admins
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();
                admins.AddRange(from u in users
                                where !CoreContext.UserManager.IsUserInGroup(u, Constants.GroupAdmin.ID)
                                select CoreContext.UserManager.GetUsers(u));
                ctrlListTaskView.UserList = admins.SortByUserName();
            }

            _phTasksView.Controls.Add(ctrlListTaskView);
        }

        private void ExecHistoryView()
        {
            EventsTab.TabName = CRMCommonResource.History;
            EventsTab.TabAnchorName = "history";

            var historyViewControl = (HistoryView) LoadControl(HistoryView.Location);
            historyViewControl.Title = String.Format(CRMDealResource.AddNoteToDeal, TargetDeal.Title.HtmlEncode());
            historyViewControl.TargetEntityID = TargetDeal.ID;
            historyViewControl.TargetEntityType = EntityType.Opportunity;

            _phHistoryView.Controls.Add(historyViewControl);
        }

        private void ExecPeopleView()
        {
            ContactsTab.TabAnchorName = "contacts";
            var people = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetDealDao().GetMembers(TargetDeal.ID));
            //ContactsTab.TabName = people.Count > 0 ? String.Format("{0} ({1})", CRMDealResource.PeopleInDeal, people.Count) : CRMDealResource.PeopleInDeal;
            ContactsTab.TabName = CRMDealResource.PeopleInDeal;
            //init ListContactView
            var listContactView = (ListContactView)LoadControl(ListContactView.Location);
            listContactView.IsSimpleView = true;
            listContactView.EntityID = TargetDeal.ID;
            listContactView.EntityType = EntityType.Opportunity;
            _phContactsView.Controls.Add(listContactView);

            //init ContactSelector
            var selector = (ContactSelector) LoadControl(ContactSelector.Location);
            selector.CurrentType = ContactSelector.SelectorType.All;
            selector.SelectedContacts = people;
            if (TargetDeal.ContactID != 0)
                selector.ExcludedArrayIDs = new List<int> {TargetDeal.ContactID};
            selector.ShowOnlySelectorContent = true;
            selector.ShowNewCompanyContent = true;
            selector.ShowNewContactContent = true;
            selector.ID = "dealContactSelector";
            selector.DescriptionText = CRMCommonResource.FindContactByName;
            phContactSelector.Controls.Add(selector);

            //init EmptyScreen
            var emptyParticipantScreenControl = new EmptyScreenControl
                                                    {
                                                        ID = "emptyDealParticipantPanel",
                                                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_opportunity_participants.png", ProductEntryPoint.ID),
                                                        Header = CRMDealResource.EmptyPeopleInDealContent,
                                                        Describe = CRMDealResource.EmptyPeopleInDealDescript,
                                                        ButtonHTML = String.Format(@"
                                                            <a class='linkAddMediumText baseLinkAction' onclick='javascript:jq(""#dealParticipantPanel"").show();jq(""#emptyDealParticipantPanel"").hide();'>{0}</a>",
                                                            CRMCommonResource.AddParticipant)

                                                    };
            _phEmptyPeopleView.Controls.Add(emptyParticipantScreenControl);

        }

        private void ExecFilesView()
        {
            FilesTab.TabAnchorName = "files";
            FilesTab.TabName = CRMCommonResource.Documents;

            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "opportunity";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
        }

        #region Ajax Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ChangeDealMilestone(int dealID, int dealMilestoneID)
        {
            var result = new AjaxResponse();
            var deal = Global.DaoFactory.GetDealDao().GetByID(dealID);
            var dealMilestone = Global.DaoFactory.GetDealMilestoneDao().GetByID(dealMilestoneID);

            if (dealMilestone.Status != DealMilestoneStatus.Open)
            {
                deal.ActualCloseDate = TenantUtil.DateTimeNow();
                result.rs2 = deal.ActualCloseDate.ToLocalTime().ToString(DateTimeExtension.ShortDatePattern);
                result.rs3 = "true"; //isClosed = true
            }
            else
            {
                deal.ActualCloseDate = DateTime.MinValue;
                result.rs2 = deal.ExpectedCloseDate != DateTime.MinValue ? deal.ExpectedCloseDate.ToLocalTime().ToString(DateTimeExtension.ShortDatePattern) : "";
                result.rs3 = "false"; //isClosed = false
            }

            deal.DealMilestoneID = dealMilestoneID;
            deal.DealMilestoneProbability = dealMilestone.Probability;

            Global.DaoFactory.GetDealDao().EditDeal(deal);
            result.rs1 = deal.DealMilestoneProbability.ToString();

            return result;
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (DealDetailsView));
            Utility.RegisterTypeForAjax(typeof (ContactSelector));

            FilesTab.TabAnchorName = "files";
            FilesTab.TabName = CRMCommonResource.Documents;


            Page.JsonPublisher(Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, TargetDeal.ID, false), "customFieldList");

            AllDealMilestonesRepeater.DataSource = Global.DaoFactory.GetDealMilestoneDao().GetAll();
            AllDealMilestonesRepeater.DataBind();


            var tagViewControl = (TagView) LoadControl(TagView.Location);
            tagViewControl.Tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Opportunity, TargetDeal.ID);
            tagViewControl.TargetEntityType = EntityType.Opportunity;

            phTagContainer.Controls.Add(tagViewControl);

            ExecHistoryView();
            ExecTasksView();
            ExecPeopleView();
            ExecFilesView();
        }


        #endregion

    }
}