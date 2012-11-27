
namespace ASC.Projects.Core.DataInterfaces
{
    public interface IDaoFactory
    {       
        IMilestoneDao GetMilestoneDao();

        IProjectDao GetProjectDao();
        
        ITaskDao GetTaskDao();

        ISubtaskDao GetSubtaskDao();

        IParticipantDao GetParticipantDao();
        
        IMessageDao GetMessageDao();
        
        ICommentDao GetCommentDao();
        
        ITimeSpendDao GetTimeSpendDao();
        
        ISearchDao GetSearchDao();
        
        IEventDao GetEventDao();
        
        ITagDao GetTagDao();

        IReportDao GetReportDao();

        ITemplateDao GetTemplateDao();

        IIssueDao GetIssueDao();
    }
}
