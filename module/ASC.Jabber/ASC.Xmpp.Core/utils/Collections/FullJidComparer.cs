// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="FullJidComparer.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region file header

#endregion

#region file header

#endregion

using System;
using System.Collections;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Core.utils.Collections
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class FullJidComparer : IComparer
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <returns> </returns>
        public int Compare(object x, object y)
        {
            if (x is Jid && y is Jid)
            {
                var jidX = (Jid) x;
                var jidY = (Jid) y;

                if (jidX.ToString() == jidY.ToString())
                {
                    return 0;
                }
                else
                {
                    return String.Compare(jidX.ToString(), jidY.ToString());
                }
            }

            throw new ArgumentException("the objects to compare must be Jids");
        }

        #endregion
    }
}