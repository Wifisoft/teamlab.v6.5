// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Headers.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.shim
{
    /// <summary>
    ///   JEP-0131: Stanza Headers and Internet Metadata (SHIM)
    /// </summary>
    public class Headers : Element
    {
        // <headers xmlns='http://jabber.org/protocol/shim'>
        //	 <header name='In-Reply-To'>123456789@capulet.com</header>
        // <header name='Keywords'>shakespeare,&lt;xmpp/&gt;</header>
        // </headers>
        public Headers()
        {
            TagName = "headers";
            Namespace = Uri.SHIM;
        }

        /// <summary>
        ///   Adds a new Header
        /// </summary>
        /// <returns> </returns>
        public Header AddHeader()
        {
            var h = new Header();
            AddChild(h);
            return h;
        }

        /// <summary>
        ///   Adds the given Header
        /// </summary>
        /// <param name="header"> </param>
        /// <returns> returns the given Header </returns>
        public Header AddHeader(Header header)
        {
            AddChild(header);
            return header;
        }

        /// <summary>
        ///   Adds a new Header
        /// </summary>
        /// <param name="name"> header name </param>
        /// <param name="val"> header value </param>
        /// <returns> returns the new added header </returns>
        public Header AddHeader(string name, string val)
        {
            var header = new Header(name, val);
            AddChild(header);
            return header;
        }

        public void SetHeader(string name, string val)
        {
            Header header = GetHeader(name);
            if (header != null)
                header.Value = val;
            else
                AddHeader(name, val);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"> </param>
        public Header GetHeader(string name)
        {
            return (Header) SelectSingleElement("header", "name", name);
        }

        public Header[] GetHeaders()
        {
            ElementList nl = SelectElements("header");
            var headers = new Header[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                headers[i] = (Header) e;
                i++;
            }
            return headers;
        }
    }
}