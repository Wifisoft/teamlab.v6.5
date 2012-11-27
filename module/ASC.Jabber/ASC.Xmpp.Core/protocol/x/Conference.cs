// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Conference.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x
{

    #region usings

    #endregion

    /*
	<message from='crone1@shakespeare.lit/desktop' to='hecate@shakespeare.lit'>
		<body>You have been invited to darkcave@macbeth.</body>
		<x jid='room@service' xmlns='jabber:x:conference'/>
	</message>
	*/

    /// <summary>
    ///   is used for inviting somebody to a chatroom
    /// </summary>
    public class Conference : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Conference()
        {
            TagName = "x";
            Namespace = Uri.X_CONFERENCE;
        }

        /// <summary>
        /// </summary>
        /// <param name="room"> </param>
        public Conference(Jid room) : this()
        {
            Chatroom = room;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Room Jid
        /// </summary>
        public Jid Chatroom
        {
            get { return new Jid(GetAttribute("jid")); }

            set { SetAttribute("jid", value.ToString()); }
        }

        #endregion
    }
}