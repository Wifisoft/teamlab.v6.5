using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ASC.Data.Backup
{
    public class BackupManager
    {
        private const string ROOT = "backup";
        private const string XML_NAME = "backupinfo.xml";

        private IDictionary<string, IBackupProvider> providers;
        private readonly string backup;
        private readonly string[] configs;
        private int _stepCount = 0;
        private int _currentStep = 0;


        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        public event EventHandler<ErrorEventArgs> ProgressError;


        public BackupManager(string backup)
            : this(backup, null)
        {
        }

        public BackupManager(string backup, params string[] configs)
        {
            this.backup = backup;
            this.configs = configs ?? new string[0];

            providers = new Dictionary<string, IBackupProvider>();
            AddBackupProvider(new DbBackupProvider());
            AddBackupProvider(new FileBackupProvider());
        }

        public void AddBackupProvider(IBackupProvider provider)
        {
            providers.Add(provider.Name, provider);
            provider.ProgressChanged += OnProgressChanged;
        }

        public void RemoveBackupProvider(string name)
        {
            if (providers.ContainsKey(name))
            {
                providers[name].ProgressChanged -= OnProgressChanged;
                providers.Remove(name);
            }
        }


        public void Save()
        {
            Save(-1);
        }

        public void Save(int tenant)
        {
            SetupProgress(tenant);

            using (var backupWriter = new ZipWriteOperator(backup))
            {
                var doc = new XDocument(new XElement(ROOT, new XAttribute("tenant", tenant)));
                foreach (var provider in providers.Values)
                {
                    try
                    {
                        var elements = provider.GetElements(tenant, configs, backupWriter);
                        if (elements != null)
                        {
                            doc.Root.Add(new XElement(provider.Name, elements));
                        }
                    }
                    catch (Exception ex)
                    {
                        OnProgressError(ex);
                    }
                }

                var data = Encoding.UTF8.GetBytes(doc.ToString(SaveOptions.None));
                var stream = backupWriter.BeginWriteEntry(XML_NAME);
                stream.Write(data, 0, data.Length);
                backupWriter.EndWriteEntry();
            }
        }

        private void SetupProgress(int tenant)
        {
            _stepCount = 0;
            _currentStep = 0;

            foreach (var provider in providers)
            {
                _stepCount += provider.Value.GetStepCount(tenant, configs);
            }
        }

        public void Load()
        {
            using (var reader = new ZipReadOperator(backup))
            using (var xml = XmlReader.Create(reader.GetEntry(XML_NAME)))
            {
                var root = XDocument.Load(xml).Element(ROOT);
                if (root == null) return;

                var tenant = int.Parse(root.Attribute("tenant").Value);
                SetupProgress(tenant);
                foreach (var provider in providers.Values)
                {
                    try
                    {
                        var element = root.Element(provider.Name);
                        provider.LoadFrom(element != null ? element.Elements() : new XElement[0], tenant, configs, reader);
                    }
                    catch (Exception ex)
                    {
                        OnProgressError(ex);
                    }
                }
            }
        }


        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                var currentProgress = (_currentStep * 100 + Math.Max(0, e.Progress)) / (double)_stepCount;
                if (e.Progress == 100 && e.Status == "OK")
                {
                    _currentStep++;
                }
                ProgressChanged(this, new ProgressChangedEventArgs(e.Status, _currentStep == _stepCount ? -1 : currentProgress));
            }
        }

        private void OnProgressError(Exception error)
        {
            if (ProgressError != null) ProgressError(this, new ErrorEventArgs(error));
        }
    }
}