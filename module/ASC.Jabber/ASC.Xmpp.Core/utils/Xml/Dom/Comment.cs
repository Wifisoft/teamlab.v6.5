// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Comment.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.utils.Xml.Dom
{
    /// <summary>
    ///   Summary description for Comment.
    /// </summary>
    public class Comment : Node
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Comment()
        {
            NodeType = NodeType.Comment;
        }

        /// <summary>
        /// </summary>
        /// <param name="text"> </param>
        public Comment(string text) : this()
        {
            Value = text;
        }

        #endregion
    }
}