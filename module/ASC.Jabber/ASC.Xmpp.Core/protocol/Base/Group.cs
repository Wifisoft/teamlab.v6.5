// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Group.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.Base
{
    /// <summary>
    ///   Summary description for Group.
    /// </summary>
    public class Group : Element
    {
        public Group()
        {
            TagName = "group";
        }

        public Group(string groupname) : this()
        {
            Name = groupname;
        }

        /// <summary>
        ///   gets or sets the Name of the contact group
        /// </summary>
        public string Name
        {
            set { Value = value; }
            get { return Value; }
        }
    }
}