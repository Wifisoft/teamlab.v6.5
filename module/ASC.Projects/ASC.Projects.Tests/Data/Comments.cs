using System;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Comments
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Projects.Tests");


        [Test]
        public void CountTest()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Task task = daoFactory.GetTaskDao().GetById(1337);
            Console.WriteLine(daoFactory.GetCommentDao().Count(task));
        }

        [Test]
        public void SaveOrUpdate()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Task task = daoFactory.GetTaskDao().GetById(25);

            Comment comment = new Comment
                                  {
                                      Content = "ghb",
                                      TargetUniqID = typeof(Task).Name + task.ID.ToString(),
                                  };

            // task.Comments.Add(comment);

            daoFactory.GetCommentDao().Save(comment);

            Console.WriteLine(comment.ID);
        }
    }
}
