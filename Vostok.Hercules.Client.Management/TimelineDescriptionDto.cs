using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Queries;

namespace Vostok.Hercules.Client.Management
{
    internal class TimelineDescriptionDto
    {
        //TODO: choose better default.
        private const int DefaultSlicesCount = 3;
        private static readonly TimeSpan DefaultTTL = TimeSpan.FromDays(3);
        private static readonly TimeSpan DefaultTimetrapSize = TimeSpan.FromSeconds(30);
        
        public TimelineDescriptionDto(CreateTimelineQuery query)
        {
            Name = query.Name;
            Sources = query.Sources;
            Slices = query.Slices ?? DefaultSlicesCount;
            TTL = (long) (query.TTL ?? DefaultTTL).TotalMilliseconds;
            TimetrapSize = (long) (query.TimetrapSize ?? DefaultTimetrapSize).TotalMilliseconds;
            ShardingKey = query.ShardingKey;
        }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        [ItemNotNull]
        public string[] Sources { get; set; }

        public int Slices { get; set; }

        public long TTL { get; set; }

        public long TimetrapSize { get; set; }

        [CanBeNull]
        [ItemNotNull]
        public string[] ShardingKey { get; set; }
    }
}