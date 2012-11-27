// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PrivateLog.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class PrivateLog : Element
    {
        public PrivateLog()
        {
            TagName = "query";
            Namespace = Uri.X_TM_IQ_PRIVATELOG;
        }
    }
}