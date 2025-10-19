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
    private static readonly string dir = GetSaveDir();
    private static readonly string path = $"{dir}/save.json";

    public override void _ExitTree() => Save();

    private static string GetSaveDir()
    {
        var isMobile = OS.HasFeature("mobile") || OS.HasFeature("android") || OS.HasFeature("ios");
        var isDesktop = OS.HasFeature("desktop") || OS.HasFeature("windows") || OS.HasFeature("linux") ||
                        OS.HasFeature("macos");

        if (isDesktop && OS.IsDebugBuild())
        {
            return "res://data";
        }

        return "user://data";
    }

    public static void StartSaving()
    {
        Load();

        var timer = new Timer { WaitTime = OS.IsDebugBuild() ? 15 : 60, Autostart = false };
        Instance.AddChild(timer);
        timer.Timeout += Save;
        timer.Start();
    }

    public static void Load()
    {
        Logger.Info("Loading save data...");
        if (FileAccess.FileExists(path))
        {
            var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var content = file.GetAsText();
            file.Close();

            Data = JsonConvert.DeserializeObject<Save>(content) ?? new Save();
            LoadInventory();
            LoadShopData();
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

    public static void SaveInventory()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.Items == null)
        {
            GD.PrintErr("[SaveInventory] InventoryManager or Items not initialized.");
            return;
        }

        GD.Print($"[SaveInventory] Saving {InventoryManager.Instance.Items.Count} items...");

        foreach (var group in InventoryManager.Instance.Items.Values)
        {
            if (group.Item == null)
            {
                GD.PrintErr("[SaveInventory] Tried to save null item, skipping...");
                continue;
            }

            var id = group.Item.Id;

            if (string.IsNullOrEmpty(id))
            {
                GD.PrintErr($"[SaveInventory] Could not resolve ID for item '{group.Item.Name}', skipping...");
                continue;
            }


            var existing = Data.Inventory.FirstOrDefault(i => i.Id == id);
            if (existing != null)
            {
                existing.Quantity = group.Quantity;
            }
            else
            {
                Data.Inventory.Add(new SavedItem
                {
                    Id = id,
                    Quantity = group.Quantity,
                    AcquiredAt = System.DateTime.UtcNow.ToString("s")
                });
            }
        }

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

    public static void LoadShopData()
    {
        if (Data == null) return;

        ShopManager.Stars = Data.Shop.Stars;
    }

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
        return Data?.Shop.PurchaseHistory ?? [];
    }
}