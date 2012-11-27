// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="StringWriterWithEncoding.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using System.IO;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.IO
{

    #region usings

    #endregion

    /// <summary>
    ///   This class is inherited from the StringWriter Class The standard StringWriter class supports no encoding With this Class we can set the Encoding of a StringWriter in the Constructor
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Encoding m_Encoding;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="encoding"> </param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            m_Encoding = encoding;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public override Encoding Encoding
        {
            get { return m_Encoding; }
        }

        #endregion
    }
}