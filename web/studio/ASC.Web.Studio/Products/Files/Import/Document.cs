using System;

namespace ASC.Web.Files.Import
{
    internal class Document
    {
        public string Id { get; set; }

        public string Parent { get; set; }

        public string Title { get; set; }

        public string ContentLink { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public bool IsFolder { get; set; }
    }
}