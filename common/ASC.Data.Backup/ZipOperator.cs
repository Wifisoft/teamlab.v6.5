using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace ASC.Data.Backup
{
	public class ZipWriteOperator : IDataWriteOperator
	{
		private readonly ZipOutputStream fz;

		public ZipWriteOperator(string targetFile)
		{
			fz = new ZipOutputStream(new FileStream(targetFile, FileMode.Create));
			fz.SetLevel(9);
			fz.UseZip64 = UseZip64.Off;
		}

		public Stream BeginWriteEntry(string key)
		{
            var entry = new ZipEntry(key) { IsUnicodeText = true };
			fz.PutNextEntry(entry);
			return fz;
		}

		public void EndWriteEntry()
		{
			fz.CloseEntry();
		}

		public void Dispose()
		{
			fz.Finish();
			fz.Close();
		}
	}

	public class ZipReadOperator : IDataReadOperator
	{
		private readonly ZipFile zip;

		public ZipReadOperator(string targetFile)
		{
			zip = new ZipFile(targetFile);
		}
		
		public Stream GetEntry(string key)
		{
			foreach (ZipEntry entry in zip)
			{
				if (entry.Name.Equals(key, StringComparison.OrdinalIgnoreCase)) return zip.GetInputStream(entry);
			}
			return null;
		}

		public void Dispose()
		{
			zip.Close();
		}
	}
}