// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Primary.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.primary
{
    /// <summary>
    ///   http://www.jabber.org/jeps/inbox/primary.html
    /// </summary>
    public class Primary : Element
    {
        /*
		<presence from='juliet@capulet.com/balcony'>
			<status>I&apos;m back!</status>
			<p xmlns='http://jabber.org/protocol/primary'/>
		</presence>
		*/

        public Primary()
        {
            TagName = "p";
            Namespace = Uri.PRIMARY;
        }
    }
}