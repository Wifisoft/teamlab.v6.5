using System;
using System.Configuration;
using ASC.Common.Data;

namespace ASC.FullTextIndex.Service
{
	class ModuleInfo
	{
		public string Name
		{
			get;
			private set;
		}

		public string Select
		{
			get;
			private set;
		}

		public ConnectionStringSettings ConnectionString
		{
			get;
			private set;
		}

		public ModuleInfo(string name, string select, ConnectionStringSettings connectionString)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			if (string.IsNullOrEmpty(select)) throw new ArgumentNullException("select");
			if (connectionString == null) throw new ArgumentNullException("connectionString");

			Name = name;
			Select = select.Trim();
			ConnectionString = connectionString;
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var mi = obj as ModuleInfo;
			return mi != null && Name == mi.Name;
		}
	}
}
