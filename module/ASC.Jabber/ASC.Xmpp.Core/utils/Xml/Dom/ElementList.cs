// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="ElementList.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using System;
using System.Collections;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class ElementList : CollectionBase
    {
        #region Constructor

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public void Add(Node e)
        {
            // can't add a empty node, so return immediately
            // Some people tried dthis which caused an error
            if (e == null)
            {
                return;
            }

            List.Add(e);
        }

        // Method implementation from the CollectionBase class
        /// <summary>
        /// </summary>
        /// <param name="index"> </param>
        /// <exception cref="Exception"></exception>
        public void Remove(int index)
        {
            if (index > Count - 1 || index < 0)
            {
                // Handle the error that occurs if the valid page index is       
                // not supplied.    
                // This exception will be written to the calling function             
                throw new Exception("Index out of bounds");
            }

            List.RemoveAt(index);
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public void Remove(Element e)
        {
            List.Remove(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"> </param>
        /// <returns> </returns>
        public Element Item(int index)
        {
            return (Element) List[index];
        }

        #endregion
    }
}