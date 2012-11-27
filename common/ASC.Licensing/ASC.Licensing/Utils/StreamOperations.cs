// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="StreamOperations.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace System.IO
{
    public static class StreamOperations
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            return ReadAllBytes(stream, true);
        }

        public static byte[] ReadAllBytes(this Stream stream, bool seekStart)
         {
             using (var memoryStream = new MemoryStream())
             {
                 if (stream.CanSeek && seekStart)
                     stream.Seek(0, SeekOrigin.Begin);

                 int readed;
                 var buffer = new byte[1024];
                 do
                 {
                     readed = stream.Read(buffer, 0, buffer.Length);
                     //Copy buffer to stream
                     memoryStream.Write(buffer, 0, readed);

                 } while (readed > 0);

                 memoryStream.Position = 0;
                 var outbuffer = new byte[memoryStream.Length];
                 memoryStream.Read(outbuffer, 0, outbuffer.Length);
                 return outbuffer;
             }
         }
    }
}