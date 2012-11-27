using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace ASC.Data.Backup.Restore
{
    public partial class MainForm : Form
    {
        private bool error = false;

        private bool progress = false;


        public MainForm()
        {
            InitializeComponent();

            labelStatus.Text = string.Empty;
            TryFindConfigurations();
        }

        private void buttonCoreConfig_Click(object sender, EventArgs e)
        {
            SetFileName(openFileDialogCoreConfig, textBoxCoreConfig.Text);
            if (openFileDialogCoreConfig.ShowDialog() == DialogResult.OK)
            {
                textBoxCoreConfig.Text = openFileDialogCoreConfig.FileName;
            }
        }

        private void buttonWebConfig_Click(object sender, EventArgs e)
        {
            SetFileName(openFileDialogWebConfig, textBoxWebConfig.Text);
            if (openFileDialogWebConfig.ShowDialog() == DialogResult.OK)
            {
                textBoxWebConfig.Text = openFileDialogWebConfig.FileName;
            }
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {
            SetFileName(openFileDialogBackup, textBoxBackup.Text);
            if (openFileDialogBackup.ShowDialog() == DialogResult.OK)
            {
                textBoxBackup.Text = openFileDialogBackup.FileName;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            buttonRestore.Enabled = 0 < textBoxCoreConfig.TextLength && 0 < textBoxWebConfig.TextLength && 0 < textBoxBackup.TextLength;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (!progress)
            {
                Application.Exit();
            }
        }


        private void TryFindConfigurations()
        {
            try
            {
                var config = ConfigurationManager.AppSettings["coreConfig"];
                config = Path.GetFullPath(!string.IsNullOrEmpty(config) ? config : "TeamLabSvc.exe.Config");
                if (File.Exists(config)) textBoxCoreConfig.Text = config;

                config = ConfigurationManager.AppSettings["webConfig"];
                config = Path.GetFullPath(!string.IsNullOrEmpty(config) ? config : "..\\WebStudio\\Web.Config");
                if (File.Exists(config)) textBoxWebConfig.Text = config;
            }
            catch { }
        }

        private void SetFileName(OpenFileDialog dialog, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (File.Exists(fileName)) dialog.FileName = fileName;
                if (Directory.Exists(fileName)) dialog.InitialDirectory = fileName;
            }
        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            var backuper = new BackupManager(textBoxBackup.Text, textBoxCoreConfig.Text, textBoxWebConfig.Text);
            backuper.AddBackupProvider(new Restarter());
            backuper.ProgressChanged += backuper_ProgressChanged;
            backuper.ProgressError += backuper_ProgressError;

            try
            {
                progress = true;
                error = false;
                labelStatus.Text = string.Empty;
                buttonRestore.Enabled = buttonCancel.Enabled = false;
                
                backuper.Load();
            }
            catch
            {
                progress = false;
                buttonRestore.Enabled = buttonCancel.Enabled = true;
                throw;
            }
        }

        private void backuper_ProgressError(object sender, ErrorEventArgs e)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ErrorEventArgs>(backuper_ProgressError), sender, e);
            }
            else
            {
                error = true;
                progress = false;
                labelStatus.Text = "Error: " + e.GetException().Message.Replace("\r\n", " ");
                progressBar.Value = 0;
                buttonRestore.Enabled = buttonCancel.Enabled = true;
            }
        }

        private void backuper_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ProgressChangedEventArgs>(backuper_ProgressChanged), sender, e);
            }
            else
            {
                if (error) return;
                if (e.Status == "OK" && e.Progress == -1)
                {
                    progress = false;
                    labelStatus.Text = "Complete";
                    progressBar.Value = 0;
                    buttonRestore.Enabled = buttonCancel.Enabled = true;
                }
                else
                {
                    labelStatus.Text = e.Status;
                    progressBar.Value = (int)e.Progress;
                    Application.DoEvents();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = progress;
        }
    }
}
