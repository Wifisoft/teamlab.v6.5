// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Log.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.Base;

namespace ASC.Xmpp.Core.protocol.component
{
    public enum LogType
    {
        NONE = -1,
        warn,
        info,
        verbose,
        debug,
        notice
    }

    /// <summary>
    ///   Zusammenfassung für Log.
    /// </summary>
    public class Log : Stanza
    {
        public Log()
        {
            TagName = "log";
            Namespace = Uri.ACCEPT;
        }

        /// <summary>
        ///   creates a new Log Packet with the given message
        /// </summary>
        /// <param name="message"> </param>
        public Log(string message) : this()
        {
            Value = message;
        }


        /// <summary>
        ///   Gets or Sets the logtype
        /// </summary>
        public LogType Type
        {
            get { return (LogType) GetAttributeEnum("type", typeof (LogType)); }
            set
            {
                if (value == LogType.NONE)
                    RemoveAttribute("type");
                else
                    SetAttribute("type", value.ToString());
            }
        }

        /// <summary>
        ///   The namespace for logging
        /// </summary>
        public string LogNamespace
        {
            get { return GetAttribute("ns"); }
            set { SetAttribute("ns", value); }
        }
    }
}