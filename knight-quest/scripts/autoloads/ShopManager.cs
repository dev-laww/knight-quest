using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Utils;
using Godot;

namespace Game.Autoloads;

// TODO: Separate shop management and inventory management
[GlobalClass]
public partial class ShopManager : Autoload<ShopManager>
{
    [Signal] public delegate void CoinsChangedEventHandler(int coins);

    [Signal] public delegate void ItemBoughtEventHandler(Item item);

    public static int stars { get; private set; } = 0;
    // public static IReadOnlyList<Item> ShopItems => shopItems.AsReadOnly();

    private static Dictionary<Type, List<Item>> shopItems = new()
    {
        { typeof(Consumable), [] },
    };

    public override void _Ready()
    {
        shopItems[typeof(Consumable)] = ItemRegistry.GetItemsByType<Consumable>().Select(Item (item) => item).ToList();
        GD.Print($"[DEBUG] Consumables loaded: {shopItems[typeof(Consumable)].Count}");

        if (shopItems[typeof(Consumable)].Count > 0)
        {
            var firstConsumable = shopItems[typeof(Consumable)][0];
            InventoryManager.Instance.AddItem(firstConsumable);
            GD.Print($"[DEBUG] Added {firstConsumable.Name} to inventory.");
            AddCoins(100);// Testing money
        }
    }

    public static void AddCoins(int amount)
    {
        stars += amount;
        Instance.EmitSignalCoinsChanged(stars);
    }

    public static void SpendCoins(int amount)
    {
        if (stars < amount) return;

        stars -= amount;
        Instance.EmitSignalCoinsChanged(stars);
    }

    public static List<Item> GetItemsByType<T>() where T : Item =>
        shopItems.TryGetValue(typeof(T), out var items) ? items : [];

    public static void BuyItem(Item item)
    {
        if (item is null) return;
        if (stars < item.Cost) return;

        var typeKey = typeof(Consumable);

        if (!shopItems.TryGetValue(typeKey, out var items))
            return;

        var existingItem = items.Find(i => i.ResourcePath == item.ResourcePath);
        if (existingItem is null) return;
        
        SpendCoins(item.Cost);
        
        InventoryManager.Instance.AddItem(item);

        Instance.EmitSignalItemBought(item);
        
        Logger.Info($"Bought {item.Name}");
       if (InventoryManager.Instance.Items.TryGetValue(item, out var itemGroup))
       {
           Logger.Info($"{item.Name} Quantity: {itemGroup.Quantity}");
       }
       else
       {
           Logger.Info($"{item.Name} not found in inventory.");
       }
    }

}