using System;
using System.IO;
using ASC.Data.Backup;
using log4net;
using log4net.Config;
using NUnit.Framework;

[assembly: XmlConfigurator]

namespace ASC.Data
{
	[TestFixture]
	public class Test
	{
		private static ILog log = LogManager.GetLogger("ASC.Data.Backup");


		[Test]
		public void DatabaseBackupTest()
		{
			var backup = CreateBackupManager();
			backup.RemoveBackupProvider("Files");
			backup.Save(1);
		}

		[Test]
		public void BackupTest()
		{
			var backup = CreateBackupManager(@"..\..\..\..\..\_ci\deploy\webstudio\Web.config");
			backup.Save(0);
		}

		private BackupManager CreateBackupManager(string config)
		{
			var backup = new BackupManager("Backup.zip", config);
			backup.ProgressChanged += ProgressChanged;
			backup.ProgressError += Error;
			return backup;
		}

		private BackupManager CreateBackupManager()
		{
			return CreateBackupManager(null);
		}


		private void Error(object sender, ErrorEventArgs e)
		{
			log.ErrorFormat("\r\nError: {0}", e.GetException());
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.Progress == -1d)
			{
				log.InfoFormat("\r\n\r\n{0}", e.Status);
			}
			else
			{
				log.InfoFormat("{0}% / {1}", e.Progress, e.Status);
			}
		}
	}
}
