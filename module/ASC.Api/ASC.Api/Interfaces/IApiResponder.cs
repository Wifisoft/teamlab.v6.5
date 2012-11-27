using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using ASC.Api.Impl;

namespace ASC.Api.Interfaces
{
    public interface IApiResponder
    {
        string Name { get; }
        IEnumerable<string> GetSupportedExtensions();
        bool CanSerializeType(Type type);
        bool CanRespondTo(IApiStandartResponce responce, HttpContextBase context);
        void RespondTo(IApiStandartResponce responce, HttpContextBase context);
    }
}