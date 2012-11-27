using System;
using System.Data.SQLite;

namespace ASC.Common.Data.SQLite
{
    [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class UpperFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length == 0 || args[0] == null) return null;
            if (args[0] == DBNull.Value) return DBNull.Value;
            return ((string) args[0]).ToUpper();
        }
    }
}