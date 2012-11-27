using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

namespace ASC.Api.Interfaces
{
    public interface IApiSerializer
    {
        IEnumerable<string> GetSupportedExtensions();
        bool CanSerializeType(Type type);
        bool CanRespondTo(IApiStandartResponce responce, string path, string contentType);
        ContentType RespondTo(IApiStandartResponce responce, TextWriter output, string path, string contentType, bool pretty, bool async);
    }
}