// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="History.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using System;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class History : Element
    {
        public History()
        {
            TagName = "query";
            Namespace = Uri.X_TM_IQ_HISTORY;
        }

        public DateTime From
        {
            get { return Time.Date(GetAttribute("from")); }
            set { SetAttribute("from", Time.Date(value)); }
        }

        public DateTime To
        {
            get
            {
                DateTime to = Time.Date(GetAttribute("to"));
                return to != DateTime.MinValue ? to : DateTime.MaxValue;
            }
            set { SetAttribute("to", Time.Date(value)); }
        }

        public int Count
        {
            get { return GetAttributeInt("count"); }
            set { SetAttribute("count", value); }
        }
    }
}