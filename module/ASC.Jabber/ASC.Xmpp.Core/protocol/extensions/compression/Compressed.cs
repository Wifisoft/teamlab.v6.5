// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Compressed.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.compression
{
    /*
     * Example 5. Receiving Entity Acknowledges Stream Compression
     * <compressed xmlns='http://jabber.org/protocol/compress'/> 
     */

    public class Compressed : Element
    {
        public Compressed()
        {
            TagName = "compressed";
            Namespace = Uri.COMPRESS;
        }
    }
}