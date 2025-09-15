using Newtonsoft.Json;
using System.Collections.Generic;

namespace Game.Data;

public class Account
{
    [JsonProperty("username")] 
    public string Username { get; set; } = string.Empty;

    [JsonProperty("password")] 
    public string Password { get; set; } = string.Empty;

    [JsonProperty("unlock_level")] 
    public List<string> UnlockedLevel { get; set; } = new();

    [JsonProperty("items")] 
    public List<ItemGroup> Items { get; set; } = new();

    [JsonProperty("stars")] 
    public int Stars { get; set; } = 0;

    // --- Safe methods ---
    public void UnlockLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level)) return;
        if (!UnlockedLevel.Contains(level))
            UnlockedLevel.Add(level);
    }

    public void SetItems(List<ItemGroup> items)
    {
        Items = items ?? new List<ItemGroup>();
    }
}