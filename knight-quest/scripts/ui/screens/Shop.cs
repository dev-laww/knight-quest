using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Autoloads;
using Game.Data;
using Game.Entities;
using Game.Utils;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class Shop : CanvasLayer
{
    [Node] private TextureButton closeButton;
    [Node] private GridContainer slotContainer;
    [Node] private Label coinLabel;
    [Node] private Label selectedItemName;
    [Node] private RichTextLabel selectedItemDescription;
    [Node] private Button buyButton;
    [Node] private Player playerDummy;
    

    [Signal]
    public delegate void ItemBoughtEventHandler();

    private List<Slot> slots;
    private ItemGroup selectedItem;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        coinLabel.Text = $"Coins: {ShopManager.Coins}";

        // Grab only the Slot nodes from slotContainer
        slots = slotContainer.GetChildren()
            .OfType<Slot>() // filters only Slot type
            .ToList();

        slots.ForEach(slot => slot.Pressed += SelectSlot);

        buyButton.Pressed += OnBuyButtonPress;
        closeButton.Pressed += Close;

        PopulateSlots();
        Reset();
    }


    private void PopulateSlots()
    {
        var items = ShopManager.GetItemsByType<Consumable>();
        slots.Where(slot => slot.Item != null).ToList().ForEach(slot => slot.Item = null);

        for (var i = 0; i < items.Count; i++)
        {
            slots[i].Item.Item = items[i];
            
        }
        SelectSlot(slots.First());;
      UpdateSelectedItem(slots.First().Item);

    }

    private void SelectSlot(Slot slot)
    {
        var selectedSlot = slots.FirstOrDefault(s => s.Selected);

        if (selectedSlot is null)
        {
            slot.Selected = true;
            return;
        }

        if (selectedSlot == slot) return;

        slot.Selected = true;
        selectedSlot.Selected = false;
        UpdateSelectedItem(slot.Item);
    }

    private void UpdateSelectedItem(ItemGroup item)
    {
        selectedItem = item;
        selectedItemName.Text = item?.Item.Name;
        selectedItemDescription.Text = item?.Item.Description ?? string.Empty;
        
        UpdateButtonState();
    }

    private void OnBuyButtonPress()
    {
        if (!CanBuy()) return;

        Buy();
        EmitSignalItemBought();
        Close();
    }

    private void Reset()
    {
        if (!IsInstanceValid(this) || slots.Count == 0) return;
        SelectSlot(slots.First());
    }

    private void UpdateButtonState()
    {
        buyButton.Visible = selectedItem != null;
        buyButton.Disabled = !CanBuy();
        buyButton.Text = CanBuy() ? "Buy" : "Not enough Coins";
    }

    private bool CanBuy()
    {
        return selectedItem != null && ShopManager.Coins >= selectedItem.Item.Cost;
    }


    private void Buy()
    {
        ShopManager.BuyItem(selectedItem);
    }

    private void Close()
    {
        Hide();
        Reset();
    }
}