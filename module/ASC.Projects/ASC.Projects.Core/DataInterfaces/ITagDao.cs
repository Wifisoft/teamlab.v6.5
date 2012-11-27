
using System.Collections.Generic;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITagDao
    {
        Dictionary<int, string> GetTags();

        Dictionary<int, string> GetTags(string prefix);


        int[] GetTagProjects(string tag);


        Dictionary<int, string> GetProjectTags(int projectId);

        Dictionary<int, string> GetProjectRequestTags(int requestId);

        void SetProjectTags(int projectId, string[] tags);

        void SetProjectRequestTags(int requestId, string[] tags);
    }
}
