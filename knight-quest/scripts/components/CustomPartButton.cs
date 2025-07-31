using Game.Entities;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game.Components;

public partial class CustomPartButton : TextureButton
{
    [Export] public PartItem PartData;
    private bool isEquipped = false;
    [Node]private Button equipButton;
    

    public override void _Ready()
    {
        TextureNormal = PartData.Icon;
        equipButton = GetNode<Button>("EquipButton");
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

        if (player.IsPartEquipped(PartData))
        {
            player.UnequipPart(PartData);
        }
        else
        {
            player.ApplyPart(PartData);
        }
        UpdateEquipButtonText();
    }

    private void UpdateEquipButtonText()
    {
        var player = this.GetPlayer();
        equipButton.Text = player != null && player.IsPartEquipped(PartData) ? "Unequip" : "Equip";
    }
}