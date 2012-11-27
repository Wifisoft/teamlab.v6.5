#region usings

using System;
using System.Collections.Generic;
using System.Web;

#endregion

namespace ASC.Common.Web
{
    public class DisposableHttpContext : IDisposable
    {
        private const string key = "disposable.key";
        private readonly HttpContext ctx;

        public DisposableHttpContext(HttpContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException();
            this.ctx = ctx;
        }

        public static DisposableHttpContext Current
        {
            get
            {
                if (HttpContext.Current == null) throw new NotSupportedException("Avaliable in web request only.");
                return new DisposableHttpContext(HttpContext.Current);
            }
        }

        public object this[string key]
        {
            get { return Items.ContainsKey(key) ? Items[key] : null; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (!(value is IDisposable)) throw new ArgumentException("Only IDisposable may be added!");
                Items[key] = (IDisposable) value;
            }
        }

        private Dictionary<string, IDisposable> Items
        {
            get
            {
                var table = (Dictionary<string, IDisposable>) ctx.Items[key];
                if (table == null)
                {
                    table = new Dictionary<string, IDisposable>(1);
                    ctx.Items.Add(key, table);
                }
                return table;
            }
        }

        #region IDisposable Members

        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                foreach (IDisposable item in Items.Values)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch
                    {
                    }
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}