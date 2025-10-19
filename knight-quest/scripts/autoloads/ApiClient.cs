using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;
using Game.Data;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Game.Autoloads;

public partial class ApiClient : Autoload<ApiClient>
{
    [Export] private string baseUrl = "";

    [Signal] public delegate void RequestCompletedEventHandler(Variant result);
    [Signal] public delegate void RequestFailedEventHandler(string error);

    private HttpClient client;
    private JsonSerializerOptions jsonOptions;

    public override void _Ready()
    {
        client = new HttpClient();
        jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        if (baseUrl == string.Empty) return;

        client.BaseAddress = new Uri(baseUrl);
    }

    public static void SetBaseUrl(string baseUrl)
    {
        Instance.baseUrl = baseUrl;
        Instance.client.BaseAddress = new Uri(baseUrl);
    }

    public static void SetDefaultHeader(string key, string value)
    {
        Instance.client.DefaultRequestHeaders.Add(key, value);
    }

    public static void SetAuthorizationBearer(string token)
    {
        Instance.client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public static void SetTimeout(float seconds)
    {
        Instance.client.Timeout = TimeSpan.FromSeconds(seconds);
    }

    public static async Task<string> GetAsync(string url)
    {
        var response = await Instance.client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<T> GetAsync<T>(string url)
    {
        var response = await Instance.client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, Instance.jsonOptions);
    }

    public static async Task<string> PostAsync(string url, string jsonData)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PostAsync(string url, object data)
    {
        var json = JsonSerializer.Serialize(data, Instance.jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<T> PostAsync<T>(string url, string jsonData)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, Instance.jsonOptions);
    }

    public static async Task<T> PostAsync<T>(string url, object data)
    {
        var json = JsonSerializer.Serialize(data, Instance.jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, Instance.jsonOptions);
    }

    public static async Task<string> PutAsync(string url, string jsonData)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PutAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PutAsync(string url, object data)
    {
        var json = JsonSerializer.Serialize(data, Instance.jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PutAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<T> PutAsync<T>(string url, string jsonData)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PutAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, Instance.jsonOptions);
    }

    public static async Task<T> PutAsync<T>(string url, object data)
    {
        var json = JsonSerializer.Serialize(data, Instance.jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Instance.client.PutAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, Instance.jsonOptions);
    }

    public static async Task<string> DeleteAsync(string url)
    {
        var response = await Instance.client.DeleteAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<T> DeleteAsync<T>(string url)
    {
        var response = await Instance.client.DeleteAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, Instance.jsonOptions);
    }

    public static async Task<ApiResponse<T>> PostWithResponseAsync<T>(string url, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, Instance.jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await Instance.client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, Instance.jsonOptions);

            if (apiResponse == null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Failed to parse server response",
                    Code = (int)response.StatusCode
                };
            }

            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Network error: {ex.Message}",
                Code = 0
            };
        }
        catch (TaskCanceledException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Request timed out",
                Code = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}",
                Code = 0
            };
        }
    }

    public static async Task<ApiResponse> PostWithResponseAsync(string url, object data)
    {
        var response = await PostWithResponseAsync<object>(url, data);
        return new ApiResponse
        {
            Code = response.Code,
            Success = response.Success,
            Message = response.Message,
            Data = response.Data,
            Error = response.Error,
            ErrorCode = response.ErrorCode
        };
    }

    public static async Task<ApiResponse<T>> GetWithResponseAsync<T>(string url)
    {
        try
        {
            var response = await Instance.client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, Instance.jsonOptions);

            if (apiResponse == null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Failed to parse server response",
                    Code = (int)response.StatusCode
                };
            }

            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Network error: {ex.Message}",
                Code = 0
            };
        }
        catch (TaskCanceledException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Request timed out",
                Code = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}",
                Code = 0
            };
        }
    }

    public static async Task<ApiResponse> GetWithResponseAsync(string url)
    {
        var response = await GetWithResponseAsync<object>(url);
        return new ApiResponse
        {
            Code = response.Code,
            Success = response.Success,
            Message = response.Message,
            Data = response.Data,
            Error = response.Error,
            ErrorCode = response.ErrorCode
        };
    }

    public static async Task<ApiResponse<T>> PutWithResponseAsync<T>(string url, object data)
    {
        try
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None);
            
            Utils.Logger.Debug(json);
            
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await Instance.client.PutAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, Instance.jsonOptions);

            if (apiResponse == null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Failed to parse server response",
                    Code = (int)response.StatusCode
                };
            }

            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Network error: {ex.Message}",
                Code = 0
            };
        }
        catch (TaskCanceledException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Request timed out",
                Code = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}",
                Code = 0
            };
        }
    }

    public static async Task<ApiResponse> PutWithResponseAsync(string url, object data)
    {
        var response = await PutWithResponseAsync<object>(url, data);
        return new ApiResponse
        {
            Code = response.Code,
            Success = response.Success,
            Message = response.Message,
            Data = response.Data,
            Error = response.Error,
            ErrorCode = response.ErrorCode
        };
    }

    public override void _ExitTree()
    {
        client?.Dispose();
    }
}