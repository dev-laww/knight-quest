using Game.Data;
using Game.Entities;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game.Components;

[Scene]
public partial class CustomPartButton : TextureButton
{
    [Export] public Cosmetic PartData;

    [Node] private Button equipButton;

    private bool isEquipped;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        TextureNormal = PartData.Icon;
        equipButton.Visible = false;
        equipButton.Pressed += ToggleEquipState;
        Pressed += OnCustomPartButtonPressed;
        UpdateEquipButtonText();
    }

    private void OnCustomPartButtonPressed()
    {
        equipButton.Visible = true;
    }

    private void ToggleEquipState()
    {
        var player = this.GetPlayer();
        if (player == null) return;

        if (player.IsCosmeticEquipped(PartData))
        {
            player.UnequipCosmetic(PartData);
        }
        else
        {
            player.EquipCosmetic(PartData);
        }

        UpdateEquipButtonText();
    }

    private void UpdateEquipButtonText()
    {
        var player = this.GetPlayer();
        equipButton.Text = player != null && player.IsCosmeticEquipped(PartData) ? "Unequip" : "Equip";
    }
}