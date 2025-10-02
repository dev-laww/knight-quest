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

    private static Account currentAccount;

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

    public static bool Register(string username, string password)
    {
        if (Data == null) Load();

        if (Data.Accounts.Exists(a => a.Username == username))
        {
            Logger.Warn("Account already exists!");
            return false;
        }

        var account = new Account
        {
            Username = username,
            Token = "",
            FirstName = "",
            LastName = "",
            Role = "",
            Progression = new Progression(),
            Inventory = new List<SavedItem>(),
            Shop = new Shop()
        };

        Data.AddAccount(account);
        Save();
        Logger.Info($"Account '{username}' registered successfully!");
        return true;
    }

    public static bool Login(string username, string password)
    {
        if (Data == null) Load();

        var account = Data.Accounts.Find(a => a.Username == username /* && a.Password == password */);

        if (account != null)
        {
            currentAccount = account;
            Logger.Info($"Login successful! Welcome {account.Username}!");
            return true;
        }

        Logger.Warn("Invalid username or password!");
        return false;
    }

    public static Account CurrentAccount => currentAccount;

    // Inventory
    public static void SaveInventory()
    {
        if (CurrentAccount == null) return;

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

        CurrentAccount.Inventory = savedItems;
        Save();
    }


    public static void LoadInventory()
    {
        if (CurrentAccount == null) return;

        InventoryManager.Instance.ClearInventory();

        foreach (var saved in CurrentAccount.Inventory)
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
        if (CurrentAccount == null) return;
        var finishedLevel = new FinishedLevel
        {
            Id = levelId,
            StarsEarned = starsEarned,
            CompletedAt = System.DateTime.UtcNow.ToString("s")
        };
        CurrentAccount.Progression.LevelsFinished.Add(finishedLevel);
        Save();
    }

    public static List<FinishedLevel> LoadFinishedLevels()
    {
        return CurrentAccount?.Progression.LevelsFinished ?? new List<FinishedLevel>();
    }

    // Shop
    public static void SaveShopPurchase(string itemId, int quantity, int cost)
    {
        if (CurrentAccount == null) return;
        var purchase = new PurchaseHistory
        {
            Id = itemId,
            Quantity = quantity,
            Cost = cost,
            PurchasedAt = System.DateTime.UtcNow.ToString("s")
        };
        CurrentAccount.Shop.PurchaseHistory.Add(purchase);
        Save();
    }

    public static List<PurchaseHistory> LoadShopHistory()
    {
        return CurrentAccount?.Shop.PurchaseHistory ?? new List<PurchaseHistory>();
    }
}