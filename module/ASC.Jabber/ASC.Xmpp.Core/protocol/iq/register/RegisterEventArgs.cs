// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RegisterEventArgs.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.iq.register
{
    public delegate void RegisterEventHandler(object sender, RegisterEventArgs args);

    public class RegisterEventArgs
    {
        private bool m_Auto = true;

        public RegisterEventArgs()
        {
        }

        public RegisterEventArgs(Register reg)
        {
            Register = reg;
        }

        // by default we register automatically

        /// <summary>
        ///   Set Auto to true if the library should register automatically Set it to false if you want to fill out the registration fields manual
        /// </summary>
        public bool Auto
        {
            get { return m_Auto; }
            set { m_Auto = value; }
        }

        public Register Register { get; set; }
    }
}