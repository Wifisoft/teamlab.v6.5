// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Ping.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.ping
{
    /*
     * <iq from='capulet.com' to='juliet@capulet.com/balcony' id='ping123' type='get'>
     *   <ping xmlns='urn:xmpp:ping'/>
     * </iq>
     */

    public class Ping : Element
    {
        public Ping()
        {
            TagName = "ping";
            Namespace = Uri.PING;
        }
    }
}