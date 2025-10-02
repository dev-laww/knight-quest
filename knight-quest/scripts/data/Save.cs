using System.Collections.Generic;
using Newtonsoft.Json;

namespace Game.Data;

public class Save
{
    public Account Account = new();
    public Progression Progression { get; set; } = new();
    public List<SavedItem> Inventory { get; set; } = [];
    public Shop Shop { get; set; } = new();
}