// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="MucHelpers.cs">
//   
// </copyright>
// <summary>
//   (c) Copyright Ascensio System Limited 2008-2009
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x.muc;

namespace ASC.Xmpp.Server.Services.Muc2.Helpers
{
    #region usings

    

    #endregion

    /// <summary>
    /// </summary>
    public class MucHelpers
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static Muc GetMuc(Presence presence)
        {
            return (Muc) presence.SelectSingleElement(typeof (Muc));
        }

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static string GetPassword(Presence presence)
        {
            Muc muc = GetMuc(presence);
            return muc != null ? muc.Password : null;
        }

        public static History GetHistroy(Presence presence)
        {
            Muc muc = GetMuc(presence);
            return muc != null ? muc.History : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool IsJoinRequest(Presence presence)
        {
            return presence.Type == PresenceType.available;//Group chat 1.0 and MUC
        }

        #endregion
    }
}