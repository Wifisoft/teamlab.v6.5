using System.Collections.Generic;

namespace System
{
    public static class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 item1)
        {
            return new Tuple<T1>(item1);
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

    
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5) + h1 ^ h2;
        }
    }


    [Serializable]
    public class Tuple<T1>
    {
        public T1 Item1
        {
            get;
            private set;
        }

        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        public override string ToString()
        {
            return string.Format("({0})", Item1);
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<object>.Default;
            return comparer.GetHashCode(Item1);
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1>;
            var comparer = EqualityComparer<object>.Default;
            return t != null && comparer.Equals(Item1, t.Item1);
        }
    }

    [Serializable]
    public class Tuple<T1, T2>
    {
        public T1 Item1
        {
            get;
            private set;
        }

        public T2 Item2
        {
            get;
            private set;
        }

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", Item1, Item2);
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<object>.Default;
            return Tuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2));
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1, T2>;
            var comparer = EqualityComparer<object>.Default;
            return t != null && comparer.Equals(Item1, t.Item1) && comparer.Equals(Item2, t.Item2);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3>
    {
        public T1 Item1
        {
            get;
            private set;
        }

        public T2 Item2
        {
            get;
            private set;
        }

        public T3 Item3
        {
            get;
            private set;
        }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;        
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Item1, Item2, Item3);
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<object>.Default;
            return Tuple.CombineHashCodes(Tuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2)), comparer.GetHashCode(Item3));
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1, T2, T3>;
            var comparer = EqualityComparer<object>.Default;
            return t != null && comparer.Equals(Item1, t.Item1) && comparer.Equals(Item2, t.Item2) && comparer.Equals(Item3, t.Item3);
        }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4>
    {
        public T1 Item1
        {
            get;
            private set;
        }

        public T2 Item2
        {
            get;
            private set;
        }

        public T3 Item3
        {
            get;
            private set;
        }

        public T4 Item4
        {
            get;
            private set;
        }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", Item1, Item2, Item3);
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<object>.Default;
            return Tuple.CombineHashCodes(Tuple.CombineHashCodes(Tuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2)), comparer.GetHashCode(Item3)), comparer.GetHashCode(Item4));
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tuple<T1, T2, T3, T4>;
            var comparer = EqualityComparer<object>.Default;
            return t != null && comparer.Equals(Item1, t.Item1) && comparer.Equals(Item2, t.Item2) && comparer.Equals(Item3, t.Item3) && comparer.Equals(Item4, t.Item4);
        }
    }
}
