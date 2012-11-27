// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Active.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    /// <summary>
    ///   The active list
    /// </summary>
    public class Active : List
    {
        public Active()
        {
            TagName = "active";
        }

        public Active(string name) : this()
        {
            Name = name;
        }
    }
}