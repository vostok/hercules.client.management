using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Hercules.Client.Abstractions.Models;
using Vostok.Hercules.Client.Abstractions.Queries;
using Vostok.Hercules.Client.Abstractions.Results;
using Vostok.Logging.Abstractions;

namespace Vostok.Hercules.Client.Management.Tests
{
    internal class HerculesManagementClient_Tests
    {
        [Test, Explicit]
        public void Should_create_and_delete_stream()
        {
            var log = new SilentLog();

            var apiKey = ""; //TODO
            
            var managementClient = new HerculesManagementClient(
                new HerculesManagementClientConfig
                {
                    Cluster = new FixedClusterProvider(new Uri("http://vm-hercules05:6507")),
                    ServiceName = "HerculesManagementApi",
                    ApiKeyProvider = () => apiKey
                },
                log);

            var stream = $"dotnet_test_{Guid.NewGuid().ToString().Substring(0, 8)}";

            managementClient.CreateStreamAsync(new CreateStreamQuery(new StreamDescription(stream)
                {
                    Type = StreamType.Base,
                    Partitions = 1,
                    TTL = 1.Hours()
                }), 5.Seconds())
                .GetAwaiter()
                .GetResult()
                .Status.Should()
                .Be(HerculesStatus.Success);

            managementClient.DeleteStreamAsync(stream, 5.Seconds())
                .GetAwaiter()
                .GetResult()
                .Status.Should()
                .Be(HerculesStatus.Success);
        }
    }
}