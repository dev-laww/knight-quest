using System.Collections.Generic;

namespace Game.Utils.Network;

// Response wrapper class
public class HttpResponse<T>
{
    public bool IsSuccess { get; init; }
    public System.Net.HttpStatusCode StatusCode { get; init; }
    public string Content { get; init; } = string.Empty;
    public T? Data { get; init; }
    public string? Error { get; init; }
    public Dictionary<string, string> Headers { get; init; } = new();

    public override string ToString()
    {
        return $"HttpResponse<{typeof(T).Name}>(Success: {IsSuccess}, Status: {StatusCode}, Error: {Error})";
    }
}
