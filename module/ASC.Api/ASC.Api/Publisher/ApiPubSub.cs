using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ASC.Api.Interfaces;
using ASC.Collections;

namespace ASC.Api.Publisher
{
    public class ApiPubSub : IApiPubSub
    {
        private readonly SynchronizedDictionary<string, IList<DataHandler>> _handlers = new SynchronizedDictionary<string, IList<DataHandler>>();

        public void PublishDataForKey(string key, object data)
        {
            if (_handlers.ContainsKey(key))
            {
                IList<DataHandler> handlers = null;
                using (_handlers.GetWriteLock())
                {
                    handlers = new List<DataHandler>(_handlers[key]); //Copy
                    _handlers[key].Clear();
                }
                //Call
                ThreadPool.QueueUserWorkItem(x =>
                                                 {
                                                     foreach (var handler in handlers)
                                                     {
                                                         handler.OnDataAvailible(data);
                                                     }
                                                     handlers.Clear();
                                                 });
            }
        }

        public void SubscribeForKey(string key, DataAvailibleDelegate dataAvailibleDelegate, object userObject)
        {
            if (_handlers.ContainsKey(key))
            {
                _handlers[key].Remove(new DataHandler(userObject, dataAvailibleDelegate));
            }
            else
            {
                _handlers.Add(key, new List<DataHandler>());
            }
            _handlers[key].Add(new DataHandler(userObject, dataAvailibleDelegate));

        }

        public void UnsubscribeForKey(string key, DataAvailibleDelegate dataAvailibleDelegate, object userObject)
        {
            //Dumb method
        }
    }
}