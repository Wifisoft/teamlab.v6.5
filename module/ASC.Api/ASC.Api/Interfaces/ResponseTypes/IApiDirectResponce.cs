using System.Web;

namespace ASC.Api.Interfaces.ResponseTypes
{
    public interface IApiDirectResponce
    {
        void WriteResponce(HttpResponseBase responce);
    }
}