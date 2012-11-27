// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RosterX.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.rosterx
{

    #region usings

    #endregion

    /// <summary>
    ///   Roster Item Exchange (JEP-0144)
    /// </summary>
    public class RosterX : Element
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="RosterX" /> class.
        /// </summary>
        public RosterX()
        {
            TagName = "x";
            Namespace = Uri.X_ROSTERX;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the roster.
        /// </summary>
        /// <returns> </returns>
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

        /// <summary>
        ///   Adds a roster item.
        /// </summary>
        /// <param name="r"> The r. </param>
        public void AddRosterItem(RosterItem r)
        {
            ChildNodes.Add(r);
        }

        #endregion

        /*
		<message from='horatio@denmark.lit' to='hamlet@denmark.lit'>
		<body>Some visitors, m'lord!</body>
		<x xmlns='http://jabber.org/protocol/rosterx'> 
			<item action='add'
				jid='rosencrantz@denmark.lit'
				name='Rosencrantz'>
				<group>Visitors</group>
			</item>
			<item action='add'
				jid='guildenstern@denmark.lit'
				name='Guildenstern'>
				<group>Visitors</group>
			</item>
		</x>
		</message>
		*/
    }
}