// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Nickname.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.nickname
{
    // <nick xmlns='http://jabber.org/protocol/nick'>Ishmael</nick>
    public class Nickname : Element
    {
        public Nickname()
        {
            TagName = "nick";
            Namespace = Uri.NICK;
        }

        public Nickname(string nick) : this()
        {
            Value = nick;
        }
    }
}