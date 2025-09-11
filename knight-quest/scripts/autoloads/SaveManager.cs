using System.Collections.Generic;
using Game.Data;
using Game.Utils;
using Godot;
using Newtonsoft.Json;

namespace Game.Autoloads;

public partial class SaveManager : Autoload<SaveManager>
{
    public static Save Data { get; private set; }
    private static readonly string dir = $"{(OS.IsDebugBuild() ? "res" : "user")}://data";
    private static readonly string path = $"{dir}/";
    
    
    public override void _EnterTree()
    {
        Load();
        var timer = new Timer { WaitTime = OS.IsDebugBuild() ? 15 : 60, Autostart = true };
        AddChild(timer);
        timer.Timeout += Save;
    }
    
    public override void _ExitTree()
    {
        Save();
    }
    
    public static void Load()
    {
        Logger.Info("Loading save data...");
        if (FileAccess.FileExists(path))
        {
            var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var content = file.GetAsText();
            file.Close();

            Data = JsonConvert.DeserializeObject<Save>(content);
        }
        else
        {
            Logger.Debug("Save data not found creating new save data...");
            DirAccess.MakeDirAbsolute(dir);
        }

        Data ??= new Save();
    }


    public static void Save()
    {
        Logger.Info("Saving data...");
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);

        var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
        file.StoreString(json);
        file.Close();
    }

    public static void UnlockLevel(string level) => Data.UnlockLevel(level);
}