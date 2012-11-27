#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Common.Tests.Collection
{
    using NUnit.Framework;
    using ASC.Collections;

    [TestFixture]
    public class SortedQueue_Test
    {

        #region Enqueue

        [Test]
        public void Enqueue()
        {
            SortedQueue<int> q = new SortedQueue<int>();
            q.Enqueue(1);
            q.Enqueue(3);
            q.Enqueue(2);
            

            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, q._list);

            q = new SortedQueue<int>();
            q.Enqueue(new int[] { 1, 2, 2, 3, 0, -1 });

            CollectionAssert.AreEqual(new int[] { -1,0,1,2,2,3 }, q._list);
        }

        [Test]
        public void Enqueue_NotNull()
        {
            SortedQueue<string> q = new SortedQueue<string>();
            q.Enqueue("");

            var q1 = new SortedQueue<int>();
            q1.Enqueue(0);
        }
        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void Enqueue_Null()
        {
            SortedQueue<string> q = new SortedQueue<string>();
            q.Enqueue((string)null);
        }
        #endregion

        #region Dequeue

        [Test]
        public void Dequeue()
        {
            SortedQueue<int> q = new SortedQueue<int>();
            q._list.AddRange(new int[] { 1, 1, 2 });

            Assert.AreEqual(1, q.Dequeue());
            CollectionAssert.AreEqual(new int[] { 1,2 }, q._list);
        }

        #endregion

        #region Peek

        [Test]
        public void Peek()
        {
            SortedQueue<int> q = new SortedQueue<int>();
            q._list.AddRange(new int[] { 1, 1, 2 });

            Assert.AreEqual(1, q.Peek());
            CollectionAssert.AreEqual(new int[] { 1,1, 2 }, q._list);
        }

        #endregion

        #region foreach
        
        [Test]
        public void ForEach()
        {
            SortedQueue<int> q = new SortedQueue<int>();
            q._list.AddRange(new int[] { 0,1, 1, 2 });

            List<int> result = new List<int>();
            foreach (var item in q)
            {
                result.Add(item);
            }

            CollectionAssert.AreEqual(new int[] { 0, 1, 1, 2 }, result);
        }

        #endregion
    }
}
#endif