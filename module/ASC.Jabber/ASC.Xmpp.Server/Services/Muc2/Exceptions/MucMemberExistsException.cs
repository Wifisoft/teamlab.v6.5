using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Services.Muc2.Exceptions
{
    using System;

    public class MucMemberExistsException : Exception
    {
        public Error GetError()
        {
            return new Error(ErrorCondition.Conflict);
        }
    }


    public class MucMemberNotFoundException : Exception
    {
        public Error GetError()
        {
            return new Error(ErrorCondition.ItemNotFound);
        }
    }
}