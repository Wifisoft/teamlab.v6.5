// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="BareJidComparer.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

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
    public class BareJidComparer : IComparer
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

                if (jidX.Bare == jidY.Bare)
                {
                    return 0;
                }
                else
                {
                    return String.CompareOrdinal(jidX.Bare, jidY.Bare);
                }
            }

            throw new ArgumentException("the objects to compare must be Jids");
        }

        #endregion
    }
}