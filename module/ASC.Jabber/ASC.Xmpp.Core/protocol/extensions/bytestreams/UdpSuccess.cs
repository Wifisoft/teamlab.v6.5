// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="UdpSuccess.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.bytestreams
{
    /*
        <message 
            from='proxy.host3' 
            to='target@host2/bar' 
            id='initiate'>
            <udpsuccess xmlns='http://jabber.org/protocol/bytestreams' dstaddr='Value of Hash'/>
        </message>
    */

    public class UdpSuccess : Element
    {
        public UdpSuccess(string dstaddr)
        {
            TagName = "udpsuccess";
            Namespace = Uri.BYTESTREAMS;

            DestinationAddress = dstaddr;
        }

        public string DestinationAddress
        {
            get { return GetAttribute("dstaddr"); }
            set { SetAttribute("dstaddr", value); }
        }
    }
}