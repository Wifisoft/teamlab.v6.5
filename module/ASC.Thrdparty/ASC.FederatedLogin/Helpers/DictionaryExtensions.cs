using System.Collections.Generic;
using System.Linq;

namespace ASC.FederatedLogin.Helpers
{
    public static class DictionaryExtensions
    {
         public static TValue Get<T,TValue>(this IDictionary<T, TValue> disct, T key)
         {
             TValue def;
             disct.TryGetValue(key, out def);
             return def;
         }
    }
}