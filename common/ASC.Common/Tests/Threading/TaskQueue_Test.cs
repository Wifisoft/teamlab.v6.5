#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Common.Tests.Threading
{
    using NUnit.Framework;
    using ASC.Threading;

    [TestFixture]
    public class TaskQueue_Test
    {
        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void EnqueueTask_Exc()
        {
            TaskQueue q = new TaskQueue();
            q.EnqueueTask(null, null);
        }

        [Test]
        public void Stop()
        {
            TaskQueue q = new TaskQueue(10);
            q.Stop();
        }


    }
}
#endif