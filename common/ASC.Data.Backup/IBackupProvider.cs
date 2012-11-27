using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ASC.Data.Backup
{
	public interface IBackupProvider
	{
		string Name
		{
			get;
		}

	    int GetStepCount(int tenant, string[] configs);

		IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer);

		void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader);

		event EventHandler<ProgressChangedEventArgs> ProgressChanged;

	}

	public class ProgressChangedEventArgs : EventArgs
	{
		public string Status
		{
			get;
			private set;
		}

		public double Progress
		{
			get;
			private set;
		}
        
		public ProgressChangedEventArgs(string status, double progress)
		{
			Status = status;
			Progress = progress;
		}
	}
}