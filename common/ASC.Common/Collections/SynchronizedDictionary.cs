using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ASC.Threading;

namespace ASC.Collections
{
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly Dictionary<TKey, TValue> _innerDictionary;

        public SynchronizedDictionary()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
        }

        public SynchronizedDictionary(int count)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(count);
        }

        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            using (_lock.BeginRead())
            {
                return _innerDictionary.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            using (_lock.BeginWrite())
            {
                _innerDictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public SlimReadLock GetReadLock()
        {
            return _lock.BeginRead();
        }

        public SlimWriteLock GetWriteLock()
        {
            return _lock.BeginWrite();
        }


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count
        {
            get
            {
                using (_lock.BeginRead())
                {
                    return _innerDictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(TKey key)
        {
            using (_lock.BeginRead())
            {
                return _innerDictionary.ContainsKey(key);
            }
        }

        public void Add(TKey key, TValue value)
        {
            using (_lock.BeginWrite())
            {
                _innerDictionary.Add(key, value);
            }
        }

        public bool Remove(TKey key)
        {
            using (_lock.BeginWrite())
            {
                return _innerDictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            using (_lock.BeginRead())
            {
                return _innerDictionary.TryGetValue(key, out value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                using (_lock.BeginWrite())
                {
                    _innerDictionary[key] = value;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                using (_lock.BeginRead())
                {
                    return _innerDictionary.Keys;
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                using (_lock.BeginRead())
                {
                    return _innerDictionary.Values;
                }
            }
        }
    }
}