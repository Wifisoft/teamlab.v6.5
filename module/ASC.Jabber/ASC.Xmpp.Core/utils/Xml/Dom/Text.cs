// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Text.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.utils.Xml.Dom
{
    /// <summary>
    /// </summary>
    public class Text : Node
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Text()
        {
            NodeType = NodeType.Text;
        }

        /// <summary>
        /// </summary>
        /// <param name="text"> </param>
        public Text(string text) : this()
        {
            Value = text;
        }

        #endregion
    }
}