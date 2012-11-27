#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ASC.Collections;
using NUnit.Framework;

namespace ASC.Common.Tests.Collection
{
    [TestFixture]
    public class SynchronizedDictionary_Test
    {
        private const int ThreadCountIterations = 100000;

        [Test]
        public void TestIndex()
        {
            var dictionary = new SynchronizedDictionary<string, string>();
            dictionary["1"] = "1";
            Assert.AreEqual(dictionary["1"],"1");
            Assert.AreEqual(dictionary["2"], null);
            string val1;
            dictionary.TryGetValue("3", out val1);
            Assert.AreEqual(val1,null);
            dictionary.TryGetValue("1", out val1);
            Assert.AreEqual(val1, "1");
        }


        [Test]
        public void TestSpeed()
        {
            //Note: speed is not so good fro now
            const double coef = 1.8;
            const int count = 1000000;
            var dict1 = new Dictionary<string, string>(count);
            var dict2 = new SynchronizedDictionary<string, string>(count);
            
            var bench1 = InsertInto(dict1, count);
            var bench2 = InsertInto(dict2, count);
            Assert.LessOrEqual(bench2,bench1*coef);

            bench1 = ReadFrom(dict1, count);
            bench2 = ReadFrom(dict2, count);
            Assert.LessOrEqual(bench2, bench1 * coef);

            bench1 = DeleteFrom(dict1, count);
            bench2 = DeleteFrom(dict2, count);
            

            Assert.LessOrEqual(bench2, bench1 * coef);

            Assert.AreEqual(dict1.Count, 0);
            Assert.AreEqual(dict2.Count, 0);
        }

        private long InsertInto(IDictionary<string, string> dictionary, long count)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (long i = 0; i < count; i++)
            {
                dictionary.Add(i.ToString(),i.ToString());
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long ReadFrom(IDictionary<string, string> dictionary, long count)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (long i = 0; i < count; i++)
            {
                Assert.AreEqual(dictionary[i.ToString()],i.ToString());
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private long DeleteFrom(IDictionary<string, string> dictionary, long count)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (long i = 0; i < count; i++)
            {
                dictionary.Remove(i.ToString());
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private void RemovingThread(object obj)
        {
            var dictionary = (IDictionary<string, string>)obj;
            var rnd = new Random();
            for (int i = 0; i < ThreadCountIterations; i++)
            {
                dictionary.Remove("Key" + rnd.Next(ThreadCountIterations));
            }  
        }


        private void AddingThread(object obj)
        {
            var dictionary = (IDictionary<string, string>)obj;
            var rnd = new Random();
            for (int i = 0; i < ThreadCountIterations; i++)
            {
                dictionary["Key" + rnd.Next(ThreadCountIterations)] = "Value";
            }
        }

        [Test]
        public void TestThreadedSynchronized()
        {
            var dictionary = new SynchronizedDictionary<string, string>();
            RunThreadTest(dictionary);
        }

        //NOTE: it'll crash
        [Test]
        public void TestThreadedWithSimpleDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            RunThreadTest(dictionary);
        }

        private void RunThreadTest(IDictionary<string, string> dictionary)
        {
            Action<object> add = AddingThread;
            var addResults = new IAsyncResult[100];
            for (int i = 0; i < 100; i++)
            {
                addResults[i] = add.BeginInvoke(dictionary, null, null);
            }
            Action<object> remove = RemovingThread;
            var removeResults = new IAsyncResult[100];
            for (int i = 0; i < 100; i++)
            {
                removeResults[i] = remove.BeginInvoke(dictionary, null, null);
            }

            Thread.Sleep(1000);
            for (int i = 0; i < 100; i++)
            {
                add.EndInvoke(addResults[i]);
                remove.EndInvoke(removeResults[i]);
            }

            
        }
    }
}
#endif