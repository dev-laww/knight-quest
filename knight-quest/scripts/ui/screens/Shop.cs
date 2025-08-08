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
    private Item selectedItem;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        coinLabel.Text = $"Coins: {ShopManager.Coins}";

        slots = slotContainer.GetChildrenOfType<Slot>().ToList();
        slots.ForEach(slot => slot.Pressed += SelectSlot);

        buyButton.Pressed += OnBuyButtonPress;
        closeButton.Pressed += Close;

        PopulateSlots();
        Reset();
    }

    private void PopulateSlots()
    {
        var items = ShopManager.GetItemsByType<Cosmetic>();

        for (int i = 0; i < slots.Count && i < items.Count; i++)
        {
            var slot = slots[i];
            var item = items[i];

            slot.Item = item;
            slot.icon.Texture = item.Icon;
        }
    }

    private void SelectSlot(Slot slot)
    {
        var selectedSlot = slots.FirstOrDefault(s => s.Selected);
        if (selectedSlot != null)
            selectedSlot.Selected = false;

        slot.Selected = true;
        UpdateSelectedItem(slot.Item);

        if (slot.Item is Cosmetic cosmetic)
            playerDummy.PreviewCosmetic(cosmetic);
    }

    private void UpdateSelectedItem(Item item)
    {
        selectedItem = item;
        selectedItemName.Text = item?.Name;
        selectedItemDescription.Text = item?.Description ?? string.Empty;
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
        return selectedItem != null && ShopManager.Coins >= selectedItem.Cost;
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