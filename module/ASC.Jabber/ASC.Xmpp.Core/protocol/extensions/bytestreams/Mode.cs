// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Mode.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.bytestreams
{
    /// <summary>
    ///   The Mode for the bytestream socket layer (tcp or udp)
    /// </summary>
    public enum Mode
    {
        NONE = -1,
        tcp,
        udp
    }
}