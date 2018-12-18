using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Models;

namespace Vostok.Hercules.Client.Management
{
    internal class StreamDescriptionDto
    {
        public StreamDescriptionDto(StreamDescription description)
        {
            Name = description.Name;
            Type = description.Type.ToString().ToLowerInvariant();
            Partitions = description.Partitions;
            TTL = (int) description.TTL.TotalMilliseconds;
            Sources = description.Sources;
            ShardingKey = description.ShardingKey ?? Array.Empty<string>();
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Partitions { get; set; }

        public int TTL { get; set; }
        
        [CanBeNull]
        public string[] ShardingKey { get; set; }
    
        [CanBeNull]
        public string[] Sources { get; set; }
    }
}