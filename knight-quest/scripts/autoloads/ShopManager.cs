using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Utils;
using Godot;

namespace Game.Autoloads;

// TODO: Separate shop management and inventory management
public partial class ShopManager : Autoload<ShopManager>
{
    [Signal] public delegate void CoinsChangedEventHandler(int coins);

    [Signal] public delegate void ItemBoughtEventHandler(Item item);

    public static int Coins { get; private set; } = 0;
    // public static IReadOnlyList<Item> ShopItems => shopItems.AsReadOnly();

    private static Dictionary<Type, List<Item>> shopItems = new()
    {
        { typeof(Consumable), [] },
    };

    public override void _Ready()
    {
        shopItems[typeof(Consumable)] = ItemRegistry.GetItemsByType<Consumable>().Select(Item (item) => item).ToList();
        GD.Print($"[DEBUG] Consumables loaded: {shopItems[typeof(Consumable)].Count}");

        AddCoins(100);
    }

    public static void AddCoins(int amount)
    {
        Coins += amount;
        Instance.EmitSignalCoinsChanged(Coins);
    }

    public static void SpendCoins(int amount)
    {
        if (Coins < amount) return;

        Coins -= amount;
        Instance.EmitSignalCoinsChanged(Coins);
    }

    public static List<Item> GetItemsByType<T>() where T : Item =>
        shopItems.TryGetValue(typeof(T), out var items) ? items : [];

    public static void BuyItem(Item item)
    {
        var existingItem = shopItems[item.GetType()]
            .Find(i => i.ResourcePath == item.ResourcePath);

        if (existingItem is null)
            return;

        if (Coins < item.Cost)
            return;

        SpendCoins(item.Cost);

        InventoryManager.Instance.AddItem(item);

        Instance.EmitSignalItemBought(item);
        Logger.Info($"Bought {item.Name}");
    }
}