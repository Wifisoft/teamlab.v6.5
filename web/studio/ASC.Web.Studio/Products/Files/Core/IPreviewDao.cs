using System;
using System.Collections.Generic;

namespace ASC.Files.Core
{
    public enum PreviewType
    {
        Swf,
        Txt,
        Png,
        Html
    }

    public interface IPreviewDao : IDisposable
    {
        void SetFileCompleted(PreviewFile file, string servicename, string status, long tookTimeMs);
        void SetFileError(PreviewFile file, string servicename, string status);
        void SetFileFatalError(PreviewFile file, string servicename, string status);
        void CollectStats(int intervalInMinutes, DateTime lastCollection);
        int ClearProcessing(string servicename);
        PreviewFile GetNextFileForProcessing(string servicename);
        PreviewFile GetNextFileWithoutPreview();
        List<PreviewFile> GetAllFilesWithoutPreview();
        string GetBaseUrl(string pathPrefix, PreviewType type);
        string GeFlashPreviewUrl(string pathPrefix);
        string GePreviewHtmlUrl(string pathPrefix);
        string GeImagePreviewUrl(string pathPrefix);
        string GeImageThumbUrl(string pathPrefix, int width, int height);
        void AddTextForIndex(long fileid, string text);
        IEnumerable<PreviewStatus> GetStatuses(int tenantId, int[] fileIds, int[] versions);
        void TrackUserActivity(int activityType);
    }
}