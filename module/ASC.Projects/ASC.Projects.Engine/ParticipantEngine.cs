using System;
using System.Collections.Generic;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class ParticipantEngine
    {
        private readonly IParticipantDao participantDao;


        public ParticipantEngine(IDaoFactory daoFactory)
        {
            participantDao = daoFactory.GetParticipantDao();
        }


        public Participant GetByID(Guid userID)
        {
            return new Participant(userID);
        }

        public void AddToFollowingProjects(int project, Guid participant)
        {
            participantDao.AddToFollowingProjects(project, participant);
        }

        public void RemoveFromFollowingProjects(int project, Guid participant)
        {
            participantDao.RemoveFromFollowingProjects(project, participant);
        }

        public List<int> GetInterestedProjects(Guid participant)
        {
            return participantDao.GetInterestedProjects(participant);
        }
        public List<int> GetFollowingProjects(Guid participant)
        {
            return new List<int>(participantDao.GetFollowingProjects(participant));
        }

        public List<int> GetMyProjects(Guid participant)
        {
            return new List<int>(participantDao.GetMyProjects(participant));
        }


        public DateTime? WhenReaded(Guid participant, string uniqueId)
        {
            return participantDao.WhenReaded(participant, uniqueId);
        }
        public List<bool> IsReaded(Guid participant, List<ProjectEntity> entities)
        {
            return participantDao.IsReaded(participant, entities);
        }
        public void Read(Guid participant, string uniqueId, DateTime when)
        {
            participantDao.Read(participant, uniqueId, when);
        }
    }
}