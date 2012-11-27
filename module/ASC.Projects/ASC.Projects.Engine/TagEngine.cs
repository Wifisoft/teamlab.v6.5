using System.Collections.Generic;
using System.Linq;
using ASC.Projects.Core.DataInterfaces;

namespace ASC.Projects.Engine
{
    public class TagEngine
    {
        private readonly ITagDao _tagDao;


        public TagEngine(IDaoFactory daoFactory)
        {
            _tagDao = daoFactory.GetTagDao();
        }


        public Dictionary<int, string> GetTags()
        {
            return _tagDao.GetTags();
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            return _tagDao.GetTags(prefix);
        }

        
        public int[] GetTagProjects(string tag)
        {
            return _tagDao.GetTagProjects(tag);
        }


        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            return _tagDao.GetProjectTags(projectId);
        }

        public Dictionary<int, string> GetProjectRequestTags(int requestId)
        {
            return _tagDao.GetProjectRequestTags(requestId);
        }

        public void SetProjectTags(int projectId, string tags)
        {
            _tagDao.SetProjectTags(projectId, FromString(tags));
        }

        public void SetProjectRequestTags(int requestId, string tags)
        {
            _tagDao.SetProjectRequestTags(requestId, FromString(tags));
        }

        private string[] FromString(string tags)
        {
            return (tags ?? string.Empty)
                .Split(',', ';')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
        }
    }
}
