using Game.Autoloads;
using Godot;
using GodotUtilities;
using GodotUtilities.Util;

namespace Game;

[Scene]
public partial class MainMenu : CanvasLayer
{
    [Node] private Button startButton;
    [Node] private Button shopButton;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        if (SaveManager.CurrentAccount != null)
        {
            SaveManager.LoadInventory();
            ShopManager.Stars = SaveManager.CurrentAccount.Stars; 
            Logger.Info($"Loaded inventory and stars for {SaveManager.CurrentAccount.Username}");
            foreach (var group in SaveManager.CurrentAccount.Items)
            {
                Logger.Info($"Loaded item: {group.Item.Name} x{group.Quantity}");
            }
        }
        else
        {
            InventoryManager.Instance.ClearInventory();
            ShopManager.Stars = 0;
            Logger.Error("No account logged in, inventory cleared and stars set to 0.");
        }

        startButton.Pressed += OnStartButtonPressed;
        shopButton.Pressed += OnShopButtonPressed;
    }
    private void OnStartButtonPressed()
    {
        Navigator.Push("res://scenes/ui/screens/subject_select.tscn");
    }

    private void OnShopButtonPressed()
    {
        Navigator.Push("res://scenes/ui/screens/shop.tscn");
    }
}