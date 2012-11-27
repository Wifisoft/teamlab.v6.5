using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Comments.Dao;
using ASC.Api.Comments.Model;
using ASC.Api.Employee;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Specific;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Comments
{
    public class CommentApi : IApiEntryPoint
    {
        private readonly CommentRepository _repository;
        private ApiContext _context;

        public CommentApi(ApiContext context)
        {
            _context = context;
            _repository = new CommentRepository(Name, ConfigurationManager.ConnectionStrings[Name]);
        }

        protected static Guid CurrentUserId
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        #region IApiEntryPoint Members


        public string Name
        {
            get { return "comment"; }
        }

        #endregion

        [Read("{key}")]
        public IEnumerable<Comment> GetComments(string key)
        {
            SmartList<Comment> comments = _repository.GetAll(key, CurrentUserId).ToSmartList();
            _repository.MarkAsReaded(key, CurrentUserId);
            return comments;
        }

        [Read("{key}/count")]
        public CommentCount GetCommentsCount(string key)
        {
            return _repository.GetCountWithReaded(key, CurrentUserId);
        }

        [Create("{key}")]
        [Poll]
        public Comment AddComment(string key, string content, long? parentId)
        {
            var comment = new Comment
                              {
                                  Author = EmployeeWraper.Get(CurrentUserId),
                                  Content = content,
                                  Created = (ApiDateTime) DateTime.UtcNow,
                                  Updated = (ApiDateTime) DateTime.UtcNow,
                                  Key = key,
                                  ParentId = parentId,
                              };
            _repository.SaveOrUpdate(comment);
            return comment;
        }

        [Update("{key}/{id}")]
        public Comment UpdateComment(string key, long id, string content)
        {
            Comment comment = _repository.Get(id);
            if (comment.Author.Id == CurrentUserId) //TODO: check admin or security rights
            {
                comment.Content = content;
                comment.Updated = (ApiDateTime) DateTime.UtcNow;
                _repository.SaveOrUpdate(comment);
            }
            else
            {
                throw new SecurityException("Unauthorized");
            }
            return comment;
        }

        [Delete("{key}/{id}")]
        public void DeleteComment(string key, long id)
        {
            Comment comment = _repository.Get(id);
            if (comment.Author.Id == CurrentUserId) //TODO: check admin or security rights
            {
                _repository.Delete(comment);
            }
            else
            {
                throw new SecurityException("Unauthorized");
            }
        }
    }
}