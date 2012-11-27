// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Handler.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.component
{
    public delegate void MessageHandler(object sender, Message msg);

    public delegate void PresenceHandler(object sender, Presence pres);

    public delegate void IqHandler(object sender, IQ iq);
}