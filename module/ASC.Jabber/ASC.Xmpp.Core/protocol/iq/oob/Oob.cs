// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Oob.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.oob
{
    //	<iq type="set" to="horatio@denmark" from="sailor@sea" id="i_oob_001">
    //		<query xmlns="jabber:iq:oob">
    //			<url>http://denmark/act4/letter-1.html</url>
    //			<desc>There's a letter for you sir.</desc>
    //		</query>
    // </iq>	

    /// <summary>
    ///   Zusammenfassung für Oob.
    /// </summary>
    public class Oob : Element
    {
        public Oob()
        {
            TagName = "query";
            Namespace = Uri.IQ_OOB;
        }

        public string Url
        {
            set { SetTag("url", value); }
            get { return GetTag("url"); }
        }

        public string Description
        {
            set { SetTag("desc", value); }
            get { return GetTag("desc"); }
        }
    }
}