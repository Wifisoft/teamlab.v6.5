using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.ping;
using ASC.Xmpp.Core.protocol.iq.time;
using ASC.Xmpp.Core.protocol.iq.version;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Version))]
    [XmppHandler(typeof(EntityTime))]
    [XmppHandler(typeof(Ping))]
    class VerTimePingHandler : XmppStanzaHandler
    {
        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            var answer = new IQ(IqType.result)
            {
                Id = iq.Id,
                To = iq.From,
                From = iq.To,
            };

            //iq sended to server
            if (iq.Type == IqType.get && (!iq.HasTo || iq.To.IsServer || iq.To == iq.From))
            {
                if (iq.Query is Version)
                {
                    answer.Query = new Version()
                    {
                        Name = "TeamLab Jabber Server",
                        Os = System.Environment.OSVersion.ToString(),
                        Ver = "1.0",
                    };
                    return answer;
                }
                else if (iq.Query is Ping)
                {
                    return answer;
                }
                return XmppStanzaError.ToServiceUnavailable(iq);
            }

            if (iq.Type == IqType.get && iq.HasTo)
            {
                //resend iq
                var sessionTo = context.SessionManager.GetSession(iq.To);
                var sessionFrom = context.SessionManager.GetSession(iq.From);
                if (sessionTo != null && sessionFrom != null)
                {
                    IdleWatcher.StartWatch(
                        iq.Id,
                        System.TimeSpan.FromSeconds(3),
                        (s, e) => { context.Sender.SendTo(sessionFrom, XmppStanzaError.ToServiceUnavailable(iq)); });
                    context.Sender.SendTo(sessionTo, iq);
                }
                else
                {
                    return XmppStanzaError.ToRecipientUnavailable(iq);
                }
            }
            if (iq.Type == IqType.error || iq.Type == IqType.result)
            {
                IdleWatcher.StopWatch(iq.Id);
                if (!iq.HasTo)
                {
                    return XmppStanzaError.ToBadRequest(iq);
                }
                var session = context.SessionManager.GetSession(iq.To);
                if (session != null)
                {
                    context.Sender.SendTo(session, iq);
                }
            }
            return null;
        }
    }
}