using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Storage.Interface
{
    using Services.Muc2.Room.Settings;

    public interface IMucStore
	{
		List<MucRoomInfo> GetMucs(string server);

		MucRoomInfo GetMuc(Jid mucName);

        void SaveMuc(MucRoomInfo muc);

        void RemoveMuc(Jid mucName);

        List<Message> GetMucMessages(Jid mucName, int count);

        void AddMucMessages(Jid mucName, params Message[] message);

        void RemoveMucMessages(Jid mucName);

        MucRoomSettings GetMucRoomSettings(Jid roomName);

        void SetMucRoomSettings(Jid roomName, MucRoomSettings settings);
	}
}
