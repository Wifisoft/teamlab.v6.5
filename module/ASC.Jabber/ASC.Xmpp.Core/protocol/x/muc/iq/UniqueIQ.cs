// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="UniqueIQ.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.x.muc.iq
{
    public class UniqueIQ : IQ
    {
        public virtual Unique Unique
        {
            get { return SelectSingleElement("unique") as Unique; }

            set
            {
                if (value != null)
                {
                    ReplaceChild(value);
                }
                else
                {
                    RemoveTag("unique");
                }
            }
        }
    }
}