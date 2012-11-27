#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;
using ASC.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core;
using ASC.Core.Tenants;

#endregion

namespace ASC.Api.CRM
{    

    public class CRMCalendar : BaseCalendar
    {
        private class Event : BaseEvent { }
        
        private Guid _userId;
        private DaoFactory _daoFactory;

        public CRMCalendar(DaoFactory daoFactory, Guid userId)
        {   
            _userId = userId;
            _daoFactory = daoFactory;

            this.Context.HtmlBackgroundColor = "";
            this.Context.HtmlTextColor = "";
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = delegate() { return ASC.Web.CRM.Resources.CRMCommonResource.ProductName; };
            this.Id ="crm_calendar";
            this.EventAlertType = EventAlertType.Never;
            this.Name = ASC.Web.CRM.Resources.CRMCommonResource.ProductName;
            this.Description = "";
            this.SharingOptions = new SharingOptions(); 
            this.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem() { Id =userId, IsGroup = false });
        }

        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var events = new List<IEvent>();
            var tasks = _daoFactory.GetTaskDao().GetTasks(String.Empty, userId, 0, false, DateTime.MinValue, DateTime.MinValue, EntityType.Any, 0, 0, 0, null);

            foreach (var t in tasks)
            {
                if (t.DeadLine != DateTime.MinValue)
                {
                    var e = new Event()
                    {
                        AlertType = EventAlertType.Never,
                        CalendarId = this.Id,
                        UtcStartDate = TenantUtil.DateTimeToUtc(t.DeadLine),
                        UtcEndDate = TenantUtil.DateTimeToUtc(t.DeadLine),
                        Id = t.ID.ToString(),
                        Name = ASC.Web.CRM.Resources.CRMCommonResource.ProductName + ": " + t.Title,
                        Description = t.Description
                    };

                    if (t.DeadLine.Hour == 0 && t.DeadLine.Minute == 0)
                        e.AllDayLong = true;

                    events.Add(e);
                }
            }

            return events;
        }

        public override TimeZoneInfo TimeZone
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TimeZone; }
        }
    }
}
