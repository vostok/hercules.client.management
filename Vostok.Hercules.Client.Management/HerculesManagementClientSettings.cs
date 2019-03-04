using System;
using Vostok.Clusterclient.Core.Topology;

namespace Vostok.Hercules.Client.Management
{
    /// <summary>
    /// Represents a settings of <see cref="HerculesManagementClient"/>.
    /// </summary>
    public class HerculesManagementClientSettings
    {
        public IClusterProvider Cluster { get; set; }
        public Func<string> ApiKeyProvider { get; set; }
    }
}