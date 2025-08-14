using System;
using System.Collections.Generic;
using Game.Data;
using Game.Utils;
using Godot;

namespace Game.Autoloads;

// TODO: Separate shop management and inventory management
[GlobalClass]
public partial class ShopManager : Autoload<ShopManager>
{
    [Signal]
    public delegate void CoinsChangedEventHandler(int coins);

    [Signal]
    public delegate void ItemBoughtEventHandler(Item item);

    public static int Coins { get; private set; } = 0;
    // public static IReadOnlyList<Item> ShopItems => shopItems.AsReadOnly();

    private static Dictionary<Type, List<Item>> shopItems = new()
    {
        { typeof(Consumable), [] },
        { typeof(Cosmetic), [] },
    };

    public override void _Ready()
    {
        shopItems[typeof(Consumable)] = ItemRegistry.GetItemsByType<Consumable>();
        shopItems[typeof(Cosmetic)] = ItemRegistry.GetItemsByType<Cosmetic>();
        GD.Print($"[DEBUG] Consumables loaded: {shopItems[typeof(Consumable)].Count}");
        GD.Print($"[DEBUG] Cosmetics loaded: {shopItems[typeof(Cosmetic)].Count}");
        foreach (var item in ShopManager.GetItemsByType<Cosmetic>())
        {
            GD.Print($"[DEBUG] UI processing cosmetic: {item.Name}, Icon: {item.Icon}");
            // Assign item.Icon to TextureRect, item.Name to Label, etc.
        }

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

        if (existingItem is null || existingItem is Cosmetic { Owned: true })
            return;

        if (Coins < item.Cost)
            return;

        SpendCoins(item.Cost);

        if (item is Cosmetic cosmetic)
        {
            cosmetic.Owned = true;
            InventoryManager.Instance.AddItem(cosmetic);
        }
        else if (item is Consumable consumable)
        {
            InventoryManager.Instance.AddItem(consumable, 1);
        }

        Instance.EmitSignalItemBought(item);
        Logger.Info($"Bought {item.Name}");
    }

}