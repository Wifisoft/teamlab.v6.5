// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Item.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.Base
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Item.
    /// </summary>
    public class Item : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Item()
        {
            TagName = "item";
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                {
                    return new Jid(GetAttribute("jid"));
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    SetAttribute("jid", value.ToString());
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Name
        {
            get { return GetAttribute("name"); }

            set { SetAttribute("name", value); }
        }

        #endregion
    }
}