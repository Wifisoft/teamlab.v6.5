// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Html.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.html
{
    /*
     * <message>
     *      <body>hi!</body>
     *      <html xmlns='http://jabber.org/protocol/xhtml-im'>
     *          <body xmlns='http://www.w3.org/1999/xhtml'>
     *              <p style='font-weight:bold'>hi!</p>
     *          </body>
     *      </html>
     * </message>
     */

    public class Html : Element
    {
        public Html()
        {
            TagName = "html";
            Namespace = Uri.XHTML_IM;
        }

        /// <summary>
        ///   The Body Element of the XHTML Message
        /// </summary>
        public Body Body
        {
            get { return SelectSingleElement(typeof (Body)) as Body; }
            set
            {
                if (HasTag(typeof (Body)))
                    RemoveTag(typeof (Body));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}