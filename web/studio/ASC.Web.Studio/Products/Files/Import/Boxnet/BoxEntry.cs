using System;
using System.Collections.Generic;

namespace ASC.Web.Files.Import.Boxnet
{
    public class BoxEntry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }
        public bool IsFolder { get; set; }
        public List<BoxEntry> Childs { get; set; }

        public BoxEntry()
        {
            Childs = new List<BoxEntry>();
        }
    }
}