using System;
using System.Collections.Generic;
using System.Globalization;

namespace ASC.Web.Controls.FileUploader.HttpModule
{
    public class UploadProgressStatistic
    {
        private static Dictionary<string, UploadProgressStatistic> statistics = new Dictionary<string, UploadProgressStatistic>();
        public const string UploadIdField = "__UixdId";

        public string UploadId { get; set; }
        public long TotalBytes { get; set; }
        public long UploadedBytes { get; set; }
        public bool IsFinished { get; set; }
        public string CurrentFile { get; set; }
        public int CurrentFileIndex { get; set; }
        public float Progress { get; set; }
        public int ReturnCode { get; set; }


        internal UploadProgressStatistic()
        {
            CurrentFile = string.Empty;
            CurrentFileIndex = -1;
        }

        public string ToJson()
        {
            return string.Format("{{\"TotalBytes\":{0},\"Progress\":{1},\"CurrentFileIndex\":{2},\"CurrentFile\":\"{3}\",\"UploadId\":{4},\"IsFinished\":{5},\"Swf\":false,\"ReturnCode\":{6}}}",
                TotalBytes, Progress.ToString().Replace(',', '.'), CurrentFileIndex, CurrentFile, UploadId ?? "null", (IsFinished ? "true" : "false"), ReturnCode);
        }

        internal void AddUploadedBytes(int bytesCount)
        {
            UploadedBytes += bytesCount;
            Progress = (float)UploadedBytes / TotalBytes;
            Progress = Progress > 1 ? 1 : Progress;
        }

        public static UploadProgressStatistic GetStatistic(string id)
        {
            UploadProgressStatistic us;
            if (!statistics.TryGetValue(id, out us))
                us = new UploadProgressStatistic();
            return us;
        }

        internal void EndUpload()
        {
            IsFinished = true;
        }

        internal void BeginFileUpload(string fileName)
        {
            CurrentFile = fileName;
            CurrentFileIndex++;
        }

        internal void AddFormField(string name, string value)
        {
            if (name == UploadIdField)
            {
                UploadId = value;

                if (!statistics.ContainsKey(value))
                    statistics.Add(value, this);
            }
        }

        private string DateTimeSerializeToJSON(DateTime dateTime)
        {
            var value = new DateTimeOffset(dateTime);
            var kind = dateTime.Kind;
            var num = ((value.ToUniversalTime().Ticks - new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks) / 0x2710L);
            var str = string.Empty;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Local:
                    TimeSpan offset = value.Offset;
                    str = offset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture) + offset.Minutes.ToString("00;00", CultureInfo.InvariantCulture);
                    break;
            }
            return ("\"\\/Date(" + num.ToString(CultureInfo.InvariantCulture) + str + ")\\/\"");
        }
    }
}