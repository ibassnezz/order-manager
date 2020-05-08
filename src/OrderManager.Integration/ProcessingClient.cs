using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace OrderManager.Integration
{
    public class ProcessingClient: IProcessingClient
    {
        private const string Path = "/api/process/";
        private readonly string _baseUrl;

        private readonly HttpClient _client;
        
        public ProcessingClient(HttpClient client, IOptions<ProcessingConfigurations> options)
        {
            _baseUrl = options.Value.Url;
            _client = client;
        }

        public async Task<ApiResponse> Execute(OrderProcessingDto cancellationDto, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{_baseUrl}{Path}");

            var content = StringContent(cancellationDto);

            var result = await _client.PostAsync(uri, content, cancellationToken);

            return new ApiResponse(
                result.IsSuccessStatusCode,
                result.Content is null
                    ? null
                    : await result.Content.ReadAsStringAsync());

        }

        private static StringContent StringContent(OrderProcessingDto cancellationDto)
        {
            var serialized = JsonConvert.SerializeObject(cancellationDto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var content = new StringContent(serialized, System.Text.Encoding.UTF8, "application/json");
            return content;
        }
    }
}
