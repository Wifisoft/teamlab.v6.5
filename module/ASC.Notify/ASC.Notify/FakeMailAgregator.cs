using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Notify.Messages;
using System.Threading;

namespace ASC.Notify
{
    //Всё равно уберу, пусть пока статик
    static class FakeMailAgregator
    {
        private static readonly Random rnd = new Random();

        internal static bool SendMail(NotifyMessage m)
        {
            Thread.Sleep(rnd.Next(1000, 3000));
            return (rnd.Next(100)>10);
        }
    }
}
