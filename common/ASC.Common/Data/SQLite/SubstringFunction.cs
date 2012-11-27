using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace ASC.Common.Data.SQLite
{
    [SQLiteFunction(Name = "substring", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class SubstringFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length != 3 || args[0] == null) return null;
            if (args[0] == DBNull.Value) return DBNull.Value;
            var str = Convert.ToString(args[0]);
            var pos = Convert.ToInt32(args[1]);
            var length = Convert.ToInt32(args[2]);
            if (pos < 0 || pos > str.Length - 1 || length < 1 || pos + length > str.Length)
                return str;
            return str.Substring(pos, length);
        }
    }
}
