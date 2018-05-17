using System;

namespace releasenotes.Entities
{
    public class Release
    {
        public string Id { get; set; }

        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        
        public string Notes { get; set; }
    }
}