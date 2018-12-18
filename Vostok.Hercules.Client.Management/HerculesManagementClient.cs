using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Transport;
using Vostok.Hercules.Client.Abstractions;
using Vostok.Hercules.Client.Abstractions.Queries;
using Vostok.Hercules.Client.Abstractions.Results;
using Vostok.Logging.Abstractions;

namespace Vostok.Hercules.Client.Management
{
    /// <summary>
    /// Represents a settings of <see cref="HerculesManagementClient"/>.
    /// </summary>
    public class HerculesManagementClientConfig
    {
        public IClusterProvider Cluster { get; set; }
        public Func<string> ApiKeyProvider { get; set; }
        public string ServiceName { get; set; }
    }

    /// <inheritdoc />
    [PublicAPI]
    public class HerculesManagementClient : IHerculesManagementClient
    {
        private ClusterClient client;
        private Func<string> getApiKey;

        /// <inheritdoc cref="HerculesManagementClient"/>
        public HerculesManagementClient(HerculesManagementClientConfig config, ILog log)
        {
            client = new ClusterClient(
                log,
                configuration =>
                {
                    configuration.ClusterProvider = config.Cluster;
                    configuration.ServiceName = config.ServiceName;
                    configuration.Transport = new UniversalTransport(log);
                });

            getApiKey = config.ApiKeyProvider;
        }

        /// <inheritdoc />
        public async Task<HerculesResult> CreateStreamAsync(CreateStreamQuery query, TimeSpan timeout)
        {
            var request = Request
                .Post("streams/create")
                .WithHeader("apiKey", getApiKey())
                .WithContent(JsonConvert.SerializeObject(query.Description));
            
            var clusterResult = await client.SendAsync(request, timeout).ConfigureAwait(false);
            
            var herculesStatus = clusterResult.Status != ClusterResultStatus.Success
                ? ConvertFailureToHerculesStatus(clusterResult.Status)
                : ConvertResponseCodeToHerculesStatusForStream(clusterResult.Response.Code);
            
            return new HerculesResult(herculesStatus);
        }

        /// <inheritdoc />
        public async Task<HerculesResult> CreateTimelineAsync(CreateTimelineQuery query, TimeSpan timeout)
        {
            var request = Request
                .Post("timelines/create")
                .WithHeader("apiKey", getApiKey())
                .WithContent(JsonConvert.SerializeObject(query.Description));
            
            var clusterResult = await client.SendAsync(request, timeout).ConfigureAwait(false);
            
            var herculesStatus = clusterResult.Status != ClusterResultStatus.Success
                ? ConvertFailureToHerculesStatus(clusterResult.Status)
                : ConvertResponseCodeToHerculesStatusForTimeline(clusterResult.Response.Code);
            
            return new HerculesResult(herculesStatus);
        }

        /// <inheritdoc />
        public async Task<DeleteStreamResult> DeleteStreamAsync(string name, TimeSpan timeout)
        {
            var request = Request
                .Post("streams/delete")
                .WithAdditionalQueryParameter("stream", name)
                .WithHeader("apiKey", getApiKey());
            
            var clusterResult = await client.SendAsync(request, timeout).ConfigureAwait(false);
            
            var herculesStatus = clusterResult.Status != ClusterResultStatus.Success
                ? ConvertFailureToHerculesStatus(clusterResult.Status)
                : ConvertResponseCodeToHerculesStatusForStream(clusterResult.Response.Code);
            
            return new DeleteStreamResult(herculesStatus);
        }

        /// <inheritdoc />
        public async Task<DeleteTimelineResult> DeleteTimelineAsync(string name, TimeSpan timeout)
        {
            var request = Request
                .Post("timelines/delete")
                .WithAdditionalQueryParameter("timeline", name)
                .WithHeader("apiKey", getApiKey());
            
            var clusterResult = await client.SendAsync(request, timeout).ConfigureAwait(false);
            
            var herculesStatus = clusterResult.Status != ClusterResultStatus.Success
                ? ConvertFailureToHerculesStatus(clusterResult.Status)
                : ConvertResponseCodeToHerculesStatusForTimeline(clusterResult.Response.Code);
            
            return new DeleteTimelineResult(herculesStatus);
        }

        private static HerculesStatus ConvertFailureToHerculesStatus(ClusterResultStatus status)
        {
            switch (status)
            {
                case ClusterResultStatus.TimeExpired:
                    return HerculesStatus.Timeout;
                case ClusterResultStatus.Canceled:
                    return HerculesStatus.Canceled;
                case ClusterResultStatus.Throttled:
                    return HerculesStatus.Throttled;
                default:
                    return HerculesStatus.UnknownError;
            }
        }
        
        private static HerculesStatus ConvertResponseCodeToHerculesStatusForStream(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.NotFound:
                    return HerculesStatus.StreamNotFound;
                case ResponseCode.Conflict:
                    return HerculesStatus.StreamAlreadyExists;
                default:
                    return ConvertResponseCodeToHerculesStatus(code);
            }
        }
        
        private static HerculesStatus ConvertResponseCodeToHerculesStatusForTimeline(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.NotFound:
                    return HerculesStatus.TimelineNotFound;
                case ResponseCode.Conflict:
                    return HerculesStatus.TimelineAlreadyExists;
                default:
                    return ConvertResponseCodeToHerculesStatus(code);
            }
        }
        
        private static HerculesStatus ConvertResponseCodeToHerculesStatus(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.Ok:
                    return HerculesStatus.Success;
                case ResponseCode.RequestTimeout:
                    return HerculesStatus.Timeout;
                case ResponseCode.BadRequest:
                    return HerculesStatus.IncorrectRequest;
                case ResponseCode.Unauthorized:
                    return HerculesStatus.Unauthorized;
                case ResponseCode.Forbidden:
                    return HerculesStatus.InsufficientPermissions;
                default:
                    return HerculesStatus.UnknownError;
            }
        }
    }
}