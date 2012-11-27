using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class CommentEngine
    {
        private readonly ICommentDao dao;
        private readonly IParticipantDao pdao;


        public CommentEngine(IDaoFactory daoFactory)
        {
            dao = daoFactory.GetCommentDao();
            pdao = daoFactory.GetParticipantDao();
        }


        public List<Comment> GetComments(DomainObject<Int32> targetObject)
        {
            return targetObject != null ? dao.GetAll(targetObject) : new List<Comment>();
        }

        public Comment GetByID(Guid id)
        {
            return dao.GetById(id);
        }

        public Comment GetLast(DomainObject<Int32> targetObject)
        {
            return targetObject != null ? dao.GetLast(targetObject) : null;
        }

        public List<CommentUpdate> GetUpdates(DateTime from, DateTime to)
        {
            var comments = dao.GetUpdates(from, to);
            return comments.Where(x => ProjectSecurity.CanRead(new Project() {ID = x.ProjectId})).ToList();
        }

        public int Count(DomainObject<Int32> targetObject)
        {
            if (targetObject == null) return 0;
            return dao.Count(targetObject);
        }

        public List<int> GetCommentsCount(List<ProjectEntity> targets)
        {
            return dao.GetCommentsCount(targets);
        }

        public Comment SaveOrUpdate(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException("comment");

            if (!SecurityContext.CurrentAccount.IsAuthenticated) throw new System.Security.SecurityException();
            if (comment.CreateBy == default(Guid)) comment.CreateBy = SecurityContext.CurrentAccount.ID;

            DateTime now = TenantUtil.DateTimeNow();
            if (comment.CreateOn == default(DateTime)) comment.CreateOn = now;

            var newComment = dao.Save(comment);
            //mark entity as jast readed
            //
            pdao.Read(comment.CreateBy, comment.TargetUniqID, now);

            return newComment;
        }
    }
}
