using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server.Services.Muc2.Room.Settings
{
    public class MucRoomInfo
    {
        public Jid Jid { get; set; }

        public string Description { get; set; }

        public MucRoomInfo(Jid jid)
        {
            Jid = jid;
        }
    }
}