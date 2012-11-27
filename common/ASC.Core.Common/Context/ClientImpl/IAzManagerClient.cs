using System;
using System.Collections.Generic;
using ASC.Common.Security;

namespace ASC.Core
{
    public interface IAzManagerClient
    {
        IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId);

        IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId, ISecurityObjectId objectId);

        IEnumerable<AzRecord> GetAcesWithInherits(Guid subjectId, Guid actionId, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider);

        void AddAce(AzRecord azRecord);

        void RemoveAce(AzRecord azRecord);

        void RemoveAllAces(ISecurityObjectId id);
    }
}