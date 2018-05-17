using System;

namespace releasenotes.Models
{
    public class ReleaseSummary
    {
        public string Id { get; set; }

        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

        public static string GenerateHref(string projectId, string releaseId)
            => $"{BasePaths.ApiRoot}{BasePaths.Projects}/{projectId}/{releaseId}";

        public string href { get; set; }
    }
}