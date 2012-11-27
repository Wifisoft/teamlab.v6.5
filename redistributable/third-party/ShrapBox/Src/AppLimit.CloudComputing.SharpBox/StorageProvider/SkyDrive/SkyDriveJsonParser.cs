using System;
using System.Collections.Generic;
using System.Linq;
using AppLimit.CloudComputing.SharpBox.Common.Net.Json;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive
{
    public class SkyDriveParserException : Exception
    {
        private readonly String _message;

        public override string Message
        {
            get { return _message; }
        }

        public SkyDriveParserException()
            : this(String.Empty)
        {
        }

        public SkyDriveParserException(string message)
        {
            _message = message;
        }
    }

    internal static class SkyDriveJsonParser
    {
        public static ICloudFileSystemEntry ParseSingleEntry(IStorageProviderSession session, String json)
        {
            return ParseSingleEntry(session, json, null);
        }

        public static ICloudFileSystemEntry ParseSingleEntry(IStorageProviderSession session, String json, JsonHelper parser)
        {
            if (parser == null) 
                parser = CreateParser(json);
            
            ContainsError(json, true, parser);

            BaseFileEntry entry;

            String type = parser.GetProperty("type");
            
            if (!IsFolderType(type) && !IsFileType(type))
                throw new SkyDriveParserException("Can not retrieve entry type while parsing single entry");

            String id = parser.GetProperty("id");
            String name = parser.GetProperty("name");
            String parentID = parser.GetProperty("parent_id");
            String uploadLocation = parser.GetProperty("upload_location");
            DateTime updatedTime = Convert.ToDateTime(parser.GetProperty("updated_time"));

            if (IsFolderType(type))
            {
                int count = parser.GetPropertyInt("count");
                entry = new BaseDirectoryEntry(name, count, updatedTime, session.Service, session) {Id = id};
            }
            else
            {
                int size = parser.GetPropertyInt("size");
                entry = new BaseFileEntry(name, size, updatedTime, session.Service, session) {Id = id};
            }
            entry[SkyDriveConstants.UploadLocationKey] = uploadLocation;
            
            if (!String.IsNullOrEmpty(parentID))
                entry.ParentID = parentID;   
            else
                entry.Name = "/";

            return entry;
        }

        public static String ParseEntryID (String json)
        {
            var parser = CreateParser(json);
            return parser.GetProperty("id");
        }

        public static IEnumerable<ICloudFileSystemEntry> ParseListOfEntries(IStorageProviderSession session, String json)
        {
            return ParseListOfEntries(session, json, null);
        }

        public static IEnumerable<ICloudFileSystemEntry> ParseListOfEntries(IStorageProviderSession session, String json, JsonHelper parser)
        {
            if (parser == null)
                parser = CreateParser(json);
            
            ContainsError(json, true, parser);

            return parser.GetListProperty("data").Select(jsonEntry => ParseSingleEntry(session, jsonEntry));
        }

        public static bool ContainsError(String json, bool throwIfError)
        {
            return ContainsError(json, throwIfError, null);
        }

        public static bool ContainsError(String json, bool throwIfError, JsonHelper parser)
        {
            if (json.Equals(""))
                return false;

            if (parser == null) 
                parser = CreateParser(json);

            var error = parser.GetProperty("error");

            if (!String.IsNullOrEmpty(error))
            {
                if (throwIfError)
                    throw new SkyDriveParserException(
                        String.Format("The returned JSON message is describing the error. The message contained the following: {0}", error));
                
                return true;
            }

            return false;
        }

        private static JsonHelper CreateParser(String json)
        {
            var parser = new JsonHelper();
            if (!parser.ParseJsonMessage(json))
                throw new SkyDriveParserException("Can not parse this JSON message");
            return parser;
        }

        private static bool IsFolderType(String type)
        {
            return type.Equals("folder") || type.Equals("album");
        }

        private static bool IsFileType(String type)
        {
            return type.Equals("file") || type.Equals("photo");
        }
    }
}
