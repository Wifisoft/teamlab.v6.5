using System;
using System.Resources;
using System.Globalization;

namespace TMResourceData
{
    public class DBResourceSet : ResourceSet
    {
        internal DBResourceSet(string fileName, CultureInfo culture)
            : base(new DBResourceReader(fileName, culture)) { }


        public override Type GetDefaultReader()
        {
            return typeof(DBResourceReader);
        }

        public void SetString(object name, object newvalue)
        {
            if (Table[name] != null)
                Table[name] = newvalue;
            else
                Table.Add(name, newvalue);
        }

        public int TableCount
        {
            get
            {
                return Table.Count;
            }
        }
    }
}
