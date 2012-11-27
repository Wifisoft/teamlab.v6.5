using System;
using ASC.Xmpp.Host;

namespace ASC.Xmpp.Server.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var launcher = new XmppServerLauncher();
                launcher.Start();

                Console.WriteLine("Press any key to stop jabber server...");
                Console.ReadKey();

                launcher.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}
