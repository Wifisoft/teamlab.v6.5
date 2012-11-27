// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Last.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using ASC.Xmpp.Core.utils.Xml.Dom;

// Send:	<iq type='get' id='MX_5' to='jfrankel@coversant.net/SoapBox'>
//				<query xmlns='jabber:iq:last'></query>
//			</iq>
// Recv:	<iq from="jfrankel@coversant.net/SoapBox" id="MX_5" to="gnauck@myjabber.net/Office" type="result">
//				<query seconds="644" xmlns="jabber:iq:last"/>
//			</iq> 

namespace ASC.Xmpp.Core.protocol.iq.last
{
    /// <summary>
    ///   Zusammenfassung für Last.
    /// </summary>
    public class Last : Element
    {
        public Last()
        {
            TagName = "query";
            Namespace = Uri.IQ_LAST;
        }

        /// <summary>
        ///   Seconds since the last activity.
        /// </summary>
        public int Seconds
        {
            get { return Int32.Parse(GetAttribute("seconds")); }
            set { SetAttribute("seconds", value.ToString()); }
        }
    }
}