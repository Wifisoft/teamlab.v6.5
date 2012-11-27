// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Default.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    /// <summary>
    ///   The default list
    /// </summary>
    public class Default : List
    {
        public Default()
        {
            TagName = "default";
        }

        public Default(string name) : this()
        {
            Name = name;
        }
    }
}