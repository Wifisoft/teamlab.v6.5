using System;
using System.IO;

namespace ASC.Data.Storage
{
    public static class TempStream
    {
        public static Stream Create()
        {
            //Return temporary stream
            return new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 2048, FileOptions.DeleteOnClose);
        }
    }
}