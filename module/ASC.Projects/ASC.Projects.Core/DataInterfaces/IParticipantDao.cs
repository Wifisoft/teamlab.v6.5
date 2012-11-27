using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IParticipantDao
    {
        int[] GetFollowingProjects(Guid participant);

        int[] GetMyProjects(Guid participant);

        List<int> GetInterestedProjects(Guid participant);

        void AddToFollowingProjects(int project, Guid participant);

        void RemoveFromFollowingProjects(int project, Guid participant);

        DateTime? WhenReaded(Guid participant, string uniqueId);

        List<bool> IsReaded(Guid participant, List<ProjectEntity> entities);

        void Read(Guid participant, string uniqueId, DateTime when);
    }
}
