namespace ASC.Web.Host
{
	public interface IServer
    {
        string Scheme { get; }

        int Port { get; }

        string VirtualPath { get; }

        string PhysicalPath { get; }

        
        void Start();

        void Stop();
	}
}