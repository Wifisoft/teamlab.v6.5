// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Roster.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Zusammenfassung für Roster.
    /// </summary>
    public class Roster : Element
    {
        // Request Roster:
        // <iq id='someid' to='myjabber.net' type='get'>
        //		<query xmlns='jabber:iq:roster'/>
        // </iq>
        public Roster()
        {
            TagName = "query";
            Namespace = Uri.IQ_ROSTER;
        }

        public RosterItem[] GetRoster()
        {
            ElementList nl = SelectElements(typeof (RosterItem));
            int i = 0;
            var result = new RosterItem[nl.Count];
            foreach (RosterItem ri in nl)
            {
                result[i] = ri;
                i++;
            }
            return result;
        }

        public void AddRosterItem(RosterItem r)
        {
            ChildNodes.Add(r);
        }
    }
}