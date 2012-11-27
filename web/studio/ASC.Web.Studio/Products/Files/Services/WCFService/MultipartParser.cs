using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ASC.Web.Files.Services.WCFService
{
    public class MultipartParser
    {
        public MultipartParser(Stream stream)
        {
            this.Parse(stream, Encoding.UTF8);
        }

        public MultipartParser(Stream stream, Encoding encoding)
        {
            this.Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            this.Success = false;

            // Read the stream into a byte array
            var data = ToByteArray(stream);

            // Copy to a string for header parsing
            var content = encoding.GetString(data);

            // The first line should contain the delimiter
            var delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                var delimiter = content.Substring(0, content.IndexOf("\r\n"));

                // Look for Content-Type
                var re = new Regex(@"(?<=Content-Type:)(.*?)(?=\r\n\r\n)");
                var contentTypeMatch = re.Match(content);

                if (!contentTypeMatch.Success)
                {
                    re = new Regex(@"(?<=Content-Disposition:)(.*?)(?=\r\n\r\n)");
                    contentTypeMatch = re.Match(content);
                }

                // Look for filename
                re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                var filenameMatch = re.Match(content);

                // Did we find the required values?
                if (contentTypeMatch.Success && filenameMatch.Success)
                {
                    // Set properties
                    this.ContentType = contentTypeMatch.Value.Trim();
                    this.Filename = filenameMatch.Value.Trim();

                    // Get the start & end indexes of the file contents
                    var startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                    var delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                    var endIndex = IndexOf(data, delimiterBytes, startIndex);

                    var contentLength = endIndex - startIndex;

                    // Extract the file contents from the byte array
                    var fileData = new byte[contentLength];

                    Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                    this.FileContents = fileData;
                    this.Success = true;
                }
            }
        }

        private int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            var index = 0;
            var startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        private byte[] ToByteArray(Stream stream)
        {
            return stream.GetStreamBuffer();
        }

        public bool Success { get; private set; }

        public string ContentType { get; private set; }

        public string Filename { get; private set; }

        public byte[] FileContents { get; private set; }
    }

}