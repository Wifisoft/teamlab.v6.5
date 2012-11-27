using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ASC.Common.Data.Mapper;
using ASC.Common.Data.Sql;

namespace ASC.Common.Data
{
    public static class DataExtensions
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText)
        {
            return CreateCommand(connection, commandText, null);
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText, params object[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.AddParameters(parameters);
            return command;
        }

        public static List<object[]> ExecuteList(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteList(sql, parameters);
            }
        }

        public static T ExecuteScalar<T>(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteScalar<T>(sql, parameters);
            }
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteNonQuery(sql, parameters);
            }
        }

        public static IDbCommand SetParameters(this IDbCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return command;
            foreach (var p in parameters)
            {
                AddParameter(command, p.Key, p.Value);
            }
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, object value)
        {
            var p = command.CreateParameter();
            if (!string.IsNullOrEmpty(name))
            {
                p.ParameterName = name.StartsWith("@") ? name : "@" + name;
            }
            if (value == null)
            {
                p.Value = DBNull.Value;
            }
            else if (value is Enum)
            {
                p.Value = ((Enum)value).ToString("d");
            }
            else if (value is DateTime)
            {
                p.Value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Unspecified);
            }
            else
            {
                p.Value = value;
            }
            command.Parameters.Add(p);
            return command;
        }

        public static IDbCommand AddParameters(this IDbCommand command, params object[] parameters)
        {
            if (parameters == null) return command;
            foreach (var value in parameters)
            {
                command.AddParameter(null, value);
            }
            return command;
        }

        public static List<object[]> ExecuteList(this IDbCommand command)
        {
            return ExecuteList(command, command.CommandText, null);
        }

        public static List<object[]> ExecuteList(this IDbCommand command, string sql)
        {
            return ExecuteList(command, sql, null);
        }

        public static List<object[]> ExecuteList(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            return ExecuteListReader(command);
        }

        private static List<object[]> ExecuteListReader(IDbCommand command)
        {
            var result = new List<object[]>();
            using (var reader = command.ExecuteReader())
            {
                var fieldCount = reader.FieldCount;
                while (reader.Read())
                {
                    var row = new object[fieldCount];
                    for (var i = 0; i < fieldCount; i++)
                    {
                        row[i] = reader[i];
                        if (DBNull.Value.Equals(row[i])) row[i] = null;
                    }
                    result.Add(row);
                }
            }
            return result;
        }

        public static T ExecuteScalar<T>(this IDbCommand command)
        {
            return ExecuteScalar<T>(command, command.CommandText, null);
        }

        public static T ExecuteScalar<T>(this IDbCommand command, string sql)
        {
            return ExecuteScalar<T>(command, sql, null);
        }

        public static object ExecuteScalar(this IDbCommand command)
        {
            return ExecuteScalar(command, command.CommandText);
        }

        public static object ExecuteScalar(this IDbCommand command, string sql)
        {
            return ExecuteScalar(command, sql, null);
        }

        public static object ExecuteScalar(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            object result = command.ExecuteScalar();
            return result;
        }

        public static T ExecuteScalar<T>(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            var result = command.ExecuteScalar();
            return result == null || result == DBNull.Value ? default(T) : (T)Convert.ChangeType(result, typeof(T));
        }

        public static int ExecuteNonQuery(this IDbCommand command, string sql)
        {
            return ExecuteNonQuery(command, sql, null);
        }

        public static int ExecuteNonQuery(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            command.Parameters.Clear();
            command.AddParameters(parameters);
            return command.ExecuteNonQuery();
        }

        public static List<object[]> ExecuteList(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteList();
        }

        public static List<T> ExecuteList<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect) where T : new()
        {
            var mapper = DbObjectMapper<T>.Get();
            return ExecuteList<T>(command, sql, dialect, mapper.Map);
        }

        public static List<T> ExecuteList<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect, Converter<IDataRecord, T> rowMapper)
        {
            ApplySqlInstruction(command, sql, dialect);
            var result = new List<T>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) result.Add(rowMapper(reader));
            }
            return result;
        }

        public static object ExecuteScalar(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteScalar();
        }

        public static T ExecuteScalar<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteScalar<T>();
        }

        public static int ExecuteNonQuery(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteNonQuery();
        }

        private static void ApplySqlInstruction(IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            var sqlStr = sql.ToString(dialect);
            var parameters = sql.GetParameters();
            command.Parameters.Clear();

            var sqlParts = sqlStr.Split('?');
            var sqlBuilder = new StringBuilder();
            for (var i = 0; i < sqlParts.Length - 1; i++)
            {
                var name = "p" + i;
                command.AddParameter(name, parameters[i]);
                sqlBuilder.AppendFormat("{0}@{1}", sqlParts[i], name);
            }
            sqlBuilder.Append(sqlParts[sqlParts.Length - 1]);
            command.CommandText = sqlBuilder.ToString();
        }


        #region IDataRecord

        public static bool Exists(this IDataRecord rec, string name)
        {
            for (int i = 0; i < rec.FieldCount; i++)
            {
                if (string.Compare(rec.GetName(i), name, StringComparison.InvariantCultureIgnoreCase) == 0) return true;
            }
            return false;
        }

        public static T Get<T>(this IDataRecord rec, int index)
        {
            var result = default(T);
            var val = rec.GetValue(index);
            if (val != DBNull.Value)
            {
                if (typeof(T) == typeof(Guid))
                {
                    val = rec.GetGuid(index);
                }
                result = (T)Convert.ChangeType(val, typeof(T));
            }
            return result;
        }

        public static T Get<T>(this IDataRecord rec, string name)
        {
            return Exists(rec, name) ? Get<T>(rec, rec.GetOrdinal(name)) : default(T);
        }

        #endregion
    }
}