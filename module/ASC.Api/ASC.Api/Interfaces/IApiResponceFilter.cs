using ASC.Api.Impl;

namespace ASC.Api.Interfaces
{
    public interface IApiResponceFilter
    {
        object FilterResponce(object responce, ApiContext context);
    }
}