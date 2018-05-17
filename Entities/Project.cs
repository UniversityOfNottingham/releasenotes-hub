using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace releasenotes.Entities
{
    public class Project
    {
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        public IList<Release> Releases { get; set; } = new List<Release>();
    }
}