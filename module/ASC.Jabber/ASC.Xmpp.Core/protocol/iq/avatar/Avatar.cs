// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Avatar.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.iq.avatar
{
    //	<iq id='2' type='result' to='user@server/resource'>
    //		<query xmlns='jabber:iq:avatar'>
    //			<data mimetype='image/jpeg'>
    //			Base64-Encoded Data
    //			</data>
    //		</query>
    //	</iq>

    /// <summary>
    ///   Summary description for Avatar.
    /// </summary>
    public class Avatar : Base.Avatar
    {
        public Avatar()
        {
            Namespace = Uri.IQ_AVATAR;
        }
    }
}