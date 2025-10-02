using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Godot;
using Newtonsoft.Json;
using Logger = Game.Utils.Logger;

namespace Game.Autoloads;

public partial class SaveManager : Autoload<SaveManager>
{
    public static Save Data { get; private set; } = new();
    private static readonly string dir = "user://data";
    private static readonly string path = $"{dir}/save.json";

    public override void _EnterTree()
    {
        Load();
        var timer = new Timer { WaitTime = OS.IsDebugBuild() ? 15 : 60, Autostart = true };
        AddChild(timer);
        timer.Timeout += Save;
    }

    public override void _ExitTree() => Save();

    public static void Load()
    {
        Logger.Info("Loading save data...");
        if (FileAccess.FileExists(path))
        {
            var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var content = file.GetAsText();
            file.Close();

            Data = JsonConvert.DeserializeObject<Save>(content) ?? new Save();
        }
        else
        {
            Logger.Debug("Save data not found creating new save data...");
            DirAccess.MakeDirAbsolute(dir);
            Data = new Save();
        }
    }

    public static void Save()
    {
        Logger.Info("Saving data...");
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
        file.StoreString(json);
        file.Close();
    }

    // Inventory
    public static void SaveInventory()
    {
        var savedItems = InventoryManager.Instance.Items.Values
            .Select(group =>
            {
                if (group.Item == null)
                {
                    GD.PrintErr("[SaveInventory] Tried to save null item, skipping...");
                    return null;
                }

                var id = ItemRegistry.PublicResources
                    .FirstOrDefault(x => x.Value == group.Item).Key;

                if (string.IsNullOrEmpty(id))
                {
                    GD.PrintErr(
                        $"[SaveInventory] Could not find registry ID for item '{group.Item.Name}', skipping...");
                    return null;
                }

                return new SavedItem
                {
                    Id = id,
                    Quantity = group.Quantity,
                    AcquiredAt = System.DateTime.UtcNow.ToString("s")
                };
            })
            .Where(si => si != null) // filter out invalid items
            .ToList();

        Data.Inventory = savedItems;
        Save();
    }


    public static void LoadInventory()
    {
        if (Data == null) return;

        InventoryManager.Instance.ClearInventory();

        foreach (var saved in Data.Inventory)
        {
            if (string.IsNullOrEmpty(saved.Id))
            {
                GD.PrintErr("[LoadInventory] Skipping item with null/empty Id");
                continue;
            }

            var item = ItemRegistry.Get(saved.Id);
            if (item != null)
            {
                for (int i = 0; i < saved.Quantity; i++)
                    InventoryManager.Instance.AddItem(item);
            }
            else
            {
                GD.PrintErr($"[LoadInventory] Failed to load item with id: {saved.Id}");
            }
        }
    }

    // Finished Levels
    public static void SaveFinishedLevel(string levelId, int starsEarned)
    {
        if (Data == null) return;
        var finishedLevel = new FinishedLevel
        {
            Id = levelId,
            StarsEarned = starsEarned,
            CompletedAt = System.DateTime.UtcNow.ToString("s")
        };
        Data.Progression.LevelsFinished.Add(finishedLevel);
        Save();
    }

    public static List<FinishedLevel> LoadFinishedLevels()
    {
        return Data?.Progression.LevelsFinished ?? new List<FinishedLevel>();
    }

    // Shop
    public static void SaveShopPurchase(string itemId, int quantity, int cost)
    {
        if (Data == null) return;
        var purchase = new PurchaseHistory
        {
            Id = itemId,
            Quantity = quantity,
            Cost = cost,
            PurchasedAt = System.DateTime.UtcNow.ToString("s")
        };
        Data.Shop.PurchaseHistory.Add(purchase);
        Save();
    }

    public static List<PurchaseHistory> LoadShopHistory()
    {
        return Data?.Shop.PurchaseHistory ?? new List<PurchaseHistory>();
    }
}