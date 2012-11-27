// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Time.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.time
{
    // Send:<iq type='get' id='MX_7' to='jfrankel@coversant.net/SoapBox'>
    //			<query xmlns='jabber:iq:time'></query>
    //		</iq>
    //
    // Recv:<iq from="jfrankel@coversant.net/SoapBox" id="MX_7" to="gnauck@myjabber.net/Office" type="result">
    //			<query xmlns="jabber:iq:time">
    //				<utc>20050125T00:06:15</utc>
    //				<display>Tuesday, January 25, 2005 12:06:15 AM</display>	
    //				<tz>W. Europe Standard Time</tz>
    //			</query>
    //		</iq> 

    /// <summary>
    ///   Zusammenfassung für Time.
    /// </summary>
    public class Time : Element
    {
        public Time()
        {
            TagName = "query";
            Namespace = Uri.IQ_TIME;
        }


        public string Utc
        {
            get { return GetTag("utc"); }
            set { SetTag("utc", value); }
        }

        /// <summary>
        ///   Timezone
        /// </summary>
        //TODO: return System.TimeZone?
        public string Tz
        {
            get { return GetTag("tz"); }
            set { SetTag("tz", value); }
        }

        /// <summary>
        ///   Human-readable date/time.
        /// </summary>
        public string Display
        {
            get { return GetTag("display"); }
            set { SetTag("display", value); }
        }
    }
}