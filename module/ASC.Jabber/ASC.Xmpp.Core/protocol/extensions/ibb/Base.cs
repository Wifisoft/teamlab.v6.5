// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Base.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.ibb
{
    /// <summary>
    ///   IBB base class
    /// </summary>
    public abstract class Base : Element
    {
        public Base()
        {
            Namespace = Uri.IBB;
        }

        /// <summary>
        ///   Sid
        /// </summary>
        public string Sid
        {
            get { return GetAttribute("sid"); }
            set { SetAttribute("sid", value); }
        }
    }
}