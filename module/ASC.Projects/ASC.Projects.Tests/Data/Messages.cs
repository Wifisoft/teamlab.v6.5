using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Messages : TestBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void SaveOrUpdateTest()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var messageDao = daoFactory.GetMessageDao();

            var message = new Message()
            {
                Title = "New Message",
                Project = daoFactory.GetProjectDao().GetById(1),
                Content = "Content",
            };

            messageDao.Save(message);
        }
    }
}
