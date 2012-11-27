using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Api.Comments.Dao;
using ASC.Api.Comments.Model;
using ASC.Api.Employee;
using ASC.Specific;
using NUnit.Framework;

namespace ASC.Api.Comments.Tests
{
    [TestFixture]
    public class CommentRepositoryTests
    {
        private CommentRepository _repository;

        [TestFixtureSetUp]
        public void Setup()
        {
            _repository = new CommentRepository("comments",
                                                new ConnectionStringSettings("comments",
                                                                             "Server=teamlab;Database=Test;UserID=dev;Pwd=dev;pooling=True;Character Set=utf8",
                                                                             "MySql.Data.MySqlClient"));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _repository.DeleteTree("testkey");
        }

        [Test]
        public void TestAdd()
        {
            var comment = new Comment
                              {
                                  Author = EmployeeWraper.GetSample(),
                                  Content = "Test comment",
                                  Created = (ApiDateTime) DateTime.UtcNow,
                                  Deleted = false,
                                  Key = "testkey",
                                  Updated = null
                              };
            _repository.SaveOrUpdate(comment);
            Assert.AreNotEqual(comment.Id, 0);
            Comment retrieveComment = _repository.Get(comment.Id);
            Assert.AreEqual(comment.Id, retrieveComment.Id);
            Assert.AreEqual(comment.Content, retrieveComment.Content);
        }

        [Test]
        public void TestGet()
        {
            IEnumerable<Comment> comments = _repository.GetAll("testkey");
            Assert.IsTrue(comments.Count() > 0);
        }

        [Test]
        public void TestGetCount()
        {
            long count = _repository.GetCount("testkey");
            CommentCount countWithUsers = _repository.GetCountWithReaded("testkey", Core.Users.Constants.LostUser.ID);
        }

        [Test]
        public void TestGetWithUser()
        {
            IEnumerable<Comment> comments = _repository.GetAll("testkey", Core.Users.Constants.LostUser.ID);
            Assert.IsTrue(comments.All(x => x.Readed == null));
            _repository.MarkAsReaded("testkey", Core.Users.Constants.LostUser.ID);
            IEnumerable<Comment> commentsReaded = _repository.GetAll("testkey", Core.Users.Constants.LostUser.ID);
            Assert.IsTrue(commentsReaded.All(x => x.Readed != null));
        }
    }
}