using System;
using System.Collections.Generic;

namespace releasenotes.Models
{
    public class ProjectSummary
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int ReleaseCount { get; set; }

        public Release LatestRelease { get; set; }
    }
}