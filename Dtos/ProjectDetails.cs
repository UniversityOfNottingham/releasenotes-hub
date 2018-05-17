using System;
using System.Collections.Generic;

namespace releasenotes.Dtos
{
    public class ProjectDetails
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public IList<ReleaseSummary> Releases { get; set; } = new List<ReleaseSummary>();
    }
}