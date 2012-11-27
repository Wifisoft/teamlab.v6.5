using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;
using System.Collections;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IProjectDao
    {
        List<Project> GetAll(ProjectStatus? status, int max);

        List<Project> GetLast(ProjectStatus? status, int offset, int max);
        
        List<Project> GetByParticipiant(Guid participantId, ProjectStatus status);

        List<Project> GetFollowing(Guid participantId);


        List<Project> GetUpdates(DateTime from, DateTime to);
            
        Project GetById(int projectId);

        List<Project> GetById(ICollection projectIDs);

        bool IsExists(int projectId);

        
        int Count();

        List<int> GetTaskCount(List<int> projectId, params TaskStatus[] taskStatus);

        int GetMessageCount(int projectId);
        
        int GetTotalTimeCount(int projectId);
        
        int GetMilestoneCount(int projectId, params MilestoneStatus[] milestoneStatus);

        Project Save(Project project);

        void Delete(int projectId);


        void AddToTeam(int projectId, Guid participantId);

        void RemoveFromTeam(int projectId, Guid participantId);

        bool IsInTeam(int projectId, Guid participantId);

        List<Participant> GetTeam(int projectId);

        List<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to); 

        void SetTeamSecurity(int projectId, Guid participantId, ProjectTeamSecurity teamSecurity);

        ProjectTeamSecurity GetTeamSecurity(int projectId, Guid participantId);


        List<Project> GetByFilter(TaskFilter filter);

    }
}
