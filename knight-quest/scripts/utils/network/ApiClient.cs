using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game.Utils.Network;

#nullable enable
public class ApiClient() : IDisposable
{
    private readonly HttpClient _httpClient = new();

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc
    };

    private bool _disposed;

    public ApiClient(TimeSpan timeout) : this()
    {
        _httpClient.Timeout = timeout;
    }

    public ApiClient(string baseUrl) : this()
    {
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public ApiClient(string baseUrl, TimeSpan timeout) : this()
    {
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = timeout;
    }

    // GET request
    public async Task<HttpResponse<T>> GetAsync<T>(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(HttpMethod.Get, url, null, headers);
    }

    public async Task<HttpResponse<string>> GetAsync(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(HttpMethod.Get, url, null, headers);
    }

    // POST request
    public async Task<HttpResponse<T>> PostAsync<T>(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(HttpMethod.Post, url, data, headers);
    }

    public async Task<HttpResponse<string>> PostAsync(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(HttpMethod.Post, url, data, headers);
    }

    // PUT request
    public async Task<HttpResponse<T>> PutAsync<T>(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(HttpMethod.Put, url, data, headers);
    }

    public async Task<HttpResponse<string>> PutAsync(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(HttpMethod.Put, url, data, headers);
    }

    // PATCH request
    public async Task<HttpResponse<T>> PatchAsync<T>(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(new HttpMethod("PATCH"), url, data, headers);
    }

    public async Task<HttpResponse<string>> PatchAsync(string url, object? data = null,
        Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(new HttpMethod("PATCH"), url, data, headers);
    }

    // DELETE request
    public async Task<HttpResponse<T>> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(HttpMethod.Delete, url, null, headers);
    }

    public async Task<HttpResponse<string>> DeleteAsync(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(HttpMethod.Delete, url, null, headers);
    }

    // HEAD request
    public async Task<HttpResponse<object>> HeadAsync(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<object>(HttpMethod.Head, url, null, headers);
    }

    // OPTIONS request
    public async Task<HttpResponse<T>> OptionsAsync<T>(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<T>(HttpMethod.Options, url, null, headers);
    }

    public async Task<HttpResponse<string>> OptionsAsync(string url, Dictionary<string, string>? headers = null)
    {
        return await SendRequestAsync<string>(HttpMethod.Options, url, null, headers);
    }

    // Generic request method
    public async Task<HttpResponse<T>> SendRequestAsync<T>(
        HttpMethod method,
        string url,
        object? data = null,
        Dictionary<string, string>? headers = null)
    {
        try
        {
            var request = new HttpRequestMessage(method, url);

            // Add headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            // Add content for requests that support it
            if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put ||
                                 method == new HttpMethod("PATCH")))
            {
                string jsonContent;

                if (data is string stringData)
                {
                    jsonContent = stringData;
                }
                else
                {
                    jsonContent = JsonConvert.SerializeObject(data, _jsonSettings);
                }

                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            Logger.D($"Sending {method} request to: {url}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            Logger.D($"Response status: {response.StatusCode}, Content length: {responseContent.Length}");

            // Parse JSON response if content type is JSON and T is not string
            var parsedData = default(T);
            var isJsonResponse = response.Content.Headers.ContentType?.MediaType?.Contains("application/json") == true;

            if (isJsonResponse && typeof(T) != typeof(string) && !string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    if (typeof(T) == typeof(JObject))
                    {
                        parsedData = (T)(object)JObject.Parse(responseContent);
                    }
                    else if (typeof(T) == typeof(JArray))
                    {
                        parsedData = (T)(object)JArray.Parse(responseContent);
                    }
                    else
                    {
                        parsedData = JsonConvert.DeserializeObject<T>(responseContent, _jsonSettings);
                    }
                }
                catch (JsonException ex)
                {
                    Logger.E($"Failed to parse JSON response: {ex.Message}");
                    return new HttpResponse<T>
                    {
                        IsSuccess = false,
                        StatusCode = response.StatusCode,
                        Content = responseContent,
                        Error = $"JSON parsing error: {ex.Message}",
                        Data = default(T)
                    };
                }
            }
            else if (typeof(T) == typeof(string))
            {
                parsedData = (T)(object)responseContent;
            }

            return new HttpResponse<T>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Content = responseContent,
                Data = parsedData,
                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
            };
        }
        catch (HttpRequestException ex)
        {
            Logger.E($"HTTP request failed: {ex.Message}");
            return new HttpResponse<T>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = string.Empty,
                Error = $"HTTP request failed: {ex.Message}",
                Data = default(T)
            };
        }
        catch (TaskCanceledException ex)
        {
            Logger.E($"Request timeout: {ex.Message}");
            return new HttpResponse<T>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                Content = string.Empty,
                Error = $"Request timeout: {ex.Message}",
                Data = default(T)
            };
        }
        catch (Exception ex)
        {
            Logger.E($"Unexpected error: {ex.Message}");
            return new HttpResponse<T>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = string.Empty,
                Error = $"Unexpected error: {ex.Message}",
                Data = default(T)
            };
        }
    }

    // Convenience methods for common use cases
    public async Task<T?> GetJsonAsync<T>(string url, Dictionary<string, string>? headers = null)
    {
        var response = await GetAsync<T>(url, headers);
        return response.IsSuccess ? response.Data : default(T);
    }

    public async Task<T?> PostJsonAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        var response = await PostAsync<T>(url, data, headers);
        return response.IsSuccess ? response.Data : default(T);
    }

    public async Task<T?> PutJsonAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        var response = await PutAsync<T>(url, data, headers);
        return response.IsSuccess ? response.Data : default(T);
    }

    public async Task<T?> PatchJsonAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        var response = await PatchAsync<T>(url, data, headers);
        return response.IsSuccess ? response.Data : default(T);
    }

    // File upload methods
    public async Task<HttpResponse<T>> UploadFileAsync<T>(
        string url,
        string filePath,
        string fieldName = "file",
        Dictionary<string, string>? additionalFields = null,
        Dictionary<string, string>? headers = null)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            // Add file
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, fieldName, Path.GetFileName(filePath));

            // Add additional fields
            if (additionalFields != null)
            {
                foreach (var field in additionalFields)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            // Add headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            Logger.D($"Uploading file to: {url}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            Logger.D($"Upload response status: {response.StatusCode}");

            // Parse JSON response if applicable
            T? parsedData = default(T);
            bool isJsonResponse = response.Content.Headers.ContentType?.MediaType?.Contains("application/json") == true;

            if (isJsonResponse && typeof(T) != typeof(string) && !string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    parsedData = JsonConvert.DeserializeObject<T>(responseContent, _jsonSettings);
                }
                catch (JsonException ex)
                {
                    Logger.E($"Failed to parse JSON response: {ex.Message}");
                    return new HttpResponse<T>
                    {
                        IsSuccess = false,
                        StatusCode = response.StatusCode,
                        Content = responseContent,
                        Error = $"JSON parsing error: {ex.Message}",
                        Data = default(T)
                    };
                }
            }
            else if (typeof(T) == typeof(string))
            {
                parsedData = (T)(object)responseContent;
            }

            return new HttpResponse<T>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Content = responseContent,
                Data = parsedData,
                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
            };
        }
        catch (Exception ex)
        {
            Logger.E($"File upload failed: {ex.Message}");
            return new HttpResponse<T>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = string.Empty,
                Error = $"File upload failed: {ex.Message}",
                Data = default
            };
        }
    }

    // Download file method
    public async Task<HttpResponse<byte[]>> DownloadFileAsync(string url, Dictionary<string, string>? headers = null)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            Logger.D($"Downloading file from: {url}");

            var response = await _httpClient.SendAsync(request);
            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            Logger.D($"Download response status: {response.StatusCode}, File size: {fileBytes.Length} bytes");

            return new HttpResponse<byte[]>
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Content = string.Empty,
                Data = fileBytes,
                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
            };
        }
        catch (Exception ex)
        {
            Logger.E($"File download failed: {ex.Message}");
            return new HttpResponse<byte[]>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = string.Empty,
                Error = $"File download failed: {ex.Message}",
                Data = Array.Empty<byte>()
            };
        }
    }

    // Set default headers
    public void SetDefaultHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    public void SetDefaultHeader(string name, IEnumerable<string> values)
    {
        _httpClient.DefaultRequestHeaders.Add(name, values);
    }

    // Remove default header
    public bool RemoveDefaultHeader(string name)
    {
        return _httpClient.DefaultRequestHeaders.Remove(name);
    }

    // Clear all default headers
    public void ClearDefaultHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
    }

    // IDisposable implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _httpClient?.Dispose();
        }

        _disposed = true;
    }
}
