using Game.Entities;
using Game.Utils;
using Godot;


namespace Game.Components;
public partial class CustomPartButton : TextureButton
{
   
    [Export] public PartItem PartData;

    public override void _Ready()
    {
        TextureNormal = PartData.Icon;
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        Player player = this.GetPlayer();
        if (player != null)
        {
            player.ApplyPart(PartData);
        }
        else
        {
            GD.PrintErr("CustomPartButton: Player not found in scene tree.");
        }
    }
}
