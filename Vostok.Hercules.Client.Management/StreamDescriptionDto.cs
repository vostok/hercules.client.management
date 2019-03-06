using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Models;
using Vostok.Hercules.Client.Abstractions.Queries;

namespace Vostok.Hercules.Client.Management
{
    internal class StreamDescriptionDto
    {
        private const int DefaultPartitionsCount = 3;
        private static readonly TimeSpan DefaultTTL = TimeSpan.FromDays(3);
        
        public StreamDescriptionDto(CreateStreamQuery query)
        {
            Name = query.Name;
            Type = query.Type.ToString().ToLowerInvariant();
            Partitions = query.Partitions ?? DefaultPartitionsCount;
            TTL = (long) (query.TTL ?? DefaultTTL).TotalMilliseconds;
            Sources = query.Sources;
            ShardingKey = query.ShardingKey ?? Array.Empty<string>();
        }

        public string Name { get; }

        public string Type { get; }

        public int Partitions { get; }

        public long TTL { get; }
        
        [CanBeNull]
        public string[] ShardingKey { get; }
    
        [CanBeNull]
        public string[] Sources { get; }
    }
}