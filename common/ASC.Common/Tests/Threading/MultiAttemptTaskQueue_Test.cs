#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Common.Tests.Threading
{
    using NUnit.Framework;
    using ASC.Threading;
    using System.Diagnostics;
    using System.Threading;

    [TestFixture]
    public class MultiAttemptTaskQueue_Test
    {
        [Test]
        public void IsTaskReadyToExecute()
        {
            MultiAttemptTaskQueue q = new MultiAttemptTaskQueue(1,5,TimeSpan.FromSeconds(5));

            TaskQueue.TaskItem item = new TaskQueue.TaskItem(o => true, null);
            item.SystemData = new MultiAttemptTaskQueue.TaskRequest() { LastAttempt = DateTime.Now };
            Assert.IsFalse(q.IsTaskReadyToExecute(item));

            item.SystemData = new MultiAttemptTaskQueue.TaskRequest() { LastAttempt = DateTime.Now - TimeSpan.FromSeconds(6) };
            Assert.IsTrue(q.IsTaskReadyToExecute(item));
        }

        [Test]
        public void NeedWaitTask()
        {
            MultiAttemptTaskQueue q = new MultiAttemptTaskQueue(1, 5, TimeSpan.FromSeconds(5));

            TaskQueue.TaskItem item = new TaskQueue.TaskItem(o => true, null);
            item.SystemData = new MultiAttemptTaskQueue.TaskRequest() { LastAttempt = DateTime.Now };
            Trace.WriteLine(q.NeedWaitTask(item).TotalMilliseconds);
            Assert.Greater(q.NeedWaitTask(item),TimeSpan.Zero);

            item.SystemData = new MultiAttemptTaskQueue.TaskRequest() { LastAttempt = DateTime.Now - TimeSpan.FromSeconds(6) };
            Trace.WriteLine(q.NeedWaitTask(item).TotalMilliseconds); 
            Assert.Less(q.NeedWaitTask(item), TimeSpan.Zero);
        }

        [Test]
        public void Work()
        {
            MultiAttemptTaskQueue q = new MultiAttemptTaskQueue(2, 5, TimeSpan.FromMilliseconds(100));

            int i1 = 0;
            var execDates1 = new List<DateTime>();
            q.EnqueueTask(o => { Thread.Sleep(20); execDates1.Add(DateTime.Now); Trace.WriteLine("1:" + DateTime.Now.ToString() + ":" + i1); i1++; return i1 == 5; }, null);

            int i2 = 0;
            var execDates2 = new List<DateTime>();
            q.EnqueueTask(o => { Thread.Sleep(50); execDates2.Add(DateTime.Now); Trace.WriteLine("2:" + DateTime.Now.ToString() + ":" + i2); i2++; return i2 == 3; }, null);

            Thread.Sleep(600);
            
            Assert.AreEqual(5, execDates1.Count);
            Array.ForEach(
                new[] { 0, 1, 2, 3 }, 
                index => Console.WriteLine(execDates1[index + 1] - execDates1[index]));
            Assert.AreEqual(3, execDates2.Count);
            Array.ForEach(
               new[] { 0, 1 },
               index => Console.WriteLine(execDates2[index + 1] - execDates2[index]));
        }
    }
}
#endif