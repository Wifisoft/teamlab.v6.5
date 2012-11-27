using System.Collections.Generic;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Web.Help.DocumentGenerator
{
    public interface IApiDocumentGenerator
    {
        void GenerateDocForEntryPoint(ContainerRegistration apiEntryPointRegistration, IEnumerable<IApiMethodCall> apiMethodCalls);

        void Finish();
    }
}