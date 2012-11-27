// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Close.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.ibb
{
    /*
         <close xmlns='http://jabber.org/protocol/ibb' sid='mySID'/>      
    */

    /// <summary>
    /// </summary>
    public class Close : Base
    {
        /// <summary>
        /// </summary>
        public Close()
        {
            TagName = "close";
        }

        /// <summary>
        /// </summary>
        /// <param name="sid"> </param>
        public Close(string sid) : this()
        {
            Sid = sid;
        }
    }
}