using System;
using System.IO;

namespace ASC.Data.Backup
{
    public interface IDataWriteOperator : IDisposable
    {
        Stream BeginWriteEntry(string key);
        
		void EndWriteEntry();
    }

	public interface IDataReadOperator : IDisposable
    {
        Stream GetEntry(string key);
    }
}