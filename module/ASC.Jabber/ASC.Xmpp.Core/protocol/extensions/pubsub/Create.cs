// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Create.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class Create : PubSubAction
    {
        #region << Constructors >>

        public Create()
        {
            TagName = "create";
        }

        public Create(string node) : this()
        {
            Node = node;
        }

        public Create(Type type) : this()
        {
            Type = type;
        }

        public Create(string node, Type type) : this(node)
        {
            Type = type;
        }

        #endregion

        /*
        <iq type="set"
            from="pgm@jabber.org"
            to="pubsub.jabber.org"
            id="create1">
            <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <create node="generic/pgm-mp3-player"/>
                <configure/>
            </pubsub>
        </iq>
         
        ...
            <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <create node="test"
	                    type="collection"/>
            </pubsub>
        ...
        
        */
    }
}