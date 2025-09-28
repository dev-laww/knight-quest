// scripts/data/status_effect/LevelRegistry.cs
using System.Collections.Generic;
using System.Linq;
using Game.Utils;
using Godot;

namespace Game.Data;

[GlobalClass]
public partial class LevelRegistry : Registry<LevelInfo, LevelRegistry>
{
    // Path in your Godot project where level .tres files are stored
    protected override string ResourcePath => "res://resources/level/";
    public static LevelRegistry PublicInstance => Instance.Value;
    public static Dictionary<string, LevelInfo> PublicResources => Resources;

    public static List<LevelInfo> GetLevels() => Resources.Values.ToList();

    public static LevelInfo GetLevelById(string id)
    {
        Resources.TryGetValue(id, out var levelInfo);
        return levelInfo;
    }

    protected override void LoadResources()
    {
        var files = DirAccessUtils.GetFilesRecursively(ResourcePath);

        foreach (var file in files)
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".tres.remap")) continue;

            var resource = ResourceLoader.Load<LevelInfo>(file);
            if (resource == null) continue;

            var id = file.GetFile().GetBaseName();
            Resources[id] = resource;
        }
    }
}