using System;
using ASC.Files.Core.Security;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileShareParams
    {
        /// <summary>
        /// </summary>
        public Guid ShareTo { get; set; }
        
        /// <summary>
        /// </summary>
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public AceWrapper ToAceObject()
        {
            return new AceWrapper
                          {
                              Share = Access,
                              SubjectId = ShareTo,
                              SubjectGroup = !Core.CoreContext.UserManager.UserExists(ShareTo)
                          };
        }
    }
}