using System;
using System.IO;
using System.Web;

namespace ASC.Api.Utils
{
    public static class HttpUtils
    {
        private const int BufferReadLength = 2048;

        public static void WriteStreamToResponce(this HttpResponseBase response, Stream stream)
        {
            //set unbuffered output
            response.Buffer = false;
            response.BufferOutput = false;
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var buffer = new byte[BufferReadLength];
            int readed;
            while ((readed = stream.Read(buffer, 0, BufferReadLength)) > 0)
            {
                var subbufer = new byte[readed];
                Array.Copy(buffer, subbufer, readed);
                response.BinaryWrite(subbufer);
            }
        }
    }
}