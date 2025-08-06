using System;
using System.Collections.Generic;
using Game.Data;
using Godot;

namespace Game.Autoloads;

// TODO: Separate shop management and inventory management
[GlobalClass]
public partial class ShopManager : Autoload<ShopManager>
{
    [Signal] public delegate void ItemBoughtEventHandler(Item item);
    [Signal] public delegate void CoinsChangedEventHandler(int coins);

    public static int Coins { get; private set; } = 0;
    // public static IReadOnlyList<Item> ShopItems => shopItems.AsReadOnly();

    private static Dictionary<Type, List<Item>> shopItems = new()
    {
        { typeof(Consumable), [] },
        { typeof(Cosmetic), [] },
    };

    public override void _Ready()
    {
        // load shop items
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
        var existingItem = shopItems[item.GetType()].Find(i => i.ResourcePath == item.ResourcePath);

        if (existingItem is null or Cosmetic { Owned: true }) return;

        SpendCoins(item.Cost);

        existingItem.Owned = true;

        if (item is Consumable consumable)
        {
            consumable.Quantity++;
        }

        Instance.EmitSignalItemBought(item);
    }
}