// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RosterItem.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.x.rosterx
{
    /// <summary>
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// </summary>
        NONE = -1,

        /// <summary>
        /// </summary>
        add,

        /// <summary>
        /// </summary>
        remove,

        /// <summary>
        /// </summary>
        modify
    }

    /// <summary>
    ///   Summary description for RosterItem.
    /// </summary>
    public class RosterItem : Base.RosterItem
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public RosterItem()
        {
            Namespace = Uri.X_ROSTERX;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        public RosterItem(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        public RosterItem(Jid jid, string name) : this(jid)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        /// <param name="action"> </param>
        public RosterItem(Jid jid, string name, Action action) : this(jid, name)
        {
            Action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Action Action
        {
            get { return (Action) GetAttributeEnum("action", typeof (Action)); }

            set { SetAttribute("action", value.ToString()); }
        }

        #endregion

        /*
		<item action='delete' jid='rosencrantz@denmark' name='Rosencrantz'>   
			<group>Visitors</group>   
		</item> 
		*/
    }
}