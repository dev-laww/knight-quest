using System.Collections.Generic;
using Game.Data;
using Game.Utils;
using Godot;
using Newtonsoft.Json;

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
            Password = password
        };

        Data.AddAccount(account);
        Save();
        Logger.Info($"Account '{username}' registered successfully!");
        return true;
    }

    public static bool Login(string username, string password)
    {
        if (Data == null) Load();

        var account = Data.Accounts.Find(a => 
            a.Username == username && a.Password == password);

        if (account != null)
        {
            currentAccount = account;
            Logger.Info($"Login successful! Welcome {account.Username}!");
            return true;
        }

        Logger.Warn("Invalid username or password!");
        return false;
    }
    
    
    public static void SaveInventory()
    {
        if (CurrentAccount == null)
        {
            Logger.Warn("No account is currently logged in, cannot save inventory.");
            return;
        }

        // Convert runtime inventory dictionary to list
        var itemList = new List<ItemGroup>(InventoryManager.Instance.Items.Values);
        CurrentAccount.Items = itemList;

        Save();
        Logger.Info($"Inventory saved for {CurrentAccount.Username}");
    }
    
    public static void LoadInventory()
    {
        if (CurrentAccount == null)
        {
            Logger.Warn("No account is currently logged in, cannot load inventory.");
            return;
        }

        InventoryManager.Instance.ClearInventory();

        foreach (var group in CurrentAccount.Items)
        {
            for (int i = 0; i < group.Quantity; i++)
                InventoryManager.Instance.AddItem(group.Item);
        }

        Logger.Info($"Inventory loaded for {CurrentAccount.Username}");
    }
    




    public static Account CurrentAccount => currentAccount;
}
