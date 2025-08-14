using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Utils;
using Godot;

namespace Game.Autoloads;

[GlobalClass]
public partial class InventoryManager : Autoload<InventoryManager>
{
    [Signal]
    public delegate void UpdatedEventHandler(Item item, int quantity);

    private readonly Dictionary<Item, int> inventory = new();

    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Logger.Error("Tried to add a null item to inventory.");
            return;
        }

        if (!inventory.TryAdd(item, quantity))
            inventory[item] += quantity;
        
        // Sync Consumable quantity
        if (item is Consumable consumable)
            consumable.Quantity = inventory[item];

        EmitSignalUpdated(item, inventory[item]);
        Logger.Info($"Added {quantity}x {item.Name} to the inventory.");
    }

    public void UseItem(Item item, Entity target, int amount = 1)
    {
        if (item == null)
        {
            Logger.Error("Tried to use a null item.");
            return;
        }

        if (!inventory.TryGetValue(item, out int currentQuantity))
        {
            Logger.Warn($"Tried to use {item.Name} but it was not in the inventory.");
            return;
        }

        if (currentQuantity < amount)
        {
            Logger.Warn($"Not enough {item.Name} in inventory. Have {currentQuantity}, need {amount}.");
            return;
        }

        if (item is Consumable consumable)
        {
            for (int i = 0; i < amount; i++) consumable.Use(target);
        }

        currentQuantity -= amount;
        if (currentQuantity <= 0) inventory.Remove(item);
        else inventory[item] = currentQuantity;

        // Sync Consumable quantity
        if (item is Consumable consumable2)
            consumable2.Quantity = Math.Max(currentQuantity, 0);

        EmitSignalUpdated(item, Math.Max(currentQuantity, 0));
        Logger.Info($"Used {amount}x {item.Name}.");
    }

    public int GetQuantity(Item item) => inventory.GetValueOrDefault(item, 0);
public static List<Item> GetItemsByType<T>() where T : Item =>
    Instance.inventory.Keys.OfType<T>().Cast<Item>().ToList();

    public bool HasItem(Item item, int minQuantity = 1) =>
        inventory.TryGetValue(item, out var quantity) && quantity >= minQuantity;

    public void Clear()
    {
        inventory.Clear();
        Logger.Info("Inventory cleared.");
    }
}