using System.Collections.Generic;
using System.ComponentModel;
using Game.UI;
using Newtonsoft.Json;
namespace Game.Data;

public class Save
{
    [JsonProperty("username")] public string Username { get; private set; } = string.Empty;
    [JsonProperty("password")] public string Password { get; private set; } = string.Empty;
    [JsonProperty("unlock_level")] public List<string> UnlockedLevel { get; private set; } = [];
    [JsonProperty("items")] public List<Item> Items { get; private set; } = [];
    [JsonProperty("stars")] public int Stars { get; private set; } = 1;
    
    
    
    
    public void SetItems(List<Item> items) => Items = items;
    
    
    public void UnlockLevel(string level)
    {
        if (UnlockedLevel.Contains(level)) return;
        UnlockedLevel.Add(level);
    }

    
    
    
}