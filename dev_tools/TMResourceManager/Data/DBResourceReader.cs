using System;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace TMResourceData
{
    public class DBResourceReader : IResourceReader
    {
        readonly string _fileName = "";
        readonly string _language = "";

        public DBResourceReader(string fileName, CultureInfo culture)
        {
            _fileName = fileName;
            _language = culture.Name;

            if (_language == "")
                _language = "Neutral"; 
            
            if (!DbRegistry.IsDatabaseRegistered("tmresource"))
            {
                DbRegistry.RegisterDatabase("tmresource", ConfigurationManager.ConnectionStrings["tmresource"]);
            }
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            var dict = new Hashtable();

            using (var dbManager = new DbManager("tmresource"))
            {
                var sql = new SqlQuery("res_data")
                    .Select("res_data.title", "textValue")
                    .InnerJoin("res_files", Exp.EqColumns("res_files.id", "res_data.fileID"))
                    .LeftOuterJoin("res_cultures", Exp.EqColumns("res_cultures.title", "res_data.cultureTitle"))
                    .Where("ResName", _fileName)
                    .Where("res_cultures.Title", _language);

                var list = dbManager.ExecuteList(sql);

                foreach (var t in list)
                {
                    dict.Add(t[0], t[1]);
                }
            }

            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IDisposable.Dispose()
        {
        }

        void IResourceReader.Close()
        { }
    }
}
