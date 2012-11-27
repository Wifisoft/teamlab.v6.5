using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.ServiceModel;
using System.Threading;
using ASC.Notify.Config;
using ASC.Notify.Messages;
using log4net;

namespace ASC.Notify
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NotifyService : INotifyService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifyService));

        private readonly DbWorker db = new DbWorker();
        private ManualResetEvent waiter;
        private Thread threadManager;
        private volatile bool work;
        private int threadsCount;


        public void SendNotifyMessage(NotifyMessage notifyMessage)
        {
            try
            {
                db.AddToDbQueue(notifyMessage);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }


        public void StartSending()
        {
            db.CheckStates();

            work = true;
            threadsCount = 0;
            waiter = new ManualResetEvent(true);
            threadManager = new Thread(ThreadManagerWork)
            {
                Priority = ThreadPriority.BelowNormal,
                Name = "ThreadManager",
                IsBackground = true,
            };
            threadManager.Start();
        }

        public void StopSending()
        {
            work = false;
            threadManager.Join();
            waiter.Close();
        }


        private void ThreadManagerWork()
        {
            while (work)
            {
                try
                {
                    waiter.WaitOne(TimeSpan.FromSeconds(5));

                    lock (threadManager)
                    {
                        if (threadsCount < NotifyServiceCfg.MaxThreads)
                        {
                            var messages = db.GetLetters(NotifyServiceCfg.BufferSize);
                            if (0 < messages.Count)
                            {
                                var t = new Thread(ThreadRun)
                                {
                                    Priority = ThreadPriority.Lowest,
                                    IsBackground = true,
                                };
                                t.Start(messages);
                                threadsCount++;
                            }
                        }
                    }
                    waiter.Reset();
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        private void ThreadRun(object state)
        {
            try
            {
                foreach (var m in (IDictionary<int, NotifyMessage>)state)
                {
                    if (!work) return;

                    var result = MailSendingState.Sended;
                    try
                    {
                        NotifyServiceCfg.Senders[m.Value.Sender].Send(m.Value);
                        log.DebugFormat("Notify #{0} has been sent.", m.Key);
                    }
                    catch (Exception e)
                    {
                        result = MailSendingState.FatalError;
                        log.Error(e);
                    }
                    db.SetResult(m.Key, result);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                lock (threadManager)
                {
                    threadsCount--;
                }
                waiter.Set();
            }
        }
    }
}
