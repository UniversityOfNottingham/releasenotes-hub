using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace releasenotes.Models
{
    public class Project
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public IList<Release> Releases { get; set; } = new List<Release>();
    }
}