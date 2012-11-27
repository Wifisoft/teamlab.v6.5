using System.ServiceModel;
using ASC.Common.Module;
using log4net.Config;

namespace ASC.FullTextIndex.Service
{
    class FullTextIndexLauncher : IServiceController
    {
        private TextIndexerService indexer;
        private ServiceHost searcher;


        public string ServiceName
        {
            get { return "Full Text Index"; }
        }


        public void Start()
        {
            XmlConfigurator.Configure();

            indexer = new TextIndexerService();
            indexer.Start();

            searcher = new ServiceHost(typeof(TextSearcherService));
            searcher.Open();
        }

        public void Stop()
        {
            if (searcher != null)
            {
                searcher.Close();
                searcher = null;
            }
            if (indexer != null)
            {
                indexer.Stop();
                indexer = null;
            }
        }
    }
}
