using System;
using System.ComponentModel.DataAnnotations;

namespace releasenotes.Dtos
{
    public class Release
    {
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        
        [Required]
        public string Notes { get; set; }
    }
}