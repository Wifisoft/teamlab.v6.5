namespace ASC.Api.Interfaces
{
    public interface IApiConfiguration
    {
        string ApiPrefix { get; set; }
        string ApiVersion { get; set; }
        char ApiSeparator { get; set; }

        string GetBasePath();
        uint ItemsPerPage { get; }
    }
}