using System.Collections.Generic;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Thrdparty.TokenManagers
{
    public interface IAssociatedTokenManager : IConsumerTokenManager
    {
        IEnumerable<string> GetAcessTokens();
        IEnumerable<string> GetAssociations();
        string GetAssociation(string token);
        IEnumerable<string> GetTokensByAssociation(string associateData);
        void AssociateToken(string token, string associateData);

        void RemoveAssociationFromToken(string token);
        void ExpireToken(string token);
    }
}