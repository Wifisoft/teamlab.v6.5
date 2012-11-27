// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="SaslEventArgs.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.protocol.sasl;

#endregion

namespace ASC.Xmpp.Core.authorization
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="args"> </param>
    public delegate void SaslEventHandler(object sender, SaslEventArgs args);

    /// <summary>
    /// </summary>
    public class SaslEventArgs
    {
        #region Members

        /// <summary>
        /// </summary>
        private bool m_Auto = true;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public SaslEventArgs()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="mechanisms"> </param>
        public SaslEventArgs(Mechanisms mechanisms)
        {
            Mechanisms = mechanisms;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Set Auto to true if the library should choose the mechanism Set it to false for choosing the authentication method yourself
        /// </summary>
        public bool Auto
        {
            get { return m_Auto; }

            set { m_Auto = value; }
        }

        /// <summary>
        ///   SASL Mechanism for authentication as string
        /// </summary>
        public string Mechanism { get; set; }

        /// <summary>
        /// </summary>
        public Mechanisms Mechanisms { get; set; }

        #endregion

        // by default the library chooses the auth method
    }
}