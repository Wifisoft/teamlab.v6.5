#region Usings

using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Data.DAO;

#endregion

namespace ASC.Projects.Data
{
    public class DaoFactory : IDaoFactory
    {
        private readonly string dbId;
        private readonly int tenant;


        public DaoFactory(string dbId, int tenant)
        {
            this.dbId = dbId;
            this.tenant = tenant;
        }


        public IProjectDao GetProjectDao()
        {
            return new CachedProjectDao(dbId, tenant);
        }

        public IParticipantDao GetParticipantDao()
        {
            return new ParticipantDao(dbId, tenant);
        }

        public IMilestoneDao GetMilestoneDao()
        {
            return new CachedMilestoneDao(dbId, tenant);
        }

        public ITaskDao GetTaskDao()
        {
            return new CachedTaskDao(dbId, tenant);
        }

        public ISubtaskDao GetSubtaskDao()
        {
            return new CachedSubtaskDao(dbId, tenant);
        }

        public IMessageDao GetMessageDao()
        {
            return new CachedMessageDao(dbId, tenant);
        }

        public ICommentDao GetCommentDao()
        {
            return new CommentDao(dbId, tenant);
        }

        public ITemplateDao GetTemplateDao()
        {
            return new TemplateDao(dbId, tenant);
        }

        public ITimeSpendDao GetTimeSpendDao()
        {
            return new TimeSpendDao(dbId, tenant);
        }

        public IReportDao GetReportDao()
        {
            return new ReportDao(dbId, tenant);
        }

        public IEventDao GetEventDao()
        {
            return new EventDao(dbId, tenant);
        }

        public ISearchDao GetSearchDao()
        {
            return new SearchDao(dbId, tenant);
        }

        public ITagDao GetTagDao()
        {
            return new TagDao(dbId, tenant);
        }

        public IIssueDao GetIssueDao()
        {
            return new IssueDAO(dbId, tenant);
        }
    }
}
