using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using System.Xml.Linq;
using log4net;
using MySql.Data.MySqlClient;


namespace ASC.Data.Backup
{
    public class DbBackupProvider : IBackupProvider
    {
        public string Name
        {
            get { return "databases"; }
        }

        public int GetStepCount(int tenant, string[] configs)
        {
            var connectionKeys = new Dictionary<string, string>();
            foreach (ConnectionStringSettings connectionString in GetConnectionStrings(configs))
            {
                var connectionKey = connectionString.ProviderName + connectionString.ConnectionString;
                if (!connectionKeys.ContainsKey(connectionKey))
                {
                    connectionKeys.Add(connectionKey, connectionString.Name);
                }
            }
            return connectionKeys.Count;
        }

        public IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer)
        {
            var xml = new List<XElement>();
            var connectionKeys = new Dictionary<string, string>();
            foreach (ConnectionStringSettings connectionString in GetConnectionStrings(configs))
            {
                //do not save the base, having the same provider and connection string is not to duplicate
                //data, but also expose the ref attribute of repetitive bases for the correct recovery
                var node = new XElement(connectionString.Name);
                xml.Add(node);

                var connectionKey = connectionString.ProviderName + connectionString.ConnectionString;
                if (connectionKeys.ContainsKey(connectionKey))
                {
                    node.Add(new XAttribute("ref", connectionKeys[connectionKey]));
                }
                else
                {
                    connectionKeys.Add(connectionKey, connectionString.Name);
                    OnProgressChanged("Saving database " + connectionString.Name, -1);
                    node.Add(BackupDatabase(tenant, connectionString, writer));
                    OnProgressChanged("OK", 100);
                }
            }

            return xml;
        }

        public void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader)
        {
            var connectionKeys = new List<string>();
            foreach (var connectionString in GetConnectionStrings(configs))
            {
                var connectionKey = connectionString.ProviderName + connectionString.ConnectionString;
                if (connectionKeys.Contains(connectionKey)) continue;

                connectionKeys.Add(connectionKey);
                OnProgressChanged("Restoring database " + connectionString.Name, -1);
                RestoreDatabase(-1, connectionString, elements, reader);
                OnProgressChanged("OK", 100);
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;


        private void OnProgressChanged(string status, int progress)
        {
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(status, progress));
        }

        private Configuration GetConfiguration(string config)
        {
            if (config.Contains("\\") && !Uri.IsWellFormedUriString(config, UriKind.Relative))
            {
                var map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = string.Compare(Path.GetExtension(config), ".config", true) == 0 ?
                    config :
                    Path.Combine(config, "web.config");
                return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            return WebConfigurationManager.OpenWebConfiguration(config);
        }

        private IEnumerable<ConnectionStringSettings> GetConnectionStrings(string[] configs)
        {
            if (configs.Length == 0)
            {
                configs = new string[] { AppDomain.CurrentDomain.SetupInformation.ConfigurationFile };
            }
            var connectionStrings = new List<ConnectionStringSettings>();
            foreach (var config in configs)
            {
                connectionStrings.AddRange(GetConnectionStrings(GetConfiguration(config)));
            }
            return connectionStrings;
        }

        private IEnumerable<ConnectionStringSettings> GetConnectionStrings(Configuration cfg)
        {
            var connectionStrings = new List<ConnectionStringSettings>();
            foreach (ConnectionStringSettings connectionString in cfg.ConnectionStrings.ConnectionStrings)
            {
                if (connectionString.Name == "LocalSqlServer") continue;
                connectionStrings.Add(connectionString);
                if (connectionString.ConnectionString.Contains("|DataDirectory|"))
                {
                    connectionString.ConnectionString = connectionString.ConnectionString.Replace("|DataDirectory|", Path.GetDirectoryName(cfg.FilePath) + '\\');
                }
            }
            return connectionStrings;
        }

        private List<XElement> BackupDatabase(int tenant, ConnectionStringSettings connectionString, IDataWriteOperator writer)
        {
            var xml = new List<XElement>();
            var errors = 0;
            var timeout = TimeSpan.FromSeconds(1);

            using (var dbHelper = new DbHelper(tenant, connectionString))
            {
                var tables = dbHelper.GetTables();
                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

                    xml.Add(new XElement(table));
                    var stream = writer.BeginWriteEntry(string.Format("{0}\\{1}\\{2}", Name, connectionString.Name, table).ToLower());
                    DataTable dataTable = null;
                    while (true)
                    {
                        try
                        {
                            dataTable = dbHelper.GetTable(table);
                            break;
                        }
                        catch
                        {
                            errors++;
                            if (20 < errors) throw;
                            Thread.Sleep(timeout);
                        }
                    }
                    foreach (DataColumn c in dataTable.Columns)
                    {
                        if (c.DataType == typeof(DateTime)) c.DateTimeMode = DataSetDateTime.Unspecified;
                    }
                    dataTable.WriteXml(stream, XmlWriteMode.WriteSchema);
                    writer.EndWriteEntry();
                }
            }
            return xml;
        }

        private void RestoreDatabase(int tenant, ConnectionStringSettings connectionString, IEnumerable<XElement> elements, IDataReadOperator reader)
        {
            var dbName = connectionString.Name;
            var dbElement = elements.SingleOrDefault(e => string.Compare(e.Name.LocalName, connectionString.Name, true) == 0);
            if (dbElement != null && dbElement.Attribute("ref") != null)
            {
                dbName = dbElement.Attribute("ref").Value;
                dbElement = elements.Single(e => string.Compare(e.Name.LocalName, dbElement.Attribute("ref").Value, true) == 0);
            }
            if (dbElement == null) return;

            using (var dbHelper = new DbHelper(tenant, connectionString))
            {
                var tables = dbHelper.GetTables();
                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    OnProgressChanged(table, (int)(i / (double)tables.Count * 100));

                    if (dbElement.Element(table) == null) continue;

                    var stream = reader.GetEntry(string.Format("{0}\\{1}\\{2}", Name, dbName, table).ToLower());
                    var data = new DataTable();
                    data.ReadXml(stream);

                    dbHelper.SetTable(data);
                }
            }
        }

        private class DbHelper : IDisposable
        {
            private readonly DbProviderFactory factory;
            private readonly DbConnection connect;
            private readonly DbCommandBuilder builder;
            private readonly int tenant;
            private readonly DataTable columns;
            private readonly IDictionary<string, string> whereExceptions = new Dictionary<string, string>();
            private readonly static ILog log = LogManager.GetLogger("ASC.Data");

            public DbHelper(int tenant, ConnectionStringSettings connectionString)
            {
                this.tenant = tenant;
                factory = connectionString.ProviderName == "System.Data.SQLite" ? GetSQLiteFactory() : GetMySqlFactory();
                builder = factory.CreateCommandBuilder();
                connect = factory.CreateConnection();
                connect.ConnectionString = connectionString.ConnectionString;
                connect.Open();

                if (factory.GetType().Name == "MySqlClientFactory")
                {
                    CreateCommand("set @@session.sql_mode = concat(@@session.sql_mode, ',NO_AUTO_VALUE_ON_ZERO')").ExecuteNonQuery();
                }

                columns = connect.GetSchema("Columns");

                whereExceptions["calendar_calendar_item"] = " where calendar_id in (select id from calendar_calendars where tenant = " + tenant + ") ";
                whereExceptions["calendar_calendar_user"] = " where calendar_id in (select id from calendar_calendars where tenant = " + tenant + ") ";
                whereExceptions["calendar_event_item"] = " where event_id in (select id from calendar_events where tenant = " + tenant + ") ";
                whereExceptions["calendar_event_user"] = " where event_id in (select id from calendar_events where tenant = " + tenant + ") ";
                whereExceptions["crm_entity_contact"] = " where contact_id in (select id from crm_contact where tenant_id = " + tenant + ") ";
                whereExceptions["crm_entity_tag"] = " where tag_id in (select id from crm_tag where tenant_id = " + tenant + ") ";
                whereExceptions["files_folder_tree"] = " where folder_id in (select id from files_folder where tenant_id = " + tenant + ") ";
                whereExceptions["forum_answer_variant"] = " where answer_id in (select id from forum_answer where tenantid = " + tenant + ")";
                whereExceptions["forum_topic_tag"] = " where topic_id in (select id from forum_topic where tenantid = " + tenant + ")";
                whereExceptions["forum_variant"] = " where question_id in (select id from forum_question where tenantid = " + tenant + ")";
                whereExceptions["projects_project_participant"] = " where project_id in (select id from projects_projects where tenant_id = " + tenant + ")";
                whereExceptions["projects_following_project_participant"] = " where project_id in (select id from projects_projects where tenant_id = " + tenant + ")";
                whereExceptions["projects_project_tag"] = " where project_id in (select id from projects_projects where tenant_id = " + tenant + ")";
                whereExceptions["projects_project_tag_change_request"] = " where project_id in (select id from projects_projects where tenant_id = " + tenant + ")";
                whereExceptions["tenants_tenants"] = " where id = " + tenant;
                whereExceptions["webstudio_widgetstate"] = " where widgetcontainerid in (select id from webstudio_widgetcontainer where tenantid = " + tenant + ")";
                whereExceptions["core_usersecurity"] = " where userid in (select id from core_user where tenant = " + tenant + ")";
                whereExceptions["core_acl"] = " where tenant = " + tenant + " or tenant = -1";
                whereExceptions["core_subscription"] = " where tenant = " + tenant + " or tenant = -1";
                whereExceptions["core_subscriptionmethod"] = " where tenant = " + tenant + " or tenant = -1";
            }

            public List<string> GetTables()
            {
                return connect
                    .GetSchema("Tables")
                    .Select(@"TABLE_TYPE <> 'SYSTEM_TABLE' and (TABLE_NAME like 'blogs_%' or TABLE_NAME like 'bookmarking_%'
                            or TABLE_NAME like 'calendar_%' or TABLE_NAME like 'core_%' or TABLE_NAME like 'crm_%' or TABLE_NAME like 'events_%'
                            or TABLE_NAME like 'files_%' or TABLE_NAME like 'forum_%' or TABLE_NAME like 'photo_%'
                            or TABLE_NAME like 'projects_%' or TABLE_NAME like 'tenants_%' or TABLE_NAME like 'webstudio_%'
                            or TABLE_NAME like 'wiki_%')")
                    .Select(row => ((string)row["TABLE_NAME"]).ToLowerInvariant())
                    .Where(t => t != "core_settings" && t != "webstudio_uservisit")
                    .ToList();
            }

            public DataTable GetTable(string table)
            {
                try
                {
                    var dataTable = new DataTable(table);
                    var adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = CreateCommand("select * from " + Quote(table) + GetWhere(table));

                    log.Debug(adapter.SelectCommand.CommandText);

                    adapter.Fill(dataTable);
                    return dataTable;
                }
                catch (Exception error)
                {
                    log.ErrorFormat("Table {0}: {1}", table, error);
                    throw;
                }
            }

            public List<string> GetColumns(string table)
            {
                try
                {
                    var dt = GetTable(table);
                    return dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Table {0}: {1}", table, e);
                    throw;
                }
            }


            public void SetTable(DataTable table)
            {
                if (!GetTables().Contains(table.TableName)) return;

                using (var tx = connect.BeginTransaction())
                {
                    try
                    {
                        CreateCommand("delete from " + Quote(table.TableName) + GetWhere(table.TableName)).ExecuteNonQuery();

                        var sql = new StringBuilder("insert into " + Quote(table.TableName) + "(");

                        var columns = GetColumns(table.TableName);
                        var newColumns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                        columns = columns.Intersect(newColumns).ToList();

                        columns.ForEach(column => sql.AppendFormat("{0}, ", Quote(column)));
                        sql.Replace(", ", ") values (", sql.Length - 2, 2);

                        var insert = connect.CreateCommand();
                        columns.ForEach(column =>
                        {
                            sql.AppendFormat("@{0}, ", column);
                            var p = insert.CreateParameter();
                            p.ParameterName = p.SourceColumn = column;
                            insert.Parameters.Add(p);
                        });
                        sql.Replace(", ", ")", sql.Length - 2, 2);
                        insert.CommandText = sql.ToString();

                        foreach (var r in table.Rows.Cast<DataRow>())
                        {
                            foreach (var c in columns)
                            {
                                ((IDbDataParameter)insert.Parameters[c]).Value = r[c];
                            }
                            insert.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Table {0}: {1}", table, e);
                    }
                }
            }

            public void Dispose()
            {
                builder.Dispose();
                connect.Dispose();
            }

            private DbCommand CreateCommand(string sql)
            {
                var command = connect.CreateCommand();
                command.CommandText = sql;
                return command;
            }

            private string Quote(string identifier)
            {
                return builder.QuoteIdentifier(identifier);
            }

            private string GetWhere(string tableName)
            {
                if (tenant == -1) return string.Empty;

                if (whereExceptions.ContainsKey(tableName.ToLower())) return whereExceptions[tableName.ToLower()];
                var tenantColumn = columns.Select(string.Format("TABLE_NAME = '{0}' and COLUMN_NAME like 'tenant%'", tableName));
                return 0 < tenantColumn.Length ?
                    " where " + Quote(tenantColumn[0]["COLUMN_NAME"].ToString()) + " = " + tenant :
                    " where 1 = 0";
            }

            private DbProviderFactory GetSQLiteFactory()
            {
                return new SQLiteFactory();
            }

            private DbProviderFactory GetMySqlFactory()
            {
                return new MySqlClientFactory();
            }
        }
    }
}
