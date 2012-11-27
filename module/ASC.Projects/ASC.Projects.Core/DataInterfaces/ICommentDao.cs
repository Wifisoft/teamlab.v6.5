using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ICommentDao
    {
        List<Comment> GetAll(DomainObject<Int32> target);

        Comment GetById(Guid id);

        Comment GetLast(DomainObject<Int32> target);

        int Count(DomainObject<Int32> target);

        List<int> GetCommentsCount(List<ProjectEntity> targets);

        List<CommentUpdate> GetUpdates(DateTime from, DateTime to);
        
        Comment Save(Comment comment);

        void Delete(Guid id);
    }
}
