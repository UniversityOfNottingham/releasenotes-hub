using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace releasenotes.Dtos
{
    public class Project
    {
        [Required]
        public string Name { get; set; }

        public IList<Release> Releases { get; set; } = new List<Release>();
    }
}