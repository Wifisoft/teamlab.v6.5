// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="GeoLocIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.geoloc
{
    /// <summary>
    ///   a GeoLoc InfoQuery
    /// </summary>
    public class GeoLocIq : IQ
    {
        private readonly GeoLoc m_GeoLoc = new GeoLoc();

        public GeoLocIq()
        {
            base.Query = m_GeoLoc;
            GenerateId();
        }

        public GeoLocIq(IqType type) : this()
        {
            Type = type;
        }

        public GeoLocIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public GeoLocIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new GeoLoc Query
        {
            get { return m_GeoLoc; }
        }
    }
}