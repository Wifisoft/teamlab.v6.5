using System;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using ASC.Projects.Engine;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{

    [TestFixture]
    public class Tasks : TestBase
    {
        private TaskEngine engine = new TaskEngine(new DaoFactory(DbId, 0), new EngineFactory(DbId, 0, null));


        public Tasks()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);
        }

        [Test]
        public void SaveOrUpdateTest()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var task = daoFactory.GetTaskDao().GetById(187);
            var participant = new Participant(SecurityContext.CurrentAccount.ID);

            Console.WriteLine(task.UniqID);
            daoFactory.GetParticipantDao().Read(participant.ID, task.UniqID, DateTime.Now);
            Console.WriteLine(daoFactory.GetParticipantDao().WhenReaded(participant.ID, task.UniqID));
        }

        [Test]
        public void BuildTaskListReport()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var result = daoFactory.GetReportDao().BuildTaskListReport(new ReportFilter());
            Console.WriteLine(result.Count);
        }

        [Test]
        public void BuildUsersWorkReport1()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var result = daoFactory.GetReportDao().BuildUsersWorkReport(new ReportFilter());
            Console.WriteLine(result.Count);

        }

        [Test]
        public void GetByResponsibleTest()
        {
            var tasks = engine.GetByResponsible(Guid.Empty);
        }
    }
}
