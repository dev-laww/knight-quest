using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;

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

    public override void _ExitTree()
    {
        client?.Dispose();
    }
}