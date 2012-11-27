// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Bind.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.bind
{
    /// <summary>
    ///   Summary description for Bind.
    /// </summary>
    public class Bind : Element
    {
        // SENT: <iq id="jcl_1" type="set">
        //			<bind xmlns="urn:ietf:params:xml:ns:xmpp-bind"><resource>Exodus</resource></bind>
        //		 </iq>
        // RECV: <iq id='jcl_1' type='result'>
        //			<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><jid>user@server.org/agsxmpp</jid></bind>
        //		 </iq>
        public Bind()
        {
            TagName = "bind";
            Namespace = Uri.BIND;
        }

        public Bind(string resource) : this()
        {
            Resource = resource;
        }

        public Bind(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        ///   The resource to bind
        /// </summary>
        public string Resource
        {
            get { return GetTag("resource"); }
            set { SetTag("resource", value); }
        }

        /// <summary>
        ///   The jid the server created
        /// </summary>
        public Jid Jid
        {
            get { return GetTagJid("jid"); }
            set { SetTag("jid", value.ToString()); }
        }
    }
}