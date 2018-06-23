using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APICore
{
    public class ApiClient
    {
        private const string JsonContentType = "application/json";
        private const string PlainContentType = "text/plain";

        private readonly HttpClient _httpClient;

        HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        public ApiClient(bool isCompressed)
        {
            // One instance of HttpClient per application (https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/#problem-description)

            if (isCompressed)
            {
                _httpClient = new HttpClient(new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                })
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };
            }
            else
            {
                _httpClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };
            }
        }

        private Task<HttpResponseMessage> PostWithoutLoadingBodyAsync(string apiEndpointUrl, string content, string contentType, IReadOnlyDictionary<string, string> headers)
        {
            StringContent httpRequest = new StringContent(content, Encoding.UTF8, contentType);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            return _httpClient.PostAsync(apiEndpointUrl, httpRequest);
        }

        public async Task<HttpResponse> PostAsync(string apiEndpointUrl, string content = null, string contentType = PlainContentType, IReadOnlyDictionary<string, string> headers = null)
        {
            var postResult = await PostWithoutLoadingBodyAsync(apiEndpointUrl, content, contentType, headers);

            return new HttpResponse(postResult);
        }

        public async Task<HttpJsonResponse<TResponse>> PostJsonAsync<TResponse>(string apiEndpointUrl, IReadOnlyDictionary<string, string> headers = null)
        {
            var result = await PostWithoutLoadingBodyAsync(apiEndpointUrl, string.Empty, JsonContentType, headers);

            return new HttpJsonResponse<TResponse>(result);
        }

        public async Task<HttpJsonResponse<TResponse>> PostJsonAsync<TResponse, TRequest>(TRequest requestData, string apiEndpointUrl, IReadOnlyDictionary<string, string> headers = null)
        {
            string json = JsonConvert.SerializeObject(requestData);

            var result = await PostWithoutLoadingBodyAsync(apiEndpointUrl, json, JsonContentType, headers);

            return new HttpJsonResponse<TResponse>(result);
        }

        private Task<HttpResponseMessage> GetWithoutLoadingBodyAsync(string apiEndpointUrl, object query, IReadOnlyDictionary<string, string> headers)
        {
            if (query != null)
            {
                apiEndpointUrl += "?" + query.ToQueryString();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, apiEndpointUrl);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return _httpClient.SendAsync(request);
        }

        public async Task<HttpResponse> GetAsync(string apiEndpointUrl, object query = null,
            IReadOnlyDictionary<string, string> headers = null)
        {
            var response = await GetWithoutLoadingBodyAsync(apiEndpointUrl, query, headers);

            return new HttpResponse(response);
        }

        public async Task<HttpJsonResponse<T>> GetJsonAsync<T>(string apiEndpointUrl, object query = null, IReadOnlyDictionary<string, string> headers = null)
        {
            var response = await GetWithoutLoadingBodyAsync(apiEndpointUrl, query, headers);

            return new HttpJsonResponse<T>(response);
        }

        public string BuildUrl(string baseUrl, params string[] pathSegments)
        {
            var escapedPathSegments = pathSegments.Select(Uri.EscapeDataString);

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            return baseUrl + String.Join("/", escapedPathSegments);
        }
    }
}
