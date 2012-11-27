using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Specific;

namespace ASC.Api.Projects.Calendars
{
    public class ProjectCalendar : BaseCalendar
    {
        private class Event : BaseEvent{}

        private Project _project;        
        private EngineFactory _engine;
        private Guid _userId;
        private bool _following;

        public ProjectCalendar(EngineFactory engine, Guid userId, Project project, string backgroundColor,  string textColor, SharingOptions sharingOptions, bool following)
        {
            _project = project;
            _engine = engine;
            _userId = userId;
            _following = following;

            this.Context.HtmlBackgroundColor = backgroundColor;
            this.Context.HtmlTextColor = textColor;
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = () => ASC.Web.Projects.Resources.ProjectsCommonResource.ProductName;
            this.Id = _project.UniqID;
            this.EventAlertType = EventAlertType.Hour;
            this.Name = _project.Title;
            this.Description = _project.Description;
            this.SharingOptions = sharingOptions;            
        }
      
        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var events = new List<IEvent>();

            List<Task> tasks = new List<Task>();
            if (!_following)
                tasks = _engine.GetTaskEngine().GetByProject(_project.ID, TaskStatus.Open, userId);

            var milestones = _engine.GetMilestoneEngine().GetByProject(_project.ID);

            foreach(var m in milestones)
            {
                events.Add(new Event()
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = this.Id,
                    UtcStartDate = (ApiDateTime)m.DeadLine,
                    UtcEndDate = (ApiDateTime)m.DeadLine,
                    AllDayLong = true,
                    Id = m.UniqID,
                    Name = ASC.Web.Projects.Resources.MilestoneResource.Milestone + ": " + m.Title
                });
            }

            foreach(var t in tasks)
            {
                events.Add(new Event()
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = this.Id,
                    UtcStartDate = (ApiDateTime)t.Deadline,
                    UtcEndDate = (ApiDateTime)t.Deadline,
                    AllDayLong = true,
                    Id = t.UniqID,
                    Name = ASC.Web.Projects.Resources.TaskResource.Task +": " + t.Title,
                    Description = t.Description
                });
            }

            return events;
        }       

        public override TimeZoneInfo TimeZone
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TimeZone; }
        }
    }
}
