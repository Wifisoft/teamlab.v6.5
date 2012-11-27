#if DEBUG
namespace ASC.TeamLabSvc.Tests
{
    using System;
    using ASC.Common.Module;

    class TestService1 : IServiceController
    {
        public string ServiceName
        {
            get { return GetType().Name; }
        }

        public void Start()
        {
            Console.WriteLine("Start service.");
        }

        public void Stop()
        {
            Console.WriteLine("Stop service.");
        }
    }
}
#endif