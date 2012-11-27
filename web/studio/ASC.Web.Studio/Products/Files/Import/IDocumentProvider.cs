using System.Collections.Generic;
using System.IO;

namespace ASC.Web.Files.Import
{
    internal interface IDocumentProvider
    {
        string Name { get; }

        IEnumerable<Document> GetDocuments();

        Stream GetDocumentStream(string contentLink, out long size);
    }
}