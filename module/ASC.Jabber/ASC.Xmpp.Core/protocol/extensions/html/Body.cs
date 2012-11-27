// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Body.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.html
{
    /// <summary>
    ///   The Body Element of a XHTML message
    /// </summary>
    public class Body : Element
    {
        public Body()
        {
            TagName = "body";
            Namespace = Uri.XHTML;
        }

        /// <summary>
        /// </summary>
        public string InnerHtml
        {
            get
            {
                // Thats a HACK
                string xml = ToString();

                int start = xml.IndexOf(">");
                int end = xml.LastIndexOf("</" + TagName + ">");

                return xml.Substring(start + 1, end - start - 1);
            }
        }
    }
}