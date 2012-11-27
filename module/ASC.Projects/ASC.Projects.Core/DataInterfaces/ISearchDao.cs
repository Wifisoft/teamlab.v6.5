using System.Collections;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ISearchDao
    {
         IList Search(string text, int projectId);
    }
}
