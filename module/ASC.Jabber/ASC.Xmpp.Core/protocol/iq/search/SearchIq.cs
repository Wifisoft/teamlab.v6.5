// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="SearchIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

//	Example 1. Requesting Search Fields
//
//	<iq type='get'
//		from='romeo@montague.net/home'
//		to='characters.shakespeare.lit'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'/>
//	</iq>

namespace ASC.Xmpp.Core.protocol.iq.search
{
    /// <summary>
    ///   Summary description for SearchIq.
    /// </summary>
    public class SearchIq : IQ
    {
        private readonly Search m_Search = new Search();

        public SearchIq()
        {
            base.Query = m_Search;
            GenerateId();
        }

        public SearchIq(IqType type) : this()
        {
            Type = type;
        }

        public SearchIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public SearchIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Search Query
        {
            get { return m_Search; }
        }
    }
}