using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;


namespace ChildHttpMatrixRepository
{
    public class HttpApiExecutiveRepository
    {
        private ILogger<HttpApiExecutiveRepository> _logger;

        public HttpApiExecutiveRepository(ILogger<HttpApiExecutiveRepository> logger)
        {
            _logger = logger;
        }


        internal async Task VoidGet(string connections, string request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(connections);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(connections + request);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository VoidGet " +
                            $"for request={connections + request} succes status is {response.StatusCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository VoidGet " +
                            $"for request={connections + request} response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository VoidGet " +
                    $"for request={connections + request} url NotFound; {ex.Message}");
            }
        }

        internal async Task<T> GetTDirectResponse<T>(string connections, string request) where T : IResponseDirect, new()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}> " +
                $"called with request={connections + request}");

            T result = new T();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(connections);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(connections + request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<T>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}>" +
                            $" for request={connections + request} succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}>" +
                            $" for request={connections + request} response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}> " +
                            $"for request={connections + request} response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}> " +
                    $"for request={connections + request} url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiExecutiveRepository GetTDirectResponse<{typeof(T).Name}> request url NotFound; {ex.Message}");
                result.Messages.Add(connections + request);
            }

            return result;
        }

        internal async Task<T> GetTNestedResponse<T>(string connections, string request) where T : IResponseNested, new()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> Called " +
                $"with request={connections + request}");

            T result = new T();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(connections);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(connections + request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<T>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> " +
                            $"for request={connections + request} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> " +
                            $"for request={connections + request} response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> " +
                            $"response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> " +
                    $"request {connections + request} url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiExecutiveRepository GetTNestedResponse<{typeof(T).Name}> request url NotFound; {ex.Message}");
                result.Response.Messages.Add(connections + request);
            }

            return result;
        }

        internal async Task<T> DoActionTDirectResponse<T>(EnumHttpActions action, string bodyJson, string connections, string request) where T : IResponseDirect, new()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}> {action} Called " +
                $"with request={connections + request}");

            T result = new T();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(connections);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    switch (action)
                    {
                        case EnumHttpActions.Post:
                            response = await client.PostAsync(connections + request, stringContent);
                            break;
                        case EnumHttpActions.Put:
                            response = await client.PutAsync(connections + request, stringContent);
                            break;
                        case EnumHttpActions.Delete:
                            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                            {
                                Method = HttpMethod.Delete,
                                RequestUri = new Uri(connections + request),
                                Content = stringContent
                            };
                            response = await client.SendAsync(httpRequestMessage);
                            break;
                    }                    

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<T>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}> " +
                            $"{action} for request={connections + request}  success");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}>" +
                            $" {action} for request={connections + request} response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}> {action} for request={connections + request} " +
                            $"response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}> " +
                    $"{action} for request={connections + request} url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiExecutiveRepository DoActionTDirectResponse<{typeof(T).Name}> {action} request url NotFound; {ex.Message}");
                result.Messages.Add(connections + request);
            }

            return result;
        }
    }
}
